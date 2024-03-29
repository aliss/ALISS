from django.test import TestCase, Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location, Claim

class ClaimViewTestCase(TestCase):
    def setUp(self):
        self.tester, _, self.claimant, self.staff = Fixtures.create_users()
        self.client.login(username='claimant@user.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.tester)

    def test_claim_list(self):
        response = self.client.get(reverse('claim_list', kwargs={}))
        self.assertEqual(response.status_code, 302)
        self.client.login(username='staff@aliss.org', password='passwurd')
        response = self.client.get(reverse('claim_list', kwargs={}))
        self.assertEqual(response.status_code, 200)

    def test_claim_create_path(self):
        response = self.client.get(reverse('claim_create', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    # def test_valid_claim_create(self):
    #     cn = Claim.objects.count()
    #     response = self.client.post(reverse('claim_create', kwargs={'pk': self.organisation.pk}),
    #         { 'comment': 'im important', 'phone': "034343243", 'data_quality': 'on' })
    #     self.assertEqual(response.status_code, 302)
    #     self.assertEqual((cn+1), Claim.objects.count())

    def test_logout_claim_create(self):
        self.client.logout()
        response = self.client.get(reverse('claim_create', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 302)

    def tearDown(self):
        Fixtures.organisation_teardown()
