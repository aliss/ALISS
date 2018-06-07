from django.test import TestCase
from django.test import Client
from django.urls import reverse

class v3SearchViewTestCase(TestCase):

    def test_get(self):
        response = self.client.get('/search/?postcode=G2+4AA')
        self.assertEqual(response.status_code, 200)