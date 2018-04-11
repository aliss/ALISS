from django.test import TestCase
from aliss.models import Organisation, ALISSUser, Claim

class ClaimTestCase(TestCase):
    def setUp(self):
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org")
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg", description="A test description",
          created_by=t, updated_by=u
        )
        Claim.objects.create(comment="I'm in charge",
          email="claimant@aliss.org",
          organisation=o,
          user=c,
          reviewed_by=u
        )

    def test_claim_exists(self):
        cl = ALISSUser.objects.get(email="claimant@aliss.org")
        c = Claim.objects.get(comment="I'm in charge", user=cl)
        self.assertTrue(isinstance(c, Claim))

    def test_reviewed_user_delete_doesnt_cascade(self):
        ALISSUser.objects.get(email="updater@aliss.org").delete()
        self.test_claim_exists()

    def test_org_delete_cascades(self):
        Organisation.objects.get(name="TestOrg").delete()
        cl = ALISSUser.objects.get(email="claimant@aliss.org")
        c = Claim.objects.filter(comment="I'm in charge", user=cl).exists()
        self.assertFalse(c)