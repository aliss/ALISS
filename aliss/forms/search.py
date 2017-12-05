from django import forms

from localflavor.gb.forms import GBPostcodeField

from aliss.geocode import geocode_location


class SearchForm(forms.Form):
    q = forms.CharField(required=False)
    postcode = GBPostcodeField(required=True)
