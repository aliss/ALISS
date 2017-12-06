from django.conf import settings

from elasticsearch import Elasticsearch
from elasticsearch.helpers import bulk
from elasticsearch_dsl import Q

from aliss.models import Organisation, Service


def _get_connection():
    import certifi
    return Elasticsearch([settings.ELASTICSEARCH_URL], http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))


organisation_mapping = {
    'id': {'type': 'keyword'},
    'name': {'type': 'text'},
    'description': {
        'type': 'text',
        'analyzer': 'description_analyzer',
    },
    'url': {'type': 'keyword'},
    'phone': {'type': 'keyword'},
    'email': {'type': 'keyword'},
    'facebook': {'type': 'keyword'},
    'twitter': {'type': 'keyword'}
}


service_mapping = {
    'id': {'type': 'keyword'},
    'organisation': {
        'properties': {
            'id': {'type': 'keyword'},
            'name': {'type': 'text'}
        }
    },
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
                'organisation': {
                    'properties': organisation_mapping
                },
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


def index_all():
    connection = _get_connection()

    organisations = Organisation.objects.all().iterator()
    # Index Organisations
    for ok in bulk(connection, ({
        '_index': 'search',
        '_type': 'organisation',
        '_id': organisation.pk,
        '_source': organisation_to_body(organisation)
    } for organisation in organisations)):
        print("%s Organisations indexed" % ok)

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


def organisation_to_body(organisation):
    return {
        'id': str(organisation.id),
        'name': organisation.name,
        'description': organisation.description,
        'url': organisation.url,
        'email': organisation.email,
        'phone': organisation.phone,
        'facebook': organisation.facebook,
        'twitter': organisation.twitter
    }


def service_to_body(service):
    return {
        'id': str(service.id),
        'organisation': {
            'id': service.organisation.pk,
            'name': service.organisation.name,
        },
        'name': service.name,
        'description': service.description,
        'url': service.url,
        'email': service.email,
        'phone': service.phone,
        'categories': [{
            'id': category.pk,
            'name': category.name,
            'slug': category.slug
        } for category in service.categories.all()],
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
            'name': service_area.name,
        } for service_area in service.service_areas.all()]
    }


def index_organisation(object):
    connection = _get_connection()
    body = organisation_to_body(object)

    connection.index(
        index='search',
        doc_type='organisation',
        id=body['id'],
        body=body,
        refresh=True
    )


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


def delete_organisation(id):
    connection = _get_connection()
    connection.delete(
        index='search',
        doc_type='organisation',
        id=id,
        refresh=True,
        ignore=404
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
    return queryset.query({
        "bool": {
            "should": [
                { "match": {
                    "categories.name": q
                }},
                {"multi_match": {
                    "query": q,
                    "fields": ["name", "description"]
                }}
            ],
            "minimum_should_match": 1
        }
    })


def filter_by_postcode(queryset, postcode, radius=5000):
    queryset = queryset.filter(
        Q('terms', service_areas__code=postcode.codes) |
        Q('geo_distance', distance="{0}m".format(radius), locations__point={
            "lat": postcode.latitude,
            "lon": postcode.longitude
        })
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


def filter_by_categories(queryset, categories):
    return queryset.filter(
        Q('terms', categories__id=[category.pk for category in categories])
    )

def filter_by_service_areas(queryset, service_areas):
    return queryset.filter(
        Q('terms', service_areas__id=[
            service_area.pk for service_area in service_areas
        ])
    )
