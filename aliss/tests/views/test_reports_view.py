from django.test import TestCase
from django.test import Client
from django.urls import reverse


class ReportsViewTestCase(TestCase):

    def test_get(self):
        response = self.client.get('/reports')
        self.assertEqual(response.status_code, 200)