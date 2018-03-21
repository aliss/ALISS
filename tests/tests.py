from django.test import TestCase
from aliss.models import Organisation, ALISSUser

class OrganisationTestCase(TestCase):
    def setUp(self):
        u = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        Organisation.objects.create(
          name="TestOrg", 
          description="A test description",
          created_by=u
        )

    def test_org_created(self):
        o = Organisation.objects.get(name="TestOrg")
        self.assertTrue(isinstance(o, Organisation))