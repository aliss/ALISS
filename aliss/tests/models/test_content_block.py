from django.conf import settings
from django.test import TestCase
from aliss.models import *

class ContentBlockTestCase(TestCase):

    def test_can_create_content_block(self):
        body_content = "<h1>Edinburgh Custom Block Content</h2><p>New content block for testing.</p>"
        c = ContentBlock.objects.create(slug='places-edinburgh', body=body_content)
        self.assertTrue(isinstance(c,ContentBlock))

    def test_can_get_content_block(self):
        body_content = "<h1>Edinburgh Custom Block Content</h2><p>New content block for testing.</p>"
        c = ContentBlock.objects.create(slug='places-edinburgh', body=body_content)
        self.assertTrue(isinstance(c,ContentBlock))
        content_block = ContentBlock.objects.get(slug='places-edinburgh')
        self.assertEqual(c, content_block)

    def test_can_delete_content_block(self):
        body_content = "<h1>Edinburgh Custom Block Content</h2><p>New content block for testing.</p>"
        c = ContentBlock.objects.create(slug='places-edinburgh', body=body_content)
        self.assertTrue(isinstance(c,ContentBlock))
        ContentBlock.objects.get(slug='places-edinburgh').delete()
        self.assertTrue(ContentBlock.objects.all().count() < 1)

    def tearDown(self):
        for block in ContentBlock.objects.all():
            block.delete()
