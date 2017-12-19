from django import forms

from localflavor.gb.forms import GBPostcodeField

from aliss.geocode import geocode_location


class SearchForm(forms.Form):
    LOCAL = 'point'
    AREA = 'area'
    LOCATION_TYPE_CHOICES = (
        (LOCAL, LOCAL),
        (AREA, AREA)
    )

    q = forms.CharField(required=False)
    postcode = GBPostcodeField(required=True)
    location_type = forms.ChoiceField(
        choices=LOCATION_TYPE_CHOICES, required=False
    )
