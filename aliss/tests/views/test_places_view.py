from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class PlacesViewTestCase(TestCase):
    fixtures = ['categories.json','service_areas.json', ]

    def setUp(self):
        self.service = Fixtures.create()
        self.category = Category.objects.get(slug="conditions")
        self.service.categories.add(self.category)
        self.service.save()
        self.postcode = Postcode.objects.get(place_name='Glasgow')
        self.postcode.generate_place_name_slug()

    def test_valid_landing_page(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertEqual(self.service.categories.first().slug, self.category.slug)
        self.assertEqual(self.postcode.slug, 'glasgow')
        self.assertEqual(response.status_code, 200)

    def test_response_contains_blurb(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(response, "Help and support with Conditions in")
        self.assertContains(response, "Glasgow")

    def test_response_result(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(response, "My First Service")

    def test_response_with_error(self):
        response = self.client.get('/places/borkington/borks/')
        self.assertEqual(response.status_code, 404)
