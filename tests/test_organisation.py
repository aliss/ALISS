from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location

class OrganisationTestCase(TestCase):
    def setUp(self):
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org")
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=t,
          updated_by=u,
          claimed_by=c
        )

    def test_org_exists(self):
        o = Organisation.objects.get(name="TestOrg")
        self.assertTrue(isinstance(o, Organisation))

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        ALISSUser.objects.get(email="claimant@aliss.org").delete()
        self.test_org_exists()