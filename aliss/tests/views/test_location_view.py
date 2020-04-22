from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location
import mock
import geopy

class LocationViewTestCase(TestCase):
    def setUp(self):
        self.service = Fixtures.create()
        self.organisation = self.service.organisation
        self.location = self.organisation.locations.first()
        self.client.login(username="claimant@user.org", password="passwurd")


    def test_location_edit(self):
        path=reverse('location_edit', kwargs={'pk':self.location.pk})
        response = self.client.get(path)
        self.assertEqual(response.status_code, 200)


    def test_location_update(self):
        with mock.patch('aliss.forms.LocationForm.geocode_address') as mock_geocode_address:
            mock_geocode_address.return_value = geopy.location.Location(
                "Newkirkgate, Leith, Edinburgh, City of Edinburgh, Scotland, EH6 6AB, United Kingdom",
                (55.9714304, -3.1715182)
            )
            path=reverse('location_edit', kwargs={'pk':self.location.pk})
            response = self.client.post(path,
                { 'name': 'Leith Community Education Centre', 'street_address': '12A Newkirkgate',
                  'locality': 'Edinburgh', 'postal_code': 'EH6 6AD' })

            self.location.refresh_from_db()
            self.assertEqual(self.location.name, 'Leith Community Education Centre')
            self.assertTrue(mock_geocode_address.called)
            self.assertEqual(self.location.latitude, 55.9714304)
            self.assertEqual(self.location.longitude, -3.1715182)
            self.assertEqual(response.status_code, 302)


    def test_location_create(self):
        x=reverse('location_create', kwargs={'pk':self.organisation.pk})
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)


    def test_logout_location_create(self):
        self.client.logout()
        response = self.client.get(reverse('location_create', kwargs={'pk':self.organisation.pk}))
        self.assertEqual(response.status_code, 302)


    def tearDown(self):
        Fixtures.organisation_teardown()
