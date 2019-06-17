from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location

class AccountViewTestCase(TestCase):
    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')

    def test_saved_services(self):
        response = self.client.get(reverse('account_saved_services'))
        self.assertEqual(response.status_code, 200)

    def test_logout_saved_services(self):
        self.client.logout()
        response = self.client.get(reverse('account_saved_services'))
        self.assertEqual(response.status_code, 302)

    def test_account_valid_creation(self):
        response = self.client.post(reverse('signup'),
            { 'name': 'Chef', 'accept_terms_and_conditions': True,
            'email': 'Bork@bork.com', 'password1': 'passwurd', 'password2': 'passwurd' })
        u = ALISSUser.objects.latest('date_joined')
        self.assertEqual(u.email, 'bork@bork.com')
        self.assertEqual(response.status_code, 302)