from django.test import TestCase, Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *
from aliss.api.views import TrackUsageMixin


class TrackUsageMixinTestCase(TestCase):

    def setUp(self):
        Fixtures.create_users()
        self.client = Client()

    def test_get(self):
        response = self.client.get("/api/v3/search/", { 'postcode': 'G2 4AA' }, format="json")
        request = response.wsgi_request
        analyics_payload = TrackUsageMixin.build_request(request)
        self.assertTrue('tid=' in analyics_payload)
        self.assertTrue('ec=api' in analyics_payload)
        self.assertTrue('ea=/api/v3/search/,postcode=G2+4AA' in analyics_payload)
        self.assertTrue('cid=' in analyics_payload)

    def test_get_logged_in(self):
        self.client.login(username="tester@aliss.org", password="passwurd")
        response = self.client.get("/api/v3/search/", { 'postcode': 'G2 4AA' }, format="json")
        request = response.wsgi_request
        analyics_payload = TrackUsageMixin.build_request(request)
        self.assertTrue('cid=' not in analyics_payload)
        self.assertTrue('uid=' in analyics_payload)