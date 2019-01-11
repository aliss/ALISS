from django.test import TestCase, Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location, Claim
from aliss.search import (get_service)


class AdminViewsTestCase(TestCase):
    def setUp(self):
        self.user, self.editor, self.claimant, self.staff = Fixtures.create_users()
        self.client.login(username='staff@aliss.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user, self.editor, self.claimant)

    def test_organisation_list(self):
        response = self.client.get(reverse('admin:aliss_organisation_changelist'))
        self.assertEqual(response.status_code, 200)

    def test_service_list(self):
        response = self.client.get(reverse('admin:aliss_service_changelist'))
        self.assertEqual(response.status_code, 200)

    def test_categories_list(self):
        response = self.client.get(reverse('admin:aliss_category_changelist'))
        self.assertEqual(response.status_code, 200)

    def test_service_areas_list(self):
        response = self.client.get(reverse('admin:aliss_servicearea_changelist'))
        self.assertEqual(response.status_code, 200)

    def test_users_list(self):
        response = self.client.get(reverse('admin:aliss_alissuser_changelist'))
        self.assertEqual(response.status_code, 200)

    def tearDown(self):
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
