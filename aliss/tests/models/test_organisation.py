from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Service, Location
from aliss.tests.fixtures import Fixtures

class OrganisationTestCase(TestCase):
    def setUp(self):
        t,u,c,s = Fixtures.create_users()
        o = Fixtures.create_organisation(t, u, c)

    def test_org_exists(self):
        o = Organisation.objects.get(name="TestOrg")
        self.assertTrue(isinstance(o, Organisation))

    def test_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="tester@aliss.org").delete()
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        ALISSUser.objects.get(email="claimant@user.org").delete()
        self.test_org_exists()

    def test_is_edited_by(self):
        o = Organisation.objects.get(name="TestOrg")
        rep    = ALISSUser.objects.get(email="claimant@user.org")
        staff  = ALISSUser.objects.get(email="staff@aliss.org")
        editor = ALISSUser.objects.filter(is_editor=True).first()
        punter = ALISSUser.objects.create(name="Ms Random", email="random@random.org")
        self.assertTrue(o.is_edited_by(o.created_by))
        self.assertTrue(o.is_edited_by(staff))
        self.assertTrue(o.is_edited_by(editor))
        self.assertTrue(o.is_edited_by(rep))
        self.assertFalse(o.is_edited_by(punter))