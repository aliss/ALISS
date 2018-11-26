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

        # # Necessary to create services and digests to allow the email generation method to be tested.
        # test_postcode = Postcode.objects.get(pk="G2 4AA")
        # conditions_category = Category.objects.get(slug="conditions")
        # housing_category = Category.objects.get(slug="housing-and-homelessness")
        #
        # t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        # u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        # c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        # o = Organisation.objects.create(
        #   name="TestOrg",
        #   description="A test description",
        #   created_by=t,
        #   updated_by=u,
        #   claimed_by=c
        # )
        #
        # l = Location.objects.create(
        #   name="My location", street_address="my street", locality="a locality",
        #   postal_code="G2 4AA", latitude=55.865192, longitude=-4.266868,
        #   organisation=o, created_by=t, updated_by=u
        # )
        #
        # # Setup service with category of Conditions
        # service1 = Service.objects.create(name="My Conditions Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        # service1.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        # service1.categories.add(conditions_category)
        # service1.locations.add(l)
        # index_service(service1)
        # service1.save()
        #
        # # Create conditions digest for the user (should have results)
        # conditions_digest = DigestSelection.objects.create(postcode=test_postcode, category=conditions_category, user=self.user)
        #
        # # Create housing digest (should have no updated services message)
        # housing_digest = DigestSelection.objects.create(postcode=test_postcode, category=housing_category, user=self.user)

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
