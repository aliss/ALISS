from aliss.models import *
from django.test import TestCase
from django.conf import settings
from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

class Fixtures(TestCase):

    @classmethod
    def es_connection(self):
        connections.create_connection(hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        return Search(index='search', doc_type='service').params(preference='_local')

    @classmethod
    def es_organisation_connection(self):
        connections.create_connection(hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        return Search(index='organisation_search', doc_type='organisation').params(preference='_local')

    @classmethod
    def create_users(self):
        t = ALISSUser.objects.create_user("tester@aliss.org", "passwurd", name="Mr Test")
        u = ALISSUser.objects.create_user("updater@aliss.org", "passwurd", name="Mr Updater", is_editor=True)
        c = ALISSUser.objects.create_user("claimant@user.org", "passwurd")
        s = ALISSUser.objects.create_user("staff@aliss.org", "passwurd", is_staff=True, is_superuser=True)
        return [t,u,c,s]

    @classmethod
    def create_organisation(self, creator, updater=None, claimant=None):
        if updater == None:
            updater = creator
        return Organisation.objects.create(
            name="TestOrg",
            description="A test description",
            created_by=creator, updated_by=updater, claimed_by=claimant
        )

    @classmethod
    def create_location(self, o):
        return Location.objects.create(
            name="my location", street_address="my street", locality="a locality",
            postal_code="G2 4AA", latitude=55.86529182, longitude=-4.2684418,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

    @classmethod
    def create_another_location(self, o):
        return Location.objects.create(
            name="my other location", street_address="my other street", locality="another locality",
            postal_code="G2 9ZZ", latitude=55.86104946, longitude=-4.24736892,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

    @classmethod
    def create_service(self, o):
        l = Fixtures.create_location(o)
        s = Service.objects.create(
            name="My First Service",
            description="A handy service",
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )
        s.locations.add(l)
        s.save()
        return s

    @classmethod
    def create_properties(self):
        p1 = Property.objects.create(name="Interpretation",
            description="Translation and interpretation for languages other than English",
            for_organisations=False, for_locations=False
        )
        p2 = Property.objects.create(name="Wheelchair accessible",
            description="Support for visitors with wheelchairs",
            for_organisations=False, for_services=False
        )
        p3 = Property.objects.create(name="Volunteer run",
            description="Entirely staffed by volunteers",
        )
        p4 = Property.objects.create(name="Living wage employer",
            description="All our employees are paid a living wage",
            for_locations=False, for_services=False
        )
        return [p1,p2,p3,p4]

    @classmethod
    def properties_teardown(self):
        Property.objects.all().delete()

    @classmethod
    def service_teardown(self, service=None):
        if service:
            service.delete()
        else:
            Service.objects.filter(name="My First Service").delete()

    @classmethod
    def organisation_teardown(self, org=None):
        if org:
            org.delete()
        else:
            for org in Organisation.objects.filter(name="TestOrg").all():
                org.delete()

    @classmethod
    def general_teardown(self):
        Fixtures.service_teardown()
        Fixtures.organisation_teardown()
        Fixtures.properties_teardown()

    @classmethod
    def create(self):
        Postcode.objects.get_or_create(pk="G2 4AA", defaults={
                'postcode': 'G2 4AA', 'postcode_district': 'G2',
                'postcode_sector': 'G2 4', 'latitude': 55.86523763,
                'longitude': -4.26974322, 'council_area_2011_code': 'S12000046',
                'health_board_area_2014_code': 'S08000021',
                'integration_authority_2016_code': 'S37000015',
                'place_name': 'Glasgow', 'slug': 'glasgow'
            }
        )
        Postcode.objects.get_or_create(pk="EH1 1BQ", defaults={
                'postcode': 'EH1 1BQ', 'postcode_district': 'EH1',
                'postcode_sector': 'EH1 1', 'latitude': 55.95263002,
                'longitude': -3.19132872, 'council_area_2011_code': 'S12000036',
                'health_board_area_2014_code': 'S08000024',
                'integration_authority_2016_code': 'S37000012',
                'place_name': 'Edinburgh', 'slug': 'edinburgh'
            }
        )
        t,u,c,_ = Fixtures.create_users()
        o = Fixtures.create_organisation(t,u,c)
        s = Fixtures.create_service(o)
        return s
