from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *
from aliss.search import index_service, delete_service

class v4SearchViewTestCase(TestCase):

    def setUp(self):
      self.service = Fixtures.create()
      index_service(self.service)
      self.client = Client()

    def test_get(self):
        response = self.client.get('/api/v4/services/', { 'postcode': 'G2 4AA' }, format="json")
        response2 = self.client.get('/api/v4/services/', { 'q': 'money', 'postcode': 'G2 4AA' }, format="json")

        self.assertEqual(response.status_code, 200)
        self.assertEqual(response2.status_code, 200)
        self.assertEqual(response['Content-Type'], 'application/json')
        self.assertEqual(response2['Content-Type'], 'application/json')
        self.assertTrue('count' in response.data)
        self.assertTrue('meta' in response.data)
        self.assertTrue('data' in response.data)
        self.assertTrue(response.data['count'] > 0)
        self.assertTrue(response.data['count'] > response2.data['count'])

    def tearDown(self):
        delete_service(self.service.pk)