from django.conf import settings
from elasticsearch import Elasticsearch
from elasticsearch.helpers import bulk
from elasticsearch_dsl import Q
from aliss.models import ServiceArea
from django.db.models import Case, When
import json
from shapely.geometry import shape, Point
import os

def _get_connection():
    import certifi
    return Elasticsearch([settings.ELASTICSEARCH_URL], http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))


def get_connection():
    return _get_connection()


service_mapping = {
    'id': {'type': 'keyword'},
    'organisation': {
        'properties': {
            'id': {'type': 'keyword'},
            'name': {'type': 'text'},
            'slug': {'type': 'keyword'}
        }
    },
    'created_on': {'type': 'date'},
    'updated_on': {'type': 'date'},
    'last_edited': {'type': 'date'},
    'slug': {'type': 'keyword'},
    'name': {
        'type': 'text',
        'analyzer': 'bigram_combiner'
    },
    'description': {
        'type': 'text',
        'analyzer': 'description_analyzer',
    },
    'url': {'type': 'keyword'},
    'phone': {'type': 'keyword'},
    'email': {'type': 'keyword'},
    'categories': {
        'properties': {
            'id': {'type': 'integer'},
            'name': {'type': 'keyword'},
            'slug': {'type': 'keyword'}
        }
    },
    'parent_categories': {
        'properties': {
            'id': {'type': 'integer'},
            'name': {'type': 'keyword'},
            'slug': {'type': 'keyword'}
        }
    },
    'locations': {
        'properties': {
            'id': {'type': 'keyword'},
            'formatted_address': {'type': 'keyword'},
            'name': {'type': 'text'},
            'description': {
                'type': 'text',
                'analyzer': 'description_analyzer',
            },
            'street_address': {'type': 'keyword'},
            'locality': {'type': 'keyword'},
            'region': {'type': 'keyword'},
            'state': {'type': 'keyword'},
            'postal_code': {'type': 'keyword'},
            'country': {'type': 'keyword'},
            'point': {'type': 'geo_point'},
        }
    },
    'service_areas': {
        'properties': {
            'id': {'type': 'integer'},
            'code': {'type': 'keyword'},
            'type': {'type': 'keyword'},
            'name': {'type': 'keyword'}
        }
    }
}

organisation_mapping = {
    'id': {'type': 'keyword'},
    'name': {
        'type': 'text',
        'analyzer': 'bigram_combiner'},
    'description': {
        'type': 'text',
        'analyzer': 'description_analyzer',
    },
    'published':{'type':'boolean'},
    'created_on':{'type':'date'},
    'is_claimed':{'type':'boolean'},
    'slug': {'type': 'keyword'},
    'locations': {
        'properties': {
            'id': {'type': 'keyword'},
            'formatted_address': {'type': 'keyword'},
            'name': {'type': 'text'},
            'description': {
                'type': 'text',
                'analyzer': 'description_analyzer',
            },
            'street_address': {'type': 'keyword'},
            'locality': {'type': 'keyword'},
            'region': {'type': 'keyword'},
            'state': {'type': 'keyword'},
            'postal_code': {'type': 'keyword'},
            'country': {'type': 'keyword'},
            'point': {'type': 'geo_point'},
        }
    }

}


def service_to_body(service):
    parent_categories =[]
    for category in service.categories.all():
        while category.parent:
            parent_categories.append(category.parent)
            category = category.parent

    return {
        'id': str(service.id),
        'organisation': {
            'id': service.organisation.pk,
            'name': service.organisation.name,
            'is_claimed': service.organisation.is_claimed,
            'slug': service.organisation.slug
        },
        'created_on': service.created_on,
        'updated_on': service.updated_on,
        'last_edited': service.last_edited,
        'name': service.name,
        'description': service.description,
        'slug': service.slug,
        'url': service.url,
        'email': service.email,
        'phone': service.phone,
        'categories': [{
            'id': category.pk,
            'name': category.name,
            'slug': category.slug
        } for category in service.categories.all()],
        'parent_categories': [{
            'id': category.pk,
            'name': category.name,
            'slug': category.slug
        } for category in parent_categories],
        'locations': [{
            'id': location.id,
            'name': location.name,
            'formatted_address': location.formatted_address,
            'description': location.description,
            'street_address': location.street_address,
            'locality': location.locality,
            'region': location.region,
            'state': location.state,
            'postal_code': location.postal_code,
            'country': location.country,
            'point': {
                'lat': location.latitude,
                'lon': location.longitude
            }
        } for location in service.locations.all()],
        'service_areas': [{
            'id': service_area.pk,
            'code': service_area.code,
            'type': service_area.get_type_display(),
            'type_code': service_area.type,
            'name': service_area.name,
        } for service_area in service.service_areas.all()]
    }

