from django.test import TestCase, Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import Organisation, ALISSUser, Service, Location
from aliss.filters import OrganisationFilter

class OrganisationFilterTestCase(TestCase):
    def setUp(self):
        self.user, self.editor, self.claimant, self.staff = Fixtures.create_users()
        self.client.login(username='tester@aliss.org', password='passwurd')
        self.organisation = Fixtures.create_organisation(self.user, self.editor, self.claimant)
        self.org2 = Organisation.objects.create(
          name="Another's Test Org",
          description="A test description",
          created_by=self.user,
          published=True
        )

    def test_basic_query(self):
        f = OrganisationFilter({ "q": "test" })
        exists = f.qs.filter(pk=self.organisation.pk).exists()
        self.assertEqual(exists, True)
        self.assertEqual(f.qs.count(), 2)

    def test_stripping_query(self):
        f = OrganisationFilter({ "q": "tes't" })
        exists = f.qs.filter(pk=self.organisation.pk).exists()
        self.assertEqual(exists, True)
        self.assertEqual(f.qs.count(), 2)

    def test_apostrophe_query(self):
        f = OrganisationFilter({ "q": "another\'s test" })
        exists = f.qs.filter(pk=self.org2.pk).exists()
        self.assertEqual(exists, True)
        self.assertEqual(f.qs.count(), 1)

    def test_no_apostrophe_query(self):
        f = OrganisationFilter({ "q": "anothers test" })
        exists = f.qs.filter(pk=self.org2.pk).exists()
        self.assertEqual(exists, True)
        self.assertEqual(f.qs.count(), 1)

    def test_fail_query(self):
        f = OrganisationFilter({ "q": "test\'s test" })
        self.assertEqual(f.qs.count(), 0)

