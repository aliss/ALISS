from django.test import TestCase, Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location, Claim

class ClaimViewTestCase(TestCase):
    def setUp(self):
        self.user     = ALISSUser.objects.create_user("main@user.org", "passwurd")
        self.punter   = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.staff   = ALISSUser.objects.create_user("staff@aliss.org", "passwurd",  is_staff=True)
        self.editor   = ALISSUser.objects.create_user("updater@aliss.org", "passwurd",  is_editor=True)
        self.client.login(username='main@user.org', password='passwurd')
        self.organisation = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=self.punter
        )

    def test_claim_list(self):
        response = self.client.get(reverse('claim_list', kwargs={}))
        self.assertEqual(response.status_code, 302)
        self.client.login(username='staff@aliss.org', password='passwurd')
        response = self.client.get(reverse('claim_list', kwargs={}))
        self.assertEqual(response.status_code, 200)

    def test_claim_create(self):
        response = self.client.get(reverse('claim_create', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    def test_logout_claim_create(self):
        self.client.logout()
        response = self.client.get(reverse('claim_create', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 302)