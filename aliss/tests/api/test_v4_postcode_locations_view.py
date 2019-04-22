from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class v4PostcodeLocationDataViewTestCase(TestCase):

    def setUp(self):
      self.service = Fixtures.create()
      self.client = Client()

    def test_get(self):
        response = self.client.get('/api/v4/postcode-locations/', { 'q': 'Gla' }, format="json")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response['Content-Type'], 'application/json')
        # print(response.content)


    def tearDown(self):
        self.service.delete()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
