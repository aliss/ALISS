from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class v4ServiceDetailViewTestCase(TestCase):

    def setUp(self):
      self.service = Fixtures.create()
      self.client = Client()

    def test_get(self):
        path = '/api/v4/services/' + str(self.service.pk) + '/'
        response = self.client.get(path, format="json")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response['Content-Type'], 'application/json')
        self.assertTrue('id' in response.data['data'])
        self.assertTrue('name' in response.data['data'])
        self.assertTrue('description' in response.data['data'])
        self.assertTrue('categories' in response.data['data'])
        self.assertTrue('service_areas' in response.data['data'])
        self.assertTrue('locations' in response.data['data'])
        self.assertTrue('formatted_address' in response.data['data']['locations'][0])

    def test_slug(self):
        path = '/api/v4/services/' + str(self.service.slug) + '/'
        response = self.client.get(path, format="json")
        self.assertEqual(response.status_code, 200)
        self.assertTrue('id' in response.data['data'])

    def tearDown(self):
        Fixtures.organisation_teardown()
