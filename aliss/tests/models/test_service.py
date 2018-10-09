from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location, Category
from aliss.tests.fixtures import Fixtures
from aliss.search import (get_service)


class ServiceTestCase(TestCase):
    fixtures = ['categories.json']

    def setUp(self):
        t,u,c,_ = Fixtures.create_users()
        o = Fixtures.create_organisation(t, u, c)
        self.service = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)

    def test_service_exists(self):
        s = Service.objects.get(name="My First Service")
        self.assertTrue(isinstance(s, Service))

    def test_service_slugs(self):
        s1 = Service.objects.get(name="My First Service")
        s2 = Service.objects.create(name="My First Service", description="A handy service", organisation=s1.organisation, created_by=s1.created_by)
        self.assertEqual(s1.slug, "my-first-service-0")
        self.assertEqual(s2.slug, "my-first-service-1")

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        self.test_service_exists()

    def test_org_delete_cascades(self):
        queryset = Fixtures.es_connection()
        service_id = self.service.id
        Organisation.objects.get(name="TestOrg").delete()
        s = Service.objects.filter(name="My First Service").exists()
        self.assertFalse(s)
        result = get_service(queryset, service_id)
        self.assertEqual(len(result), 0)

    def test_new_service_is_indexed(self):
        queryset = Fixtures.es_connection()
        result = get_service(queryset, self.service.id)
        self.assertEqual(len(result), 1)

    def test_deleted_service_is_not_in_index(self):
        queryset = Fixtures.es_connection()
        service_id = self.service.id
        self.service.delete()
        result = get_service(queryset, service_id)
        self.assertEqual(len(result), 0)

    def test_is_edited_by(self):
        s = Service.objects.get(name="My First Service")
        rep    = ALISSUser.objects.get(email="claimant@user.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        staff  = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
        self.assertTrue(s.is_edited_by(s.organisation.created_by))
        self.assertTrue(s.is_edited_by(staff))
        self.assertTrue(s.is_edited_by(editor))
        self.assertTrue(s.is_edited_by(rep))
        self.assertFalse(s.is_edited_by(punter))

    def test_service_update_reindexes(self):
        queryset = Fixtures.es_connection()
        self.service.name = "My Updated Service"
        self.service.save()
        result = get_service(queryset, self.service.id)[0]
        self.assertEqual(result['name'], self.service.name)

    def test_adding_category_triggers_reindex(self):
        c = Category.objects.create(name='Children', slug='children')
        self.service.categories.add(c)
        self.service.save()
        queryset = Fixtures.es_connection()
        result = get_service(queryset, self.service.id)[0]
        self.assertEqual(result['categories'][0]['name'], 'Children')

    def test_service_exists(self):
        s = Service.objects.get(name="My First Service")
        s.save(kwargs={'skip_index': True})
        self.assertTrue(isinstance(s, Service))
