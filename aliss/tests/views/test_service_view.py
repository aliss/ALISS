from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location

class ServiceViewTestCase(TestCase):
    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user)

    def test_service_create(self):
        x=reverse('service_create', kwargs={'pk':self.organisation.pk})
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)

    def test_logout_service_create(self):
        self.client.logout()
        response = self.client.get(reverse('service_create', kwargs={'pk':self.organisation.pk}))
        self.assertEqual(response.status_code, 302)