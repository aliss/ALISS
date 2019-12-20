from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location, Category
from aliss.tests.fixtures import Fixtures
from aliss.search import (get_service)
from django.db.models import Count
from datetime import datetime, timedelta
import pytz


class ServiceTestCase(TestCase):
    fixtures = ['categories.json']

    def setUp(self):
        t,u,c,_ = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        self.service = Service.objects.create(name="My First Service", description="A handy service", organisation=self.org, created_by=t, updated_by=u)

    def test_service_exists(self):
        s = Service.objects.get(name="My First Service")
        self.assertTrue(isinstance(s, Service))

    def test_service_slugs(self):
        s1 = Service.objects.get(name="My First Service")
        s2 = Service.objects.create(name="My First Service", description="A handy service", organisation=s1.organisation, created_by=s1.created_by)
        self.assertEqual(s1.slug, "my-first-service")
        self.assertTrue("my-first-service-" in s2.slug)
        s1.name = "My Other Service"
        s1.save()
        self.assertEqual(s1.slug, "my-other-service")
        s3 = Service.objects.create(name="My First Service", description="Another handy service", organisation=s1.organisation, created_by=s1.created_by)
        duplicates = Service.objects.values('slug').annotate(Count('id')).order_by().filter(id__count__gt=1)
        self.assertEqual(0, duplicates.count())

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

    def test_unpublished_service_is_not_indexed(self):
        queryset = Fixtures.es_connection()
        self.org.published = False
        self.org.save()
        unpublished_service = Service.objects.create(name="My Second Service", description="A handy service", organisation=self.org, created_by=self.org.created_by)
        result = get_service(queryset, unpublished_service.id)
        self.assertEqual(len(result), 0)

    def test_deleted_service_is_not_in_index(self):
        queryset = Fixtures.es_connection()
        service_id = self.service.id
        self.service.delete()
        result = get_service(queryset, service_id)
        self.assertEqual(len(result), 0)

    def test_is_edited_by_without_claimant(self):
        o = Fixtures.create_organisation(self.org.created_by, self.org.created_by)
        s = Service.objects.create(name="My Other Service", description="A handy service", organisation=o, created_by=o.created_by, updated_by=o.created_by)
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        staff  = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
        self.assertTrue(s.is_edited_by(staff))
        self.assertTrue(s.is_edited_by(s.organisation.created_by))
        self.assertTrue(s.is_edited_by(editor))
        self.assertFalse(s.is_edited_by(punter))

    def test_is_edited_by(self):
        s = Service.objects.get(name="My First Service")
        rep    = ALISSUser.objects.get(email="claimant@user.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        staff  = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
        self.assertEqual(rep, s.organisation.claimed_by)
        self.assertTrue(s.is_edited_by(staff))
        self.assertTrue(s.is_edited_by(rep))
        self.assertFalse(s.is_edited_by(s.organisation.created_by))
        self.assertFalse(s.is_edited_by(editor))
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

    def test_service_last_edited_exists(self):
        last_edited = self.service.last_edited
        self.assertFalse(last_edited == None)

    def test_service_last_edited_persists(self):
        old_updated_date = self.service.updated_on
        old_last_edited = self.service.last_edited
        c = Category.objects.create(name='Children', slug='children')
        self.service.categories.add(c)
        self.service.save()
        new_last_edited = self.service.last_edited
        new_updated_date = self.service.updated_on
        self.assertEqual(old_last_edited, new_last_edited)
        self.assertFalse(old_updated_date == new_updated_date)

    def test_service_last_edited_update_method(self):
        old_last_edited = self.service.last_edited
        self.service.update_last_edited()
        new_last_edited = self.service.last_edited
        self.assertFalse(old_last_edited == new_last_edited)

    def test_check_service_last_reviewed_outwith_date(self):
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        out_of_range_date = (current_date - timedelta(weeks=7))
        self.service.last_edited = out_of_range_date
        self.service.save()
        last_reviewed_status = self.service.check_last_reviewed()
        service_id = self.service.pk
        self.assertEqual(last_reviewed_status, service_id)

    def test_check_service_last_reviewed_within_date(self):
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        in_range_date = (current_date - timedelta(weeks=5))
        self.service.last_edited = in_range_date
        self.service.save()
        last_reviewed_status = self.service.check_last_reviewed()
        service_id = self.service.pk
        self.assertEqual(last_reviewed_status, None)

    def tearDown(self):
        for service in Service.objects.filter(name="My First Service"):
            service.delete()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
