from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location, ServiceArea, Postcode

class SearchViewTestCase(TestCase):
    fixtures = ['service_areas.json', 'g2_postcodes.json']

    def setUp(self):
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        o = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=t, updated_by=u
        )
        s = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        s.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))

    def test_no_postcode(self):
        response = self.client.get('/search/?postcode=ZZ+ZZZ')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, '<h1>Sorry, ZZ ZZZ doesn\'t appear to be a valid postcode.</h1>')

    def test_get(self):
        response = self.client.get('/search/?postcode=G2+4AA')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, 'Help &amp; support in <span class="postcode">G2 4AA</span>')