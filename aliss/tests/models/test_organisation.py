from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location

class OrganisationTestCase(TestCase):
    def setUp(self):
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        s = ALISSUser.objects.create(name="Ms Staff", email="msstaff@aliss.org", is_staff=True)
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

    def test_is_edited_by(self):
        o = Organisation.objects.get(name="TestOrg")
        rep    = ALISSUser.objects.get(email="claimant@aliss.org")
        staff  = ALISSUser.objects.get(email="msstaff@aliss.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        self.assertTrue(o.is_edited_by(o.created_by))
        self.assertTrue(o.is_edited_by(staff))
        self.assertTrue(o.is_edited_by(editor))
        self.assertTrue(o.is_edited_by(rep))
        self.assertFalse(o.is_edited_by(punter))