from django.test import TestCase, Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location, Claim
from aliss.search import (get_service)


class OrganisationViewTestCase(TestCase):
    def setUp(self):
        self.random = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.user, self.editor, self.claimant, self.staff = Fixtures.create_users()
        self.client.login(username=self.user.email, password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user, self.editor, self.claimant)

    def test_organisation_detail(self):
        response = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)
        response = self.client.get(reverse('organisation_detail_slug', kwargs={'slug': self.organisation.slug}))
        self.assertEqual(response.status_code, 200)

    def test_organisation_edit(self):
        response = self.client.get(reverse('organisation_edit', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    def test_organisation_create(self):
        response = self.client.get(reverse('organisation_create'))
        self.assertEqual(response.status_code, 200)

    def test_organisation_can_add_logo(self):
        response_1 = self.client.get(reverse('organisation_create'))
        self.client.login(username=self.editor.email, password='passwurd')
        response_2 = self.client.get(reverse('organisation_create'))
        self.client.login(username=self.claimant.email, password='passwurd')
        response_3 = self.client.get(reverse('organisation_edit', kwargs={'pk': self.organisation.pk}))
        self.assertNotContains(response_1, '<label for="id_logo">Logo</label>', html=True)
        self.assertContains(response_2, '<label for="id_logo">Logo</label>', html=True)
        self.assertContains(response_3, '<label for="id_logo">Logo</label>', html=True)

    def test_organisation_confirm(self):
        response = self.client.get(reverse('organisation_confirm', kwargs={'pk': self.organisation.pk}))
        self.assertEqual(response.status_code, 200)

    def test_organisation_invalid_creation(self):
        response = self.client.post(reverse('organisation_create'), { 'name': '' })
        self.assertEqual(Organisation.objects.count(), 1)
        self.assertEqual(response.status_code, 200)

    def test_organisation_valid_creation(self):
        cn = Claim.objects.count()
        response = self.client.post(reverse('organisation_create'),
            { 'name': 'an organisation', 'description': 'a full description' })
        o = Organisation.objects.latest('created_on')

        self.assertEqual(cn, Claim.objects.count())
        self.assertEqual(o.name, 'an organisation')
        self.assertEqual(o.published, False)
        self.assertEqual(response.status_code, 302)

    def test_organisation_valid_creation_with_claim(self):
        response = self.client.post(reverse('organisation_create'), {
            'name': 'an organisation', 'description': 'a full description',
            'claim': 'on', 'claim-comment': 'im important', 'claim-phone': '034343243',
            'claim-data_quality': 'on'
        })

        o = Organisation.objects.latest('created_on')
        c = Claim.objects.latest('created_on')
        self.assertEqual(c.organisation, o)
        self.assertEqual(o.name, 'an organisation')
        self.assertEqual(o.published, False)
        self.assertEqual(response.status_code, 302)

    def test_organisation_invalid_creation_with_claim(self):
        on = Organisation.objects.count()
        cn = Claim.objects.count()
        response = self.client.post(reverse('organisation_create'), {
            'name': 'an organisation', 'description': 'a full description',
            'claim': 'on', 'claim-comment': '', 'claim-phone': '',
            'claim-data_quality': 'on'
        })
        self.assertEqual(on, Organisation.objects.count())
        self.assertEqual(cn, Claim.objects.count())
        self.assertEqual(response.status_code, 200)

    def test_organisation_valid_creation_with_editor(self):
        self.client.login(username='updater@aliss.org', password='passwurd')
        response = self.client.post(reverse('organisation_create'),
            { 'name': 'an organisation', 'description': 'a full description' })
        o = Organisation.objects.latest('created_on')

        self.assertEqual(o.name, 'an organisation')
        self.assertEqual(o.published, True)
        self.assertEqual(response.status_code, 302)

    def test_logout_organisation_create(self):
        self.client.logout()
        response = self.client.get(reverse('organisation_create'))
        self.assertEqual(response.status_code, 302)

    def test_organisation_valid_creation_last_edited(self):
        self.client.login(username='updater@aliss.org', password='passwurd')
        response = self.client.post(reverse('organisation_create'),
            { 'name': 'an organisation', 'description': 'a full description' })
        o = Organisation.objects.latest('created_on')
        last_edited = o.last_edited
        self.assertFalse(last_edited == None)

    def test_organisation_valid_update(self):
        response = self.client.post(reverse('organisation_edit', kwargs={'pk': self.organisation.pk}),
            { 'name': 'an updated organisation', 'description': 'a full description' })
        self.organisation.refresh_from_db()
        self.assertEqual(self.organisation.name, 'an updated organisation')
        self.assertEqual(response.status_code, 302)

    def test_last_edited_valid_update(self):
        old_last_edited = self.organisation.last_edited
        response = self.client.post(reverse('organisation_edit', kwargs={'pk': self.organisation.pk}),
            { 'name': 'an updated organisation', 'description': 'a full description' })
        self.organisation.refresh_from_db()
        new_last_edited = self.organisation.last_edited
        self.assertFalse(old_last_edited == new_last_edited)

    def test_unpublished_organisation_detail(self):
        self.organisation.published = False
        self.organisation.save()

        response_1 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.client.login(username='random@random.org', password='passwurd')
        response_2 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))
        self.client.login(username='claimant@user.org', password='passwurd')
        response_3 = self.client.get(reverse('organisation_detail', kwargs={'pk': self.organisation.pk}))

        self.assertEqual(response_1.status_code, 200)
        self.assertEqual(response_2.status_code, 403)
        self.assertEqual(response_3.status_code, 200)

    def test_organisation_delete(self):
        response = self.client.delete((reverse('organisation_delete', kwargs={'pk': self.organisation.pk})))
        self.assertRedirects(response, (reverse('account_my_organisations')))

    def test_organisation_unpublished(self):
        response = self.client.get(reverse('organisation_unpublished'))
        self.assertEqual(response.status_code, 302)
        self.client.login(username='staff@aliss.org', password='passwurd')
        response = self.client.get(reverse('organisation_unpublished'))
        self.assertEqual(response.status_code, 200)

    def test_organisation_publish(self):
        queryset = Fixtures.es_connection()
        self.organisation.published = False
        self.organisation.save()
        unpublished_service = Fixtures.create_service(self.organisation)
        index_result = get_service(queryset, unpublished_service.id)
        self.client.login(username='staff@aliss.org', password='passwurd')
        self.assertEqual(len(index_result), 0)

        response = self.client.post(reverse('organisation_publish', kwargs={'pk': self.organisation.pk}))
        self.organisation.refresh_from_db()
        index_result = get_service(queryset, unpublished_service.id)
        self.assertEqual(len(index_result), 0)
        self.assertTrue(self.organisation.published)
        self.assertEqual(response.status_code, 302)
        self.assertRedirects(response, (reverse('organisation_unpublished')))

    def test_organisation_search(self):
        response = self.client.get(reverse('organisation_search')+'?q=TestOrg')
        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "TestOrg")
