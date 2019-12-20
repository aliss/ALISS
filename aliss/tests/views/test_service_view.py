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
        self.user, self.editor, self.claimant, self.staff = Fixtures.create_users()
        self.organisation = Fixtures.create_organisation(self.user, self.editor, self.claimant)
        self.service = Fixtures.create_service(self.organisation)
        self.non_edit_user = ALISSUser.objects.create_user("nonEdit@nonEdit.org", "passwurd")
        self.client.login(username=self.claimant.email, password='passwurd')


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
        create_path=reverse('service_create', kwargs={'pk':self.organisation.pk})
        response = self.client.get(create_path)
        self.assertEqual(response.status_code, 200)


    def test_logout_service_create(self):
        self.client.logout()
        response = self.client.get(reverse('service_create', kwargs={'pk':self.organisation.pk}))
        self.assertEqual(response.status_code, 302)


    def test_service_edit(self):
        edit_path = reverse('service_edit', kwargs={ 'pk': self.service.pk })
        self.assertEqual(self.client.get(edit_path).status_code, 200)
        self.client.login(username=self.staff.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 200)
        self.client.login(username=self.user.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 302)
        self.client.login(username=self.editor.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 302)


    def test_service_edit_without_claimant(self):
        new_org = Fixtures.create_organisation(self.organisation.created_by, self.organisation.created_by)
        new_service = Fixtures.create_service(new_org)
        edit_path = reverse('service_edit', kwargs={ 'pk': new_service.pk })
        self.assertEqual(self.client.get(edit_path).status_code, 302)
        self.client.login(username=self.user.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 200)
        self.client.login(username=self.editor.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 200)
        self.client.login(username=self.staff.email, password='passwurd')
        self.assertEqual(self.client.get(edit_path).status_code, 200)


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
        self.assertEqual(self.service.slug, 'an-updated-service')
        self.assertEqual(self.service.updated_by, self.claimant)
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


    def test_editor_sees_edit_service_action(self):
        editor_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(editor_response.status_code, 200)
        self.assertContains(editor_response, "My First Service")
        self.assertContains(editor_response, "Edit service")


    def test_editor_sees_delete_service_action(self):
        editor_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(editor_response.status_code, 200)
        self.assertContains(editor_response, "My First Service")
        self.assertContains(editor_response, "Delete service")


    def test_non_editor_doesnt_see_edit_service_action(self):
        self.client.logout()
        non_editor_client = self.client.login(username='nonEdit@nonEdit.com', password='passwurd')
        non_editor_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(non_editor_response.status_code, 200)
        self.assertContains(non_editor_response, "My First Service")
        self.assertNotContains(non_editor_response, "Edit service")


    def test_non_editor_doesnt_see_delete_service_action(self):
        self.client.logout()
        non_editor_client = self.client.login(username='nonEdit@nonEdit.com', password='passwurd')
        non_editor_response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        self.assertEqual(non_editor_response.status_code, 200)
        self.assertContains(non_editor_response, "My First Service")
        self.assertNotContains(non_editor_response, "Delete service")


    def test_service_at_location_delete(self):
        location_count = self.service.locations.count()
        self.assertEqual(1, location_count)
        location_pk = self.service.locations.first().pk
        service_pk = self.service.pk
        service_at_location_slug = str(service_pk) + ':' + str(location_pk)
        response = self.client.post(reverse('service_at_location_delete', kwargs={'service_at_location_pk':service_at_location_slug}))
        self.assertEqual(response.status_code, 302)
        new_location_count = self.service.locations.count()
        self.assertEqual(0, new_location_count)


    def test_service_at_location_delete_non_editor(self):
        self.client.logout()
        non_editor_client = self.client.login(username='nonEdit@nonEdit.com', password='passwurd')
        location_count = self.service.locations.count()
        self.assertEqual(1, location_count)
        location_pk = self.service.locations.first().pk
        service_pk = self.service.pk
        service_at_location_slug = str(service_pk) + ':' + str(location_pk)
        non_editor_response = self.client.post(reverse('service_at_location_delete', kwargs={'service_at_location_pk':service_at_location_slug}))
        self.assertEqual(non_editor_response.status_code, 302)
        new_location_count = self.service.locations.count()
        self.assertEqual(1, new_location_count)


    def test_service_detail_location_lat_longs_context(self):
        locations = self.service.locations.all()
        comparison_lat_long_dict = {}
        for location in locations:
            street_address = location.street_address
            lat_long = [location.latitude, location.longitude]
            comparison_lat_long_dict[street_address] = lat_long
        response = self.client.get(reverse('service_detail_slug', kwargs={'slug':self.service.slug}))
        context_lat_longs_dict = response.context['location_lat_longs']
        self.assertEqual(comparison_lat_long_dict, context_lat_longs_dict)


    def test_service_detail_slug_embedded_map(self):
        response = self.client.get(reverse('service_detail_slug_map', kwargs={'slug':self.service.slug}))
        self.assertEqual(response.status_code, 200)


    def test_service_detail_pk_embedded_map(self):
        response = self.client.get(reverse('service_detail_map', kwargs={'pk':self.service.pk}))
        self.assertEqual(response.status_code, 200)


    def tearDown(self):
        Fixtures.organisation_teardown()
        Fixtures.service_teardown()
