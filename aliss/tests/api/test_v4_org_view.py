from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class v4OrganisationDetailViewTestCase(TestCase):

    def setUp(self):
      self.service = Fixtures.create()
      self.organisation = self.service.organisation
      self.client = Client()

    def test_get(self):
        path = '/api/v4/organisations/' + str(self.organisation.pk) + '/'
        response = self.client.get(path, format="json")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response['Content-Type'], 'application/json')
        self.assertTrue('name' in response.data)

    def tearDown(self):
        self.service.delete()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