def organisation_to_body(organisation):
    return {
        'id': str(organisation.id),
        'name': organisation.name,
        'description': organisation.description,
        'published': organisation.published,
        'created_on': organisation.created_on,
        'is_claimed': organisation.is_claimed,
        'slug': organisation.slug,
        'locations': [{
            'id': location.id,
            'name': location.name,
            'formatted_address': location.formatted_address,
            'description': location.description,
            'street_address': location.street_address,
            'locality': location.locality,
            'region': location.region,
            'state': location.state,
            'postal_code': location.postal_code,
            'country': location.country,
            'point': {
                'lat': location.latitude,
                'lon': location.longitude
            }
        } for location in organisation.locations.all()]
    }


def filter_by_query(queryset, q):
    queryset = queryset.query({
        "bool": {
            "must": [{
                "multi_match": {
                    "query": q, "operator": "and", "type": "most_fields",
                    "fields": ["name^2", "description^1.5", "categories.name"],
                    "fuzziness": "AUTO:4,7"
                }
            }],
            "should": [{
                "multi_match": {
                    "query": q, "operator": "or", "type": "most_fields",
                    "fields": ["name^2", "description^1.5", "categories.name"]
                }
            }]
        }
    })
    return queryset


def filter_organisations_by_query(queryset, q):
    queryset = queryset.query({
        "bool": {
            "must":[
                {
                    "multi_match": {
                        "query": q, "type": "most_fields",
                        "operator": "and",
                        "fields":["name^2", "description"],
                        "fuzziness": "AUTO:4,7"
                    }
                }],
            "should": [
                {
                    "multi_match": {
                        "query": q, "type": "most_fields",
                        "operator": "or",
                        "fields": ["name^2", "description^1.5"],
                    }
                }
            ]
        }
    })
    return queryset


def filter_organisations_by_published(queryset, published=True):
    published_str = "false"
    if published:
        published_str = "true"
    queryset = queryset.query({
        "bool":{
            "must":{ "term":{ "published": published_str } }
        }
    })
    return queryset


def filter_organisations_by_query_published(queryset, q):
    queryset = filter_organisations_by_query(queryset, q)
    queryset = filter_organisations_by_published(queryset)
    return queryset


def filter_by_postcode(queryset, postcode, radius=5000):
    # Give us everything that:
    # A) has a service area that is connected to our postcode
    # B) is within radius distance AND has either no service_areas or a
    #    service area in our postcode
    # This ensures that all returned results serve the postcode in some way
    # Results that are within radius but have a service area that is not
    # connected to the postcode, such as a close by service that serves a
    # different council area will not be shown.

    queryset = queryset.filter(
        Q('terms', service_areas__code=postcode.codes) |
        (
            (~Q('exists', field='service_areas') | Q('terms', service_areas__code=postcode.codes)) &
             Q('geo_distance', distance="{0}m".format(radius), locations__point={
                "lat": postcode.latitude,
                "lon": postcode.longitude
            })
        )
    )

    queryset = queryset.query(
        Q(
            {'function_score': {
                "functions": [{
                    "gauss": {
                        "locations.point": {
                            "origin": {
                                "lat" : postcode.latitude,
                                "lon" : postcode.longitude
                            },
                        "scale": "{0}m".format(radius/3),
                        "decay": 0.3
                        }
                    },
            }]}}
        )
    )
    return queryset


