from django.conf import settings

from elasticsearch import Elasticsearch
from elasticsearch.helpers import bulk
from elasticsearch_dsl import Q

from aliss.models import Organisation, Service, ServiceArea


def _get_connection():
    import certifi
    return Elasticsearch([settings.ELASTICSEARCH_URL], http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))

service_mapping = {
    'id': {'type': 'keyword'},
    'organisation': {
        'properties': {
            'id': {'type': 'keyword'},
            'name': {'type': 'text'},
            'slug': {'type': 'keyword'}
        }
    },
    'slug': {'type': 'keyword'},
    'name': {'type': 'text'},
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


def create_index():
    connection = _get_connection()
    connection.indices.create(
        index='search',
        body={
            'mappings': {
                'service': {
                    'properties': service_mapping
                }
            },
            'settings': {
                'analysis': {
                    'analyzer': {
                        'description_analyzer': {
                            'type': 'custom',
                            'tokenizer': 'standard',
                            'char_filter': ['html_strip'],
                            'filter': ['standard', 'lowercase', 'stop']
                        }
                    }
                }
            }
        }
    )


def create_slugs(force=False):
    for s in Service.objects.all():
        s.generate_slug(force)
        s.save()
        print("Slug saved: ", s.slug)
    for o in Organisation.objects.all():
        o.generate_slug(force)
        o.save()
        print("Slug saved: ", o.slug)

def index_all():
    connection = _get_connection()

    services = Service.objects.all().iterator()
    # Index Services
    for ok in bulk(connection, ({
        '_index': 'search',
        '_type': 'service',
        '_id': service.pk,
        '_source': service_to_body(service)
    } for service in services)):
        print("%s Services indexed" % ok)


def delete_index():
    connection = _get_connection()
    connection.indices.delete('search', ignore=404)


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
        'updated_on': service.updated_on,
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


def index_service(object):
    connection = _get_connection()
    body = service_to_body(object)

    connection.index(
        index='search',
        doc_type='service',
        id=body['id'],
        body=body,
        refresh=True
    )


def delete_service(id):
    connection = _get_connection()
    connection.delete(
        index='search',
        doc_type='service',
        id=id,
        refresh=True,
        ignore=404
    )


def filter_by_query(queryset, q):
    queryset = queryset.query({
        "multi_match" : {
            "query" : q,
            "type": "most_fields",
            "fields" : ["categories.name", "name", "description"]
        }
    })

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
        }
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
