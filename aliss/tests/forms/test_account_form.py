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

    def test_form_is_valid(self):
        request = self.factory.post(reverse('signup'), { 'name': self.name, 'email':self.email, 'password1': self.password, 'password2':self.password, 'postcode':'EH21 6UW' })
        form = SignupForm(request.POST)
        self.assertEqual(form.is_valid(), True)
