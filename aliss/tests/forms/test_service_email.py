from django.test import TestCase, RequestFactory
from django.urls import reverse
from aliss.models import *
from aliss.forms import ServiceEmailForm
from aliss.tests.fixtures import Fixtures

class ServiceEmailFormTestCase(TestCase):
    fixtures = ['categories.json']

    def setUp(self):
        self.factory = RequestFactory()
        t,u,c,_ = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        self.service = Service.objects.create(name="My First Service", description="A handy service", organisation=self.org, created_by=t, updated_by=u)

    def test_form_is_valid_valid_email(self):
        request = self.factory.post(reverse('service_email'), { 'email': 'test@test.com', 'service': self.service.pk })
        form = ServiceEmailForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_invalid_invalid_email(self):
        request = self.factory.post(reverse('service_email'), { 'email': 'test@test', 'service': self.service.pk })
        form = ServiceEmailForm(request.POST)
        self.assertEqual(form.is_valid(), False)

    def test_form_is_invalid_blank_email(self):
        request = self.factory.post(reverse('service_email'), { 'email': '', 'service': self.service.pk })
        form = ServiceEmailForm(request.POST)
        self.assertEqual(form.is_valid(), False)

    def tearDown(self):
        for service in Service.objects.filter(name="My First Service"):
            service.delete()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
