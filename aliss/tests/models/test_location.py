from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Location
from aliss.tests.fixtures import Fixtures

class LocationTestCase(TestCase):
    def setUp(self):
        t,u,c,s = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        l = Location.objects.create(
          name="my location", street_address="my street", locality="a locality",
          postal_code="FK1 5XA", latitude=50.0, longitude=13.0,
          organisation=self.org, created_by=t, updated_by=u
        )

    def test_location_exists(self):
        l = Location.objects.get(name="my location", postal_code="FK1 5XA")
        self.assertTrue(isinstance(l, Location))

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        ALISSUser.objects.get(email="claimant@user.org").delete()
        self.test_location_exists()

    def test_org_delete_cascades(self):
        Organisation.objects.get(name="TestOrg").delete()
        l = Location.objects.filter(name="my location", postal_code="FK1 5XA").exists()
        self.assertFalse(l)

    def test_is_edited_by(self):
        l = Location.objects.get(name="my location", postal_code="FK1 5XA")
        rep    = ALISSUser.objects.get(email="claimant@user.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        staff  = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
        self.assertTrue(l.is_edited_by(rep))
        self.assertTrue(l.is_edited_by(staff))
        self.assertFalse(l.is_edited_by(editor))
        self.assertFalse(l.is_edited_by(l.organisation.created_by))
        self.assertFalse(l.is_edited_by(punter))

    def test_is_edited_by_without_claimant(self):
        l = Location.objects.get(name="my location", postal_code="FK1 5XA")
        self.org.claimed_by = None; self.org.save()
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        staff  = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
        self.assertTrue(l.is_edited_by(staff))
        self.assertTrue(l.is_edited_by(editor))
        self.assertTrue(l.is_edited_by(l.organisation.created_by))
        self.assertFalse(l.is_edited_by(punter))

    def tearDown(self):
        for organisation in Organisation.objects.filter(name="TestOrg"):
            organisation.delete()
