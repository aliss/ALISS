from django.conf import settings

from elasticsearch import Elasticsearch
from elasticsearch.helpers import bulk
from elasticsearch_dsl import Q

from aliss.models import Organisation, Service
from search import _get_connection, service_to_body, service_mapping

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
    connection = _get_connection()
    services = Service.objects
    organisations = Organisation.objects

    if force:
        services = services.all()
        organisations = organisations.all()
    else:
        services = services.filter(slug=None).all()
        organisations = organisations.filter(slug=None).all()

    print("No. of service slugs to update: ", services.count())
    for s in services:
        s.generate_slug(force)
        s.save()

    print("No. of org slugs to update: ", organisations.count())
    for o in organisations:
        o.generate_slug(force)
        o.save()


def index_all():
    connection = _get_connection()
    services = Service.objects.all().iterator()
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