from django.test import TestCase, RequestFactory
from django.urls import reverse
from aliss.models import *
from aliss.forms import DigestSelectionForm

class DigestSelectionFormTestCase(TestCase):
    fixtures = ['categories.json']

    def setUp(self):
        self.factory = RequestFactory()
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
        self.postcode  = Postcode.objects.create(pk="G2 1AL", postcode_district="G2", postcode_sector="G2 1", latitude=55.86045205, longitude=-4.24858105, council_area_2011_code="S12000046", health_board_area_2014_code= "S08000021", integration_authority_2016_code="S37000015")


    def test_form_is_valid(self):
        # Create an instance of the post request
        request = self.factory.post(reverse('account_create_my_digest'), { 'postcode': 'G2 1AL', 'category': 'conditions' })
        request.user = self.user
        # Populate a form object digest_selection form
        form = DigestSelectionForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_invalid(self):
        # Create an instance of the post request
        request = self.factory.post(reverse('account_create_my_digest'), { 'postcode': 'zzz!', 'category': 'conditions' })
        request.user = self.user
        # Populate a form object digest_selection form
        form = DigestSelectionForm(request.POST)
        self.assertEqual(form.is_valid(), False)