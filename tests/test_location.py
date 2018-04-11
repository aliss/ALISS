from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Location

class LocationTestCase(TestCase):
    def setUp(self):
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org")
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg", description="A test description",
          created_by=t, updated_by=u, claimed_by=c
        )
        l = Location.objects.create(name="my location", 
          street_address="my street", locality="a locality", 
          postal_code="FK1 5XA", latitude=50.0, longitude=13.0,
          organisation=o, created_by=t, updated_by=u)

    def test_location_exists(self):
        l = Location.objects.get(name="my location", postal_code="FK1 5XA")
        self.assertTrue(isinstance(l, Location))

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        ALISSUser.objects.get(email="claimant@aliss.org").delete()
        self.test_location_exists()

    def test_org_delete_cascades(self):
        Organisation.objects.get(name="TestOrg").delete()
        l = Location.objects.filter(name="my location", postal_code="FK1 5XA").exists()
        self.assertFalse(l)