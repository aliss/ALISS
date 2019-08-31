from django.test import TestCase, RequestFactory, Client
from aliss.tests.fixtures import Fixtures
from django.template import Context, Template
from django.urls import reverse

class AlissTemplateTagTest(TestCase):
    def setUp(self):
        self.factory = RequestFactory()
        self.template_to_render = Template(
            '{% load aliss %}'
            '{% service_area_tip key="1" as following_service_area %}'
            '{% if following_service_area %}'
            '<h3>service area warning</h3>'
            '{% else %}'
            '<h3>no service area warning</h3>'
            '{% endif %}'
        )

    def test_area_warning_shows_in_default_search(self):
        request = self.factory.get(reverse('search'), { "q": "" })
        context = Context({
            'request': request,
            'distance_scores': {'1': 10.0, '2': None}
        })
        rendered_template = self.template_to_render.render(context)
        self.assertInHTML('<h3>service area warning</h3>', rendered_template)

    def test_area_warning_false_in_keyword_search(self):
        request = self.factory.get(reverse('search'), { "q": "smoking" })
        context = Context({
            'request': request,
            'distance_scores': {'1': 10.0, '2': None}
        })
        rendered_template = self.template_to_render.render(context)
        self.assertInHTML('<h3>no service area warning</h3>', rendered_template)

    def test_area_warning_show_in_nearest_search(self):
        request = self.factory.get(reverse('search'), { "q": "smoking", "sort": "nearest" })
        context = Context({
            'request': request,
            'distance_scores': {'1': 10.0, '2': None}
        })
        rendered_template = self.template_to_render.render(context)
        self.assertInHTML('<h3>service area warning</h3>', rendered_template)
