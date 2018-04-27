from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location

class OrganisationViewTestCase(TestCase):
    def setUp(self):
        self.user = ALISSUser.objects.create_user("main@user.org", "passwurd")
        self.punter = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.claimant = ALISSUser.objects.create_user("claimant@random.org", "passwurd")
        self.client.login(username='main@user.org', password='passwurd')
        self.organisation = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=self.user,
          claimed_by=self.claimant
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

    def test_organisation_delete(self):
        response = self.client.delete((reverse('organisation_delete', kwargs={'pk': self.organisation.pk})))
        self.assertRedirects(response, (reverse('account_my_organisations')))

    def test_logout_organisation_create(self):
        self.client.logout()
        response = self.client.get(reverse('organisation_create'))
        self.assertEqual(response.status_code, 302)

    def test_unpublished_organisation_detail(self):
        self.organisation.published = False
        self.organisation.save()

        response_1 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.client.login(username='random@random.org', password='passwurd')
        response_2 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.client.login(username='claimant@random.org', password='passwurd')
        response_3 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))

        self.assertEqual(response_1.status_code, 200)
        self.assertEqual(response_2.status_code, 404)
        self.assertEqual(response_3.status_code, 200)