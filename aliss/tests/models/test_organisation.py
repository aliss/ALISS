from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location
from aliss.tests.fixtures import Fixtures
from aliss.search import (get_service, get_organisation_by_id, filter_organisation_by_query)

class OrganisationTestCase(TestCase):
    def setUp(self):
        t,u,c,s = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        self.service = Fixtures.create_service(self.org)

    def test_org_exists(self):
        o = Organisation.objects.get(name="TestOrg")
        self.assertTrue(isinstance(o, Organisation))

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        ALISSUser.objects.get(email="claimant@user.org").delete()
        self.test_org_exists()

    def test_is_edited_by(self):
        o = Organisation.objects.get(name="TestOrg")
        rep    = ALISSUser.objects.get(email="claimant@user.org")
        staff  = ALISSUser.objects.get(email="staff@aliss.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        self.assertTrue(o.is_edited_by(o.created_by))
        self.assertTrue(o.is_edited_by(staff))
        self.assertTrue(o.is_edited_by(editor))
        self.assertTrue(o.is_edited_by(rep))
        self.assertFalse(o.is_edited_by(punter))

    def test_rename_is_reflected_in_index(self):
        self.org.name = "Renamed Test Org"
        self.org.save()
        queryset = Fixtures.es_connection()
        indexed_service = get_service(queryset, self.service.id)[0]
        self.assertEqual(indexed_service['organisation']['name'], self.org.name)

    def test_organisation_last_edited_exists(self):
        last_edited = self.org.last_edited
        self.assertFalse(last_edited == None)

    def test_organisation_last_edited_persists(self):
        old_last_edited = self.org.last_edited
        old_updated_date = self.org.updated_on
        self.org.name = "Renamed Test Org"
        self.org.save()
        self.org.name = "Renamed Renamed Test Org"
        self.org.save()
        new_last_edited = self.org.last_edited
        new_updated_date = self.org.updated_on
        self.assertFalse(old_updated_date == new_updated_date)
        self.assertEqual(old_last_edited, new_last_edited)

    def test_organisation_last_edited_update_method(self):
        old_last_edited = self.org.last_edited
        self.org.update_organisation_last_edited()
        new_last_edited = self.org.last_edited
        self.assertFalse(old_last_edited == new_last_edited)

    def test_organisation_added_to_index(self):
        queryset = Fixtures.es_organisation_connection()
        indexed_organisation = get_organisation_by_id(queryset, self.org.id)[0]
        self.assertEqual(indexed_organisation.id, str(self.org.id))
        self.assertEqual(indexed_organisation.name, self.org.name)
        self.assertEqual(indexed_organisation.description, self.org.description)
        self.assertEqual(indexed_organisation.published, self.org.published)

    def test_organisation_update_reindexes(self):
        queryset = Fixtures.es_organisation_connection()
        self.org.name = "Test Org"
        self.org.save()
        result = get_organisation_by_id(queryset, self.org.id)[0]
        self.assertEqual(result['name'], self.org.name)

    def test_organisation_filter_organisation_by_query(self):
        queryset = Fixtures.es_organisation_connection()
        self.org.name = "banana weird"
        self.org.save()
        result = filter_organisation_by_query(queryset, "banana").execute()
        self.assertEqual(result[0].name, self.org.name)

    def tearDown(self):
        for service in Service.objects.filter(name="My First Service"):
            service.delete()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
        for organisation in Organisation.objects.filter(name="Test Org"):
            organisation.delete()
        for organisation in Organisation.objects.filter(name="Renamed Test Org"):
            organisation.delete()
        for organisation in Organisation.objects.filter(name="Renamed Renamed Test Org"):
            organisation.delete()
        for organisation in Organisation.objects.filter(name="banana weird"):
            organisation.delete()
