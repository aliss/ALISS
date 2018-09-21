from django.test import TestCase
from aliss.models import ALISSUser, Category, Postcode, DigestSelection

class DigestTestCase(TestCase):
    def setUp(self):

            self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
            self.postcode  = Postcode.objects.create(pk="G2 1AL", postcode_district="G2", postcode_sector="G2 1", latitude=55.86045205, longitude=-4.24858105, council_area_2011_code="S12000046", health_board_area_2014_code= "S08000021", integration_authority_2016_code="S37000015")
            self.category = Category.objects.create(name="Conditions", slug="conditions")

    def test_can_create_digest(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))


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
