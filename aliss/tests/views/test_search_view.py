from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class SearchViewTestCase(TestCase):
    fixtures = ['service_areas.json', 'g2_postcodes.json']

    def setUp(self):
        t, u, c, _ = Fixtures.create_users()
        o = Fixtures.create_organisation(t, u, c)
        s = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        s.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))

        brechin_postcode = Postcode.objects.create(
            postcode="DD9 6AD", postcode_district="DD9",  postcode_sector="DD3 8",
            latitude="56.73313937", longitude="-2.65779541",
            council_area_2011_code="S12000041",
            health_board_area_2014_code="S08000027",
            integration_authority_2016_code="S37000003")

        erskine_postcode = Postcode.objects.create(
            postcode="PA8 7WZ", postcode_district="PA8",  postcode_sector="PA8 7",
            latitude="55.9054667", longitude="-4.45330031",
            council_area_2011_code="S12000038",
            health_board_area_2014_code="S08000021",
            integration_authority_2016_code="S37000024")

        dundee_postcode = Postcode.objects.create(
            postcode="DD3 8EA", postcode_district="DD3",  postcode_sector="DD3 8",
            latitude="56.47774662", longitude="-2.98519045",
            council_area_2011_code="S12000042",
            health_board_area_2014_code="S08000027",
            integration_authority_2016_code="S37000007")

    def test_get(self):
        response = self.client.get('/search/?postcode=G2+4AA')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, 'Help and support in <span class="postcode">G2 4AA</span>')

    def test_invalid_postcode(self):
        response = self.client.get('/search/?postcode=ZZ+ZZZ')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, '<h1>Sorry, ALISS is not available in your postcode.</h1>')
        self.assertContains(response, "gtag('event', 'search-error-unrecognised', {")

    def test_no_postcode(self):
        response = self.client.get('/search/?postcode=AK1+5SA')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, '<h1>Sorry, AK1 5SA doesn\'t appear to be a valid postcode.</h1>')
        self.assertContains(response, "gtag('event', 'search-error-unrecognised', {")

    def test_invalid_get(self):
        fail_url = "/search/?lat=55.84279&page=5&lng=-4.4089&location=Paisley+East+and+Ralston,+Renfrewshire,+PA1+1SA"
        response = self.client.get(fail_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, '<h1>Sorry, ALISS is not available in your postcode.</h1>')
        self.assertContains(response, "gtag('event', 'search-error-unrecognised', {")

    def test_brechin_legacy_url(self):
        brechin_legacy_url = "/search/?q=&distance=10&latitude=56.73334200000001&longitude=-2.6552888999999595&location=Brechin,+United+Kingdom&page=1"
        response = self.client.get(brechin_legacy_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "DD9 6AD")

    def test_erskine_legacy_url(self):
        erskine_legacy_url = "/search/?q=&location=Erskine+PA8+7FG,+United+Kingdom&latitude=55.8907926&longitude=-4.444185400000038&distance=15"
        response = self.client.get(erskine_legacy_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "PA8 7WZ")


    def test_dundee_legacy_url(self):
        dundee_legacy_url = "/search/?q=health&accounts=179,91,268&lat=56.47347449999999&lng=-2.957288100000028&location=Dundee%20DD4%207AA,%20United%20Kingdom"
        response = self.client.get(dundee_legacy_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "DD3 8EA")
        self.assertContains(response, "health")

    def tearDown(self):
        Fixtures.organisation_teardown()
