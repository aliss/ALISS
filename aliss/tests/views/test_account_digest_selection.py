from django.conf import settings
from django.test import TestCase
from django.test import Client
from django.urls import reverse
from elasticsearch_dsl import Search
from aliss.models import *
from aliss.search import *
from django.utils import timezone
from datetime import datetime
from datetime import timedelta
import pytz

class AccountDigestSelectionViewTestCase(TestCase):

    fixtures = ['service_areas.json', 'g2_postcodes.json', 'categories.json']

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

    def test_digest_valid_creation(self):
        user_digests = self.user.digest_selections.all().count()
        self.assertEqual(0, user_digests)
        response = self.client.post(reverse('account_create_my_digest'), {'postcode': 'G2 4AA', 'category': 'conditions'})
        self.assertEqual(self.user.digest_selections.all().count(), 1)
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, (reverse('account_my_digest')))

    def test_digest_invalid_postcode_creation(self):
        user_digests = self.user.digest_selections.all().count()
        self.assertEqual(0, user_digests)
        response = self.client.post(reverse('account_create_my_digest'), {'postcode': 'G2 !AA', 'category': 'conditions'})
        self.assertEqual(self.user.digest_selections.all().count(), user_digests)
        self.assertEqual(response.status_code, 200)

    def test_digest_invalid_category_creation(self):
        user_digests = self.user.digest_selections.all().count()
        self.assertEqual(0, user_digests)
        response = self.client.post(reverse('account_create_my_digest'), {'postcode': 'G2 4AA', 'category': 'go0ds'})
        self.assertEqual(self.user.digest_selections.all().count(), user_digests)
        self.assertEqual(response.status_code, 200)

    def test_digest_delete_digest(self):
        self.client.post(reverse('account_create_my_digest'), {'postcode': 'G2 4AA', 'category': 'conditions'})
        self.assertEqual(self.user.digest_selections.all().count(), 1)
        user_digest = self.user.digest_selections.first()
        response = self.client.delete((reverse('account_my_digest_delete', kwargs={'pk': user_digest.pk})))
        self.assertEqual(response.status_code, 302)
        self.assertEqual(self.user.digest_selections.all().count(), 0)
        self.assertRedirects(response, (reverse('account_my_digest')))