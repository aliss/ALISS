from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class v4SearchViewTestCase(TestCase):

    def setUp(self):
      Fixtures.create()
      self.client = Client()

    def test_get(self):
        response = self.client.get('/api/v4/services/', { 'postcode': 'G2 4AA' }, format="json")
        self.assertEqual(response.status_code, 200)