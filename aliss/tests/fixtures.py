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
    def create(self):
        if Postcode.objects.filter(pk="G2 4AA").count() == 0:
            Postcode.objects.create(
                postcode="G2 4AA", postcode_district="G2",  postcode_sector="4",
                latitude="55.86529182", longitude="-4.2684418",
                council_area_2011_code="S12000046",
                health_board_area_2014_code="S08000021",
                integration_authority_2016_code="S37000015",
                place_name="Glasgow"
            )
        t,u,c,_ = Fixtures.create_users()
        o = Fixtures.create_organisation(t,u,c)
        s = Fixtures.create_service(o)
        return s
