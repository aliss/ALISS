from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location, Category
from aliss.tests.fixtures import Fixtures
from aliss.search import (get_service)


class CategoryTestCase(TestCase):
    fixtures = ['categories.json']

    def setUp(self):
        t,u,c,_ = Fixtures.create_users()
        o = Fixtures.create_organisation(t, u, c)
        self.service = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)

    def test_service_has_renamed_category(self):
        c = Category.objects.get(slug='food-nutrition')
        self.service.categories.add(c)
        self.service.save()
        c.name = "food and nutrition"
        c.save()

        queryset = Fixtures.es_connection()
        indexed_service = get_service(queryset, self.service.id)[0]

        is_categorised = c.services.filter(id=self.service.id).exists()
        is_reindexed = c.name == indexed_service['categories'][0]['name']

        self.assertTrue(is_categorised)
        self.assertTrue(is_reindexed)

    def test_service_not_in_removed_category(self):
        queryset = Fixtures.es_connection()
        c = Category.objects.get(slug='food-nutrition')
        self.service.categories.add(c)
        self.service.save()
        indexed_service = get_service(queryset, self.service.id)[0]
        self.assertEqual(len(indexed_service['categories']), 1)
        c.delete()
        indexed_service = get_service(queryset, self.service.id)[0]
        self.assertEqual(len(indexed_service['categories']), 0)