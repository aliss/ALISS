from django import forms
from localflavor.gb.forms import GBPostcodeField
from aliss.models import Category
import re
from django.forms import ValidationError


class GBPostcodeDistrictField(GBPostcodeField):
    district_pattern = re.compile(GBPostcodeField.outcode_pattern)

    def clean(self, value):
        value = super(forms.CharField, self).clean(value)
        if value == '':
            return value
        postcode = value.upper().strip()
        # Put a single space before the incode (second part).
        postcode = self.space_regex.sub(r' \1', postcode)
        if not self.postcode_regex.search(postcode) and not self.district_pattern.search(postcode):
            raise ValidationError(self.error_messages['invalid'])
        return postcode


class SearchForm(forms.Form):
    LOCAL = 'local'
    NATIONAL = 'national'
    LOCATION_TYPE_CHOICES = (
        (LOCAL, LOCAL),
        (NATIONAL, NATIONAL)
    )

    q = forms.CharField(required=False)
    postcode = GBPostcodeDistrictField(required=True)
    location_type = forms.ChoiceField(choices=LOCATION_TYPE_CHOICES, required=False)
    sort = forms.ChoiceField(choices=[('best_match', 'best_match'), ('nearest', 'nearest'), ('keyword', 'keyword')], required=False)
    category = forms.ModelChoiceField(
        queryset=Category.objects.all(),
        to_field_name="slug",
        required=False
    )
    radius = forms.IntegerField(required=False, min_value=1)
