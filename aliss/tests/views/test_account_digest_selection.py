from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location

class AccountDigestSelectionViewTestCase(TestCase):
    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')

    def test_digest_selection(self):
        response = self.client.get(reverse('account_my_digest'))
        self.assertEqual(response.status_code, 200)

    def test_create_digest_selection(self):
        response = self.client.get(reverse('account_create_my_digest'))
        self.assertEqual(response.status_code, 200)

    def test_logout_digest_selection(self):
        self.client.logout()
        response = self.client.get(reverse('account_my_digest'))
        self.assertEqual(response.status_code, 302)

    def test_logout_create_digest_selection(self):
        self.client.logout()
        response = self.client.get(reverse('account_create_my_digest'))
        self.assertEqual(response.status_code, 302)
