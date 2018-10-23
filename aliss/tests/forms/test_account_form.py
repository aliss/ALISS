from django.test import TestCase, RequestFactory
from django.urls import reverse
from aliss.models import *
from aliss.forms import *

class SignupFormTestCase(TestCase):

    def setUp(self):
        self.factory = RequestFactory()
        self.name = "Random"
        self.email = "random@random.org"
        self.password = "passwurd"
        self.password_mismatch = "passwuld"

    def test_form_is_valid_basic_user(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password, 'accept_terms_and_conditions':True })
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_valis_basic_user_postcode(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password, 'accept_terms_and_conditions':True, 'postcode':'EH21 6UW' })
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_valis_basic_user_postcode_case_insensitive(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password, 'accept_terms_and_conditions':True, 'postcode':'eH21 6uW' })
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_valis_basic_user_postcode_whitespace(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password, 'accept_terms_and_conditions':True, 'postcode':' EH21 6UW ' })
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), True)

    def test_form_is_invalid_basic_user_password_mismatch(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password_mismatch, 'accept_terms_and_conditions':True})
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), False)

    def test_form_is_invalid_basic_user_terms_not_accepted(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password_mismatch, 'accept_terms_and_conditions':False})
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), False)

    def test_form_is_invalid_basic_user_postcode_special_character(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password_mismatch, 'accept_terms_and_conditions':False, 'postcode':"EH2£ 6UW"})
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), False)

    def test_form_is_invalid_basic_user_postcode_too_many_characters(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password_mismatch, 'accept_terms_and_conditions':False, 'postcode':"EH21 6UWW"})
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), False)
