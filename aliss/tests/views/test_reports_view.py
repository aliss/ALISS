from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location


class ReportsViewTestCase(TestCase):
    def setUp(self):
        self.staff   = ALISSUser.objects.create_user("staff@aliss.org", "passwurd", is_staff=True)
        self.client.login(username='staff@aliss.org', password='passwurd')

    def test_get(self):
        response = self.client.get('/reports/')
        self.assertEqual(response.status_code, 200)