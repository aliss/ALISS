from django.test import TestCase
from aliss.models import ALISSUser, Category, Postcode, DigestSelection, Organisation, Service
from django.utils import timezone
from datetime import datetime
from datetime import timedelta
import pytz

class DigestTestCase(TestCase):
    def setUp(self):
        # Setup for basic DigestSelection creation and deletion
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.postcode  = Postcode.objects.create(pk="G2 1AL", postcode_district="G2", postcode_sector="G2 1", latitude=55.86045205, longitude=-4.24858105, council_area_2011_code="S12000046", health_board_area_2014_code= "S08000021", integration_authority_2016_code="S37000015")
        self.category = Category.objects.create(name="Conditions", slug="conditions")

        category2 = Category.objects.create(name="Money", slug="money")


        # Setup for checking the retieve method in the model works
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=t,
          updated_by=u,
          claimed_by=c
        )

        # Setup of Service area to match postcode
        service1 = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u, categories=[self.category])

        service2 = Service.objects.create(name="My Second Service", description="A handy service", organisation=o, created_by=t, updated_by=u, categories=[self.category])

        service3 = Service.objects.create(name="My Third Service", description="A handy service", organisation=o, created_by=t, updated_by=u, categories=[self.category])

        service4 = Service.objects.create(name="My Fourth Service", description="A handy service", organisation=o, created_by=t, updated_by=u, categories=[category2])

        # Need to create string comparison datetime
        utc = pytz.UTC
        current_date = datetime.now()
        urrent_date = utc.localize(current_date)
        self.comparison_date = current_date - timedelta(weeks=1)




    def test_can_create_digest(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))

    def test_can_retrieve_digest(self):
        # Create a service which checks for category of conditions
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        # Get a digest object
        test_digest = DigestSelection.objects.get(user=self.user)
        # retrieve the services which match the digest selection
        retrieved = test_digest.retrieve(self.comparison_date)[:3]
        number_retrieved = len(retrieved)
        self.assertTrue(number_retrieved == 0)


    def test_can_create_digest_without_category(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode)
        self.assertTrue(isinstance(d,DigestSelection))

    def test_user_delete_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        ALISSUser.objects.get(email="random@random.org").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)

    def test_category_delete_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        Category.objects.get(slug="conditions").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)

    def test_postcode_delte_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        Postcode.objects.get(pk="G2 1AL").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)
