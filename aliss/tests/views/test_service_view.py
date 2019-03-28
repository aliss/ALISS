from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location, Category, ServiceArea
from aliss.search import (get_service)
from datetime import datetime


class ServiceViewTestCase(TestCase):
    fixtures = ['categories.json', 'service_areas.json']

    def setUp(self):
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.client.login(username='random@random.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user)
        self.service = Fixtures.create_service(self.organisation)

    def test_service_detail(self):
        logged_in_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        logged_out_response = Client().get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(logged_in_response.status_code, 200)
        self.assertEqual(logged_out_response.status_code, 200)

    def test_unpublished_service_detail(self):
        self.organisation.published = False
        self.organisation.save()
        logged_in_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        logged_out_response = Client().get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(logged_in_response.status_code, 200)
        self.assertEqual(logged_out_response.status_code, 302)

    def test_service_create(self):
        x=reverse('service_create', kwargs={'pk':self.organisation.pk})
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)

    def test_logout_service_create(self):
        self.client.logout()
        response = self.client.get(reverse('service_create', kwargs={'pk':self.organisation.pk}))
        self.assertEqual(response.status_code, 302)

    def test_service_update_get(self):
        x=reverse('service_edit', kwargs={ 'pk': self.service.pk })
        response = self.client.get(x)
        self.assertEqual(response.status_code, 200)

    def test_service_valid_create(self):
        category = Category.objects.first()
        response = self.client.post(reverse('service_create', kwargs={'pk':self.organisation.pk}),{
            'name': 'A whole new service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })
        queryset = Service.objects.filter(name='A whole new service')
        self.assertEqual(queryset.count(), 1)
        self.assertEqual(response.status_code, 302)

    def test_service_last_edited_valid_create(self):
        category = Category.objects.first()
        response = self.client.post(reverse('service_create', kwargs={'pk':self.organisation.pk}),{
            'name': 'A whole new service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })
        s = Service.objects.get(name='A whole new service')
        last_edited = s.last_edited
        self.assertFalse(last_edited == None)


    def test_service_valid_update(self):
        category = Category.objects.first()
        response = self.client.post(reverse('service_edit', kwargs={ 'pk': self.service.pk }),{
            'name': 'an updated service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })

        self.service.refresh_from_db()
        queryset = Fixtures.es_connection()
        result = get_service(queryset, self.service.id)[0]
        category_count = len(result['categories'])

        self.assertEqual(category_count, 1)
        self.assertEqual(result['categories'][0]['name'], category.name)
        self.assertEqual(self.service.name, 'an updated service')
        self.assertEqual(self.service.slug, 'an-updated-service-0')
        self.assertEqual(self.service.updated_by, self.user)
        self.assertEqual(response.status_code, 302)

    def test_service_last_edited_valid_update(self):
        old_last_edited_db = self.service.last_edited
        category = Category.objects.first()
        response = self.client.post(reverse('service_edit', kwargs={ 'pk': self.service.pk }),{
            'name': 'an updated service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })
        self.service.refresh_from_db()
        queryset = Fixtures.es_connection()
        result = get_service(queryset, self.service.id)[0]
        new_last_edited_db = self.service.last_edited

        new_last_edited_db_string = datetime.strftime(new_last_edited_db, '%Y-%m-%dT%H:%M:%S.%f%z')
        new_last_edited_db_string = new_last_edited_db_string.split('+0000')[0] + '+00:00'
        new_last_edited_es = result.last_edited
        self.assertFalse(old_last_edited_db == new_last_edited_db)
        self.assertEqual(new_last_edited_db_string, new_last_edited_es)

    def test_redirect_to_org_confirm_single_service_valid_create(self):
        self.organisation.services.first().delete()
        self.assertEqual(self.organisation.services.count(), 0)
        category = Category.objects.first()
        response = self.client.post(reverse('service_create', kwargs={'pk':self.organisation.pk}),{
            'name': 'A whole new service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })
        queryset = Service.objects.filter(name='A whole new service')
        self.assertEqual(queryset.count(), 1)
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, reverse('organisation_confirm',kwargs={'pk': self.organisation.pk}
        ))

    def test_redirect_to_org_detail_second_service_valid_create(self):
        self.assertEqual(self.organisation.services.count(), 1)
        category = Category.objects.first()
        response = self.client.post(reverse('service_create', kwargs={'pk':self.organisation.pk}),{
            'name': 'A whole new service',
            'description': 'a full description',
            'categories': [category.pk],
            'service_areas': [ServiceArea.objects.first().pk]
        })
        queryset = Service.objects.filter(name='A whole new service')
        self.assertEqual(queryset.count(), 1)
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, reverse('organisation_detail',kwargs={'pk': self.organisation.pk}
        ))

    def tearDown(self):
        Fixtures.service_teardown()
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