def sort_by_postcode(queryset, postcode):
    queryset = queryset.sort({
        '_geo_distance': {
            "locations.point": {"lat": postcode.latitude, "lon": postcode.longitude },
            "order":"asc", "unit":"m"
        },
        '_score': { 'order': 'desc' }
    })
    return queryset


def sort_by_score(queryset):
    queryset = queryset.sort({
        "_score": { "order": "desc" }
    })
    return queryset


def filter_by_category(queryset, category):
    return queryset.query('function_score',
        query=Q('term', categories__id=category.pk) | Q('term', parent_categories__id=category.pk),
        functions=[
            {
                'weight': 2,
                'filter': Q('term', categories__id=category.pk)
            }
        ],
        score_mode="sum",
    )


def filter_by_location_type(queryset, type):
    if type == 'local':
        return queryset.exclude('terms', service_areas__type_code=[
            ServiceArea.COUNTRY,
            ServiceArea.REGION
        ])
    elif type == 'national':
        return queryset.filter('terms', service_areas__type_code=[
            ServiceArea.COUNTRY,
            ServiceArea.REGION
        ])
    else:
        return queryset


def get_service(queryset, service_id):
    return queryset.query(Q({
        "term" : { "id" : service_id }
    })).execute()


def get_services(queryset, service_ids):
    return queryset.query({
        "terms" : { "id" : service_ids}
    })


def get_organisations(queryset, org_ids):
    return queryset.query({
        "terms" : { "id" : org_ids}
    })


def get_organisation_by_id(queryset, organisation_id):
    return queryset.query(Q({
        "term": {"id": organisation_id }
    })).execute()


def filter_by_last_edited(queryset, comparison_date):
    queryset = queryset.query({
        "bool": {
            "filter": {"range":{"last_edited":{"gte":comparison_date}}}
    }})
    return queryset


def order_organistations_by_created_on(queryset):
    queryset = queryset.sort({
        "created_on":"desc"
    })
    return queryset


def filter_by_created_on(queryset, comparison_date):
    queryset = queryset.query({
        "bool": {
            "filter": {"range":{"created_on":{"gte":comparison_date}}}
    }})
    return queryset


def positions_dict(queryset, distance_sort_boolean):
    results = 9999
    if queryset.count() < results:
        results = queryset.count()
    sorted_hits = queryset[0:results].execute()
    positions = {}
    i = 0
    while i < results:
        positions[sorted_hits[i].id] = None
        if "sort" not in sorted_hits[i].meta:
            positions[sorted_hits[i].id] = {"place":i, "score": None}
        elif type(sorted_hits[i].meta.sort[0]) == float:
            if distance_sort_boolean:
                positions[sorted_hits[i].id] = {"place":i, "score":sorted_hits[i].meta.sort[0]}
            else:
                positions[sorted_hits[i].id] = {"place":i, "score": None}

        else:
            positions[sorted_hits[i].id] = {"place":i, "score": None}
        i=i+1
    return positions


def postcode_order(queryset, postcode):
    postcode_sqs = sort_by_postcode(queryset, postcode)
    positions = positions_dict(postcode_sqs, True)
    return {
        "ids": list(positions.keys()),
        "order": Case(*[When(id=key, then=positions[key]["place"]) for key in positions]),
        "distance_scores": generate_distance_scores(positions)
    }


def keyword_order(queryset):
    positions = positions_dict(queryset, False)
    return {
        "ids": list(positions.keys()),
        "order": Case(*[When(id=key, then=positions[key]["place"]) for key in positions]),
        "distance_scores": generate_distance_scores(positions)
    }


def generate_distance_scores(positions):
    distance_scores = {}
    for key in positions:
        distance_scores[key] = positions[key]["score"]
    return distance_scores


