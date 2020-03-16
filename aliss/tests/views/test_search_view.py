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

        self.s2 = Service.objects.create(name="My Testing Service", description="A testing service", organisation=o, created_by=t, updated_by=u)

        l = Fixtures.create_location(o)
        self.s2.locations.add(l)
        self.s2.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        self.s2.save()

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

        self.location_brechin = Location.objects.create(
            name="Brechin location", street_address="Brechin Street", locality="another locality",
            postal_code="DD9 6AD", latitude=56.73313937, longitude=-2.65779541,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.location_erskine= Location.objects.create(
            name="Erskine location", street_address="Erskine Street", locality="another locality",
            postal_code="PA8 7WZ", latitude=55.9054667, longitude=-4.45330031,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.location_dundee = Location.objects.create(
            name="Dundee location", street_address="Dundee Street", locality="another locality",
            postal_code="DD3 8EA", latitude=56.47774662, longitude=-2.98519045,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.location_glasgow_not_in_district = Location.objects.create(
            name="Glasgow not in district", street_address="Glasgow not in District Street", locality="another locality",
            postal_code="G1 1AB", latitude=55.860737, longitude=-4.244422,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.location_glasgow_in_district = Location.objects.create(
            name="Glasgow location", street_address="Glasgow Street", locality="another locality",
            postal_code="G2 1AA", latitude=55.86101, longitude=-4.24947,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.another_glasgow_in_district = Location.objects.create(
            name="Another Glasgow location", street_address="Another Glasgow Street", locality="another locality",
            postal_code="G2 1RY", latitude=55.861672, longitude=-4.252545,
            organisation=o, created_by=o.created_by, updated_by=o.updated_by
        )

        self.multi_location_service = Service.objects.create(name="Multi Location Service", description="A handy service", organisation=o, created_by=t, updated_by=u)

        self.multi_location_service.locations.add(self.location_brechin)
        self.multi_location_service.locations.add(self.location_erskine)
        self.multi_location_service.locations.add(self.location_dundee)
        self.multi_location_service.locations.add(self.location_glasgow_not_in_district)
        self.multi_location_service.save()

    def test_get(self):
        response = self.client.get('/search/?postcode=G2+4AA')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "My Testing Service")
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

    def test_brechin_lowercase_url(self):
        brechin_legacy_url = "/search/?q=&distance=10&latitude=56.73334200000001&longitude=-2.6552888999999595&location=brechin,+United+Kingdom&page=1"
        response = self.client.get(brechin_legacy_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "DD9 6AD")

    def test_brechin_uppercase_url(self):
        brechin_legacy_url = "/search/?q=&distance=10&latitude=56.73334200000001&longitude=-2.6552888999999595&location=BRECHIN,+United+Kingdom&page=1"
        response = self.client.get(brechin_legacy_url)
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "DD9 6AD")

    def test_no_results_new_organisation_search_button_with_keyword(self):
        response = self.client.get('/search/?postcode=G2+4AA&q=bork')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h2>Sorry, we couldn't find anything using those terms near G2 4AA.</h2>")
        self.assertContains(response, "New organisations search")

    def test_no_results_no_new_organisation_search_button_without_keyword(self):
        self.s2.delete()
        self.multi_location_service.delete()
        response = self.client.get('/search/?postcode=G2+4AA&q=')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h2>Sorry, we couldn't find anything using those terms near G2 4AA.</h2>")
        self.assertNotContains(response, "New organisations search")

    def test_no_results_new_organisation_search_button_with_keyword_correct_link(self):
        response = self.client.get('/search/?postcode=G2+4AA&q=bork')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h2>Sorry, we couldn't find anything using those terms near G2 4AA.</h2>")
        self.assertContains(response, "<a href=\"/organisations/search/?q=bork\" class=\"button primary\">New organisations search</a>")

    def test_no_results_new_organisation_search_button_with_keyword_redirect(self):
        response = self.client.get('/search/?postcode=G2+4AA&q=bork')
        self.assertEqual(response.status_code, 200)
        response_button_click = self.client.get('/organisations/search/?q=bork')
        self.assertEqual(response_button_click.status_code, 200)

    def test_more_locations_appears_when_no_district_match(self):
        response = self.client.get('/search/?postcode=G2+4AA&q=multi+location+service')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h3>Multi Location Service</h3>")
        self.assertContains(response, "<span class=\"first-location\">")
        self.assertContains(response, "<a class=\"more-link\" tabindex=\"0\">More Locations</a>")

    def test_more_locations_appears_when_one_district_match(self):
        self.multi_location_service.locations.add(self.location_glasgow_in_district)
        self.multi_location_service.save()
        response = self.client.get('/search/?postcode=G2+4AA&q=multi+location+service')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<span class=\"first-location\">")
        self.assertContains(response, "<a class=\"more-link\" tabindex=\"0\">More Locations</a>")

    def test_more_locations_appears_when_two_district_match(self):
        self.multi_location_service.locations.add(self.location_glasgow_in_district)
        self.multi_location_service.locations.add(self.another_glasgow_in_district)
        self.multi_location_service.save()
        response = self.client.get('/search/?postcode=G2+4AA&q=multi+location+service')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<span class=\"first-location\">")
        self.assertContains(response, "<a class=\"more-link\" tabindex=\"0\">More Locations</a>")

    def test_10km_radius_filter_returns_distance_score(self):
        response = self.client.get('/search/?postcode=G2+9ZZ&radius=10000')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "G2 9ZZ")
        self.assertContains(response, "My Testing Service")
        self.assertContains(response, "Nearest location 1.4km")

    def test_1km_radius_filter_returns_distance_score(self):
        response = self.client.get('/search/?postcode=G2+9ZZ&radius=1000')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "My Testing Service")
        self.assertNotContains(response, "Nearest location 1.4km")

    def test_placename_search_redirect(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="EH1 1BQ", defaults={
                'postcode': 'EH1 1BQ', 'postcode_district': 'EH1',
                'postcode_sector': 'EH1 1', 'latitude': 55.95263002,
                'longitude': -3.19132872, 'council_area_2011_code': 'S12000036',
                'health_board_area_2014_code': 'S08000024',
                'integration_authority_2016_code': 'S37000012',
                'place_name': 'Edinburgh', 'slug': 'edinburgh'
            }
        )
        postcode = Postcode.objects.get(slug='edinburgh')
        self.assertEqual(edinburgh_postcode[0], postcode)
        response = self.client.get('/search/?postcode=edinburgh+')
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, '/search/?postcode=EH1+1BQ&place_name=Edinburgh')

    def test__valid_placename_search_has_place_in_heading(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="EH1 1BQ", defaults={
                'postcode': 'EH1 1BQ', 'postcode_district': 'EH1',
                'postcode_sector': 'EH1 1', 'latitude': 55.95263002,
                'longitude': -3.19132872, 'council_area_2011_code': 'S12000036',
                'health_board_area_2014_code': 'S08000024',
                'integration_authority_2016_code': 'S37000012',
                'place_name': 'Edinburgh', 'slug': 'edinburgh'
            }
        )
        response = self.client.get('/search/?postcode=EH1+1BQ&place_name=Edinburgh')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Help and support in <span class=\"postcode\">Edinburgh</span><span class=\"assigned-categories\">(EH1 1BQ)</span></h1>", html=True)

    def test_postcode_district_search_valid(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="EH1 1BQ", defaults={
                'postcode': 'EH1 1BQ', 'postcode_district': 'EH1',
                'postcode_sector': 'EH1 1', 'latitude': 55.95263002,
                'longitude': -3.19132872, 'council_area_2011_code': 'S12000036',
                'health_board_area_2014_code': 'S08000024',
                'integration_authority_2016_code': 'S37000012',
                'place_name': 'Edinburgh', 'slug': 'edinburgh'
            }
        )
        response = self.client.get('/search/?postcode=EH1')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Help and support in <span class=\"postcode\">EH1 1BQ</span></h1>", html=True)

    def test_postcode_district_search_invalid(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="EH1 1BQ", defaults={
                'postcode': 'EH1 1BQ', 'postcode_district': 'EH1',
                'postcode_sector': 'EH1 1', 'latitude': 55.95263002,
                'longitude': -3.19132872, 'council_area_2011_code': 'S12000036',
                'health_board_area_2014_code': 'S08000024',
                'integration_authority_2016_code': 'S37000012',
                'place_name': 'Edinburgh', 'slug': 'edinburgh'
            }
        )
        response = self.client.get('/search/?postcode=AB4')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Sorry, AB4 doesn't appear to be a valid postcode.</h1>", html=True)

    def test_four_character_postcode_district_search_valid(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="AB10 6EJ", defaults={
                'postcode': 'AB10 6EJ', 'postcode_district': 'AB10',
                'postcode_sector': 'AB10 6', 'latitude': 57.13375256,
                'longitude': -2.11863086, 'council_area_2011_code': 'S12000033',
                'health_board_area_2014_code': 'S08000020',
                'integration_authority_2016_code': 'S37000001',
                'slug': 'ab10-6ej'
            }
        )
        response = self.client.get('/search/?postcode=AB10')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Help and support in <span class=\"postcode\">AB10 6EJ</span></h1>", html=True)

    def test_four_character_postcode_district_search_invalid(self):
        edinburgh_postcode = Postcode.objects.get_or_create(pk="AB10 6EJ", defaults={
                'postcode': 'AB10 6EJ', 'postcode_district': 'AB10',
                'postcode_sector': 'AB10 6', 'latitude': 57.13375256,
                'longitude': -2.11863086, 'council_area_2011_code': 'S12000033',
                'health_board_area_2014_code': 'S08000020',
                'integration_authority_2016_code': 'S37000001',
                'slug': 'ab10-6ej'
            }
        )
        response = self.client.get('/search/?postcode=AB20')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Sorry, AB20 doesn't appear to be a valid postcode.</h1>", html=True)

    def test_invalid_placename_search_error_page(self):
        response = self.client.get('/search/?postcode=Argyll ')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Sorry, Argyll couldn't be matched with a postcode.</h1>", html=True)

    def test_invalid_search_ALISS_not_available_error_page(self):
        response = self.client.get('/search/?postcode=Argyll Test')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Sorry, ALISS is not available in your postcode.</h1>", html=True)

    def test_invalid_postcode_error_page(self):
        response = self.client.get('/search/?postcode=G2 4ZZ')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "<h1>Sorry, G2 4ZZ doesn't appear to be a valid postcode.</h1>", html=True)

    def tearDown(self):
        Fixtures.organisation_teardown()
