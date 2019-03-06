from django import forms
from django.conf import settings
from geopy.geocoders import GoogleV3
from geopy.geocoders import Nominatim
from aliss.models import Location


class LocationForm(forms.ModelForm):
    latitude = forms.FloatField(widget=forms.HiddenInput(), required=False)
    longitude = forms.FloatField(widget=forms.HiddenInput(), required=False)

    class Meta:
        model = Location
        fields = (
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
            'name': 'Location name',
            'locality': 'City/Town',
            'region': 'Local Authority',
            'postal_code': 'Post Code'
        }
        error_css_class = 'has-error'

    def nominatim_geocode(street_address, locality, postal_code):
        query = { 'street': street_address, 'city': locality, 'postalcode': postal_code }
        geolocator = Nominatim(country_bias='gb',user_agent="aliss_django")
        return geolocator.geocode(query, exactly_one=True, timeout=5)

    def clean(self):
        cleaned_data = super(LocationForm, self).clean()

        street_address = cleaned_data.get("street_address")
        locality = cleaned_data.get("locality")
        region = cleaned_data.get("region")
        postal_code = cleaned_data.get("postal_code")

        try:
            address = ",".join([street_address, locality, region, postal_code])
        except:
            raise forms.ValidationError("Could not find this address, are you sure it is valid?")

        try:
            geolocator = GoogleV3(api_key=settings.GOOGLE_API_KEY)
            geocode_result = geolocator.geocode(address, components={'country': 'gb'}, exactly_one=True, timeout=5)
        except:
            geocode_result = nominatim_geocode()

        if geocode_result:
            cleaned_data['latitude'] = geocode_result.latitude
            cleaned_data['longitude'] = geocode_result.longitude
        else:
            raise forms.ValidationError(
                    "Could not find this address, are you sure it is valid?"
                )

        return cleaned_data
