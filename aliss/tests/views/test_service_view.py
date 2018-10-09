from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location, Category, ServiceArea
from aliss.search import (get_service)

class ServiceViewTestCase(TestCase):
    fixtures = ['categories.json', 'service_areas.json']

    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user)

    def test_service_create(self):
        x=reverse('service_create', kwargs={'pk':self.organisation.pk})
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)

    def test_logout_service_create(self):
        self.client.logout()
        response = self.client.get(reverse('service_create', kwargs={'pk':self.organisation.pk}))
        self.assertEqual(response.status_code, 302)

    def test_service_update_get(self):
        service = Fixtures.create_service(self.organisation)
        x=reverse('service_edit', kwargs={ 'pk': service.pk })
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)

    def test_service_valid_update(self):
        service = Fixtures.create_service(self.organisation)
        category = Category.objects.first()
        response = self.client.post(reverse('service_edit', kwargs={ 'pk': service.pk }),{
            'name': 'an updated service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })

        service.refresh_from_db()
        queryset = Fixtures.es_connection()
        result = get_service(queryset, service.id)[0]
        category_count = len(result['categories'])

        self.assertEqual(category_count, 1)
        self.assertEqual(result['categories'][0]['name'], category.name)
        self.assertEqual(service.name, 'an updated service')
        self.assertEqual(service.updated_by, self.user)
        self.assertEqual(response.status_code, 302)
