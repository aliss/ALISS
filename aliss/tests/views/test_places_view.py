from django.test import TestCase
from django.test import Client
from django.urls import reverse

from aliss.tests.fixtures import Fixtures
from aliss.models import *

# class PlacesViewTestCase(TestCase):
fixtures = ['categories.json', 'service_areas.json', ]

def setUp(self):
        self.service = Fixtures.create()
        self.category = Category.objects.get(slug="conditions")
        self.service.categories.add(self.category)
        self.service.save()
        self.postcode = Postcode.objects.get(place_name='Glasgow')
        self.postcode.generate_place_name_slug()

def test_valid_landing_page_place_category(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertEqual(self.service.categories.first().slug,
                         self.category.slug)
        self.assertEqual(self.postcode.slug, 'glasgow')
        self.assertEqual(response.status_code, 200)

def test_response_contains_blurb(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(response, "Help and support with Conditions in")
        self.assertContains(response, "Glasgow")

def test_response_result(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(response, "My First Service")

def test_response_with_error_status_code(self):
        response = self.client.get('/places/borkington/borks/')
        self.assertEqual(response.status_code, 200)

def test_response_with_error_status_code(self):
        response = self.client.get('/places/borkington/borks/')
        self.assertContains(
          response, "<h1>Sorry, borkington or borks doesn't appear to be a valid search.</h1>")

def test_valid_search_filter_link(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(
          response, "/search/?category=conditions&amp;postcode=G2+4AA")

def test_valid_search_category_link(self):
        response = self.client.get('/places/glasgow/conditions/')
        self.assertContains(
          response, "/search/?category=goods&amp;postcode=G2+4AA")

def test_valid_landing_page_place_content_block(self):
        custom_content = "<h1>Edinburgh title test</h1><p>New landing page content for a place.</p>"
        ContentBlock.objects.create(
          slug='places-edinburgh', body=custom_content)
        response = self.client.get('/places/edinburgh/')
        self.assertContains(response, custom_content)

def test_valid_landing_page_title_content_block(self):
        custom_content = "Landing page for Edinburgh"
        ContentBlock.objects.create(
          slug='places-edinburgh', body="<p>New landing page content for a place.</p>")
        ContentBlock.objects.create(
          slug='places-edinburgh-title', body=custom_content)
        response = self.client.get('/places/edinburgh/')
        self.assertContains(response, custom_content)

def test_valid_landing_page_meta_content_blocks(self):
        custom_meta = "meta description for edinburgh"
        custom_title = "meta title for edinburgh"
        ContentBlock.objects.create(
          slug='places-edinburgh', body="<p>New landing page content for a place.</p>")
        ContentBlock.objects.create(
          slug='places-edinburgh-meta-description', body=custom_meta)
        ContentBlock.objects.create(
          slug='places-edinburgh-meta-title', body=custom_title)
        response = self.client.get('/places/edinburgh/')
        self.assertContains(response, custom_meta)
        self.assertContains(response, custom_title)

def test_valid_redirect_no_custom_content_when_placename_exists(self):
        response = self.client.get('/places/glasgow/')
        self.assertEqual(response.status_code, 302)
        #self.assertRedirects(response, reverse('search', kwargs={'postcode': 'G2 4AA'}))

def test_valid_redirect_no_custom_content_no_placename(self):
        response = self.client.get('/places/musselburgh/')
        self.assertEqual(response.status_code, 404)

def test_valid_landing_page_place_category_with_content(self):
        custom_content = "<p>Conditions services for Glasgow</p>"
        custom_title = "Glasgow: conditions and wellbeing services - ALISS"
        custom_meta_title = "Glasgow conditions / wellbeing services"
        custom_meta_desc = "Find services on ALISS"
        ContentBlock.objects.create(
          slug='places-glasgow-conditions', body=custom_content)
        ContentBlock.objects.create(
          slug='places-glasgow-conditions-title', body=custom_title)
        ContentBlock.objects.create(
          slug='places-glasgow-conditions-meta-title', body=custom_meta_title)
        ContentBlock.objects.create(
          slug='places-glasgow-conditions-meta-description', body=custom_meta_desc)
        response = self.client.get('/places/glasgow/conditions/')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, custom_content)
        self.assertContains(response, custom_title)
        self.assertContains(response, custom_meta_desc)
        self.assertContains(response, custom_meta_title)

def test_valid_landing_page_uppercase_placename(self):
        custom_content = "<h1>Edinburgh title test</h1><p>New landing page content for a place.</p>"
        ContentBlock.objects.create(
          slug='places-edinburgh', body=custom_content)
        response = self.client.get('/places/Edinburgh/')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, custom_content)

def test_invalid_landing_page_uppercase_valid_placename_redirect_search(self):
        place_name_slug = self.postcode.slug
        response = self.client.get('/places/Glasgow/')
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, '/search/?postcode=G2+4AA')

def test_valid_landing_page_uppercase_placename_uppercase_category(self):
        custom_content = "<p>Conditions services for Glasgow</p>"
        ContentBlock.objects.create(
          slug='places-glasgow-conditions', body=custom_content)
        response = self.client.get('/places/Glasgow/Conditions/')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, custom_content)

def test_invalid_landing_page_uppercase_placename_uppercase_category_standard_copy(self):
        response = self.client.get('/places/Glasgow/Conditions/')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, 'Glasgow')
        self.assertContains(response, 'Conditions')

def tearDown(self):
        for block in ContentBlock.objects.all():
            block.delete()
