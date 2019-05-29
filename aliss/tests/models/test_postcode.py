from django.test import TestCase
from aliss.models import *
from aliss.tests.fixtures import Fixtures

class PostcodeTestCase(TestCase):

    def setUp(self):
        Fixtures.create()
        self.postcode = Postcode.objects.get(place_name="Glasgow")

    def test_postcode_exists(self):
        self.assertTrue(isinstance(self.postcode, Postcode))

    def test_slug_exists(self):
        self.postcode.generate_place_name_slug()
        slug = self.postcode.slug
        self.assertEqual(slug, 'glasgow')
