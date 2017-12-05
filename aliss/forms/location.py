from django import forms
from django.conf import settings

from geopy.geocoders import GoogleV3

from aliss.models import Location


class LocationForm(forms.ModelForm):
    latitude = forms.FloatField(widget=forms.HiddenInput(), required=False)
    longitude = forms.FloatField(widget=forms.HiddenInput(), required=False)

    class Meta:
        model = Location
        fields = (
            'organisation',
            'name',
            'description',
            'street_address',
            'locality',
            'region',
            'postal_code',
            'latitude',
            'longitude'
        )
        labels = {
            'locality': 'Town',
            'region': 'Local Authority',
            'postal_code': 'Post Code'
        }

    def clean(self):
        cleaned_data = super(LocationForm, self).clean()

        street_address = cleaned_data.get("street_address")
        locality = cleaned_data.get("locality")
        region = cleaned_data.get("region")
        postal_code = cleaned_data.get("postal_code")

        address = ",".join([street_address, locality, region, postal_code])

        geolocator = GoogleV3(api_key=settings.GOOGLE_API_KEY)
        geocode_result = geolocator.geocode(
            address, components={'country': 'gb'}, exactly_one=True, timeout=5
        )

        if geocode_result:
            cleaned_data['latitude'] = geocode_result.latitude
            cleaned_data['longitude'] = geocode_result.longitude
        else:
            raise forms.ValidationError(
                    "Could not find this address, are you sure it is valid?"
                )

        return cleaned_data
