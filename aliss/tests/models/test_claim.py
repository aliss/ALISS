from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Claim
from aliss.tests.fixtures import Fixtures

class ClaimTestCase(TestCase):
    def setUp(self):
        t,u,c,s = Fixtures.create_users()
        o = Fixtures.create_organisation(t)
        Claim.objects.create(comment="I'm in charge",
          organisation=o,
          user=c,
          reviewed_by=u
        )

    def test_claim_exists(self):
        cl = ALISSUser.objects.get(email="claimant@user.org")
        c = Claim.objects.get(comment="I'm in charge", user=cl)
        self.assertTrue(isinstance(c, Claim))

    def test_reviewed_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        self.test_claim_exists()

    def test_org_delete_cascades(self):
        Organisation.objects.get(name="TestOrg").delete()
        cl = ALISSUser.objects.get(email="claimant@user.org")
        exists = Claim.objects.filter(comment="I'm in charge", user=cl).exists()
        self.assertFalse(exists)

    def test_quit_representation(self):
        cl = ALISSUser.objects.get(email="claimant@user.org")
        c = Claim.objects.get(comment="I'm in charge", user=cl)
        o = Organisation.objects.get(name="TestOrg")
        c.quit_representation()
        exists = Claim.objects.filter(comment="I'm in charge", user=cl).exists()
        self.assertFalse(exists)
        self.assertIsNone(o.claimed_by)