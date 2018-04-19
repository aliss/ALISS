from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location

class OrganisationViewTestCase(TestCase):
    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')
        self.organisation = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=self.user
        )

    def test_organisation_detail(self):
        response = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    def test_organisation_edit(self):
        response = self.client.get(reverse('organisation_edit', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    def test_organisation_create(self):
        response = self.client.get(reverse('organisation_create'))
        self.assertEqual(response.status_code, 200)

    def test_logout_organisation_create(self):
        self.client.logout()
        response = self.client.get(reverse('organisation_create'))
        self.assertEqual(response.status_code, 302)