def combined_order(filtered_queryset, postcode):
    postcode_sqs = sort_by_postcode(filtered_queryset, postcode)
    distance_sorted = positions_dict(postcode_sqs, True)
    keyword_sorted  = positions_dict(filtered_queryset, False)
    positions = { "distance": distance_sorted, "keyword": keyword_sorted }
    combined = {}

    for key in positions["distance"]:
      if positions["distance"][key]["place"] == None:
        combined[key]["place"] = float(positions["keyword"][key]["place"])
      else:
        total = positions["distance"][key]["place"] + positions["keyword"][key]["place"]
        distance = positions["distance"][key]["score"]
        combined[key] = {"place":(total/2.0), "score":distance}
    return {
        "ids": list(combined.keys()),
        "order": Case(*[When(id=key, then=combined[key]["place"]) for key in combined]),
        "distance_scores": generate_distance_scores(combined)
    }


def filter_by_claimed_status(queryset, claimed_status):
    queryset = queryset.query({
        "bool":{
            "filter":{
                "term":{
                    "is_claimed":claimed_status
                    }
                }
            }
    })
    return queryset


def find_boundary_matches(boundary, long_lat):
    with open(boundary['data_file_path']) as f:
        js = json.load(f)
    point = Point(long_lat)
    boundary_matches = []
    data_set_keys = boundary['data_set_keys']
    for feature in js['features']:
        polygon = shape(feature['geometry'])
        if polygon.contains(point):
            boundary_matches.append({
            'code-type':data_set_keys['data_set_name'],
            'code':feature['properties'][data_set_keys['code']],
            'name':feature['properties'][data_set_keys['name']],
            })
    return boundary_matches


def check_boundaries(long_lat):
    boundaries_data_mappings = setup_data_set_doubles()
    boundary_matches = []
    for service_area, boundary in boundaries_data_mappings.items():
        matches = find_boundary_matches(boundary, long_lat)
        if len(matches) > 0:
            boundary_matches = boundary_matches + matches
    return boundary_matches


def setup_data_set_doubles():
    boundaries_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), './data/boundaries'))
    boundaries_data_mappings = {}
    boundaries_data_mappings['local_authority'] = {
        'data_file_path': boundaries_dir + '/scottish_local_authority.geojson',
        'data_set_keys':{
            'data_set_name': 'local_authority',
            'code':'lad18cd',
            'name':'lad18nm',
        }
    }
    boundaries_data_mappings['health_board'] = {
        'data_file_path': boundaries_dir + '/SG_NHS_HealthBoards_2019_clipped.geojson',
        'data_set_keys':{
            'data_set_name': 'health_board',
            'code':'HBCode',
            'name':'HBName',
        }
    }
    boundaries_data_mappings['health_integration_authority'] = {
        'data_file_path': boundaries_dir + '/SG_NHS_IntegrationAuthority_2019_clipped.geojson',
        'data_set_keys':{
            'data_set_name': 'health_integration_authority',
            'code':'HIACode',
            'name':'HIAName',
        }
    }
    return boundaries_data_mappings


def return_feature(service_area_type, service_area_code=0, all_features=False):
    boundaries_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), './data/boundaries'))
    dataset = {
        "0": {
            "data_path": boundaries_dir + '/Countries_December_2017_Ultra_Generalised_Clipped_Boundaries_in_UK.geojson',
            "code_key": "ctry17cd"
         },
        "1": {
            "data_path": None,
            "code_key": None
         },
        "2": {
            "data_path": boundaries_dir + '/scottish_local_authority.geojson',
            "code_key": "lad18cd"
         },
        "3": {
            "data_path": boundaries_dir + '/SG_NHS_HealthBoards_2019_clipped.geojson',
            "code_key": "HBCode"
        },
        "4": {
            "data_path": boundaries_dir + '/SG_NHS_IntegrationAuthority_2019_clipped.geojson',
            "code_key": "HIACode"
        }
    }
    if dataset[str(service_area_type)]["data_path"]:
        with open(dataset[str(service_area_type)]["data_path"])as f:
            js = json.load(f)
        return_feature = []
        for feature in js['features']:
            if all_features:
                return_feature.append(feature)
            elif feature['properties'][dataset[str(service_area_type)]["code_key"]] == service_area_code:
                return_feature = feature
        return return_feature
    else:
        return None
