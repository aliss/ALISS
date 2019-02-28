from itertools import groupby
from django import forms
from aliss.models import Organisation, Service
from django.utils.translation import ugettext_lazy as _


class OrganisationForm(forms.ModelForm):
    class Meta:
        model = Organisation

        fields = [
            'name',
            'description',
            'phone',
            'email',
            'url',
            'facebook',
            'twitter',
            'logo',
        ]

        labels = {}

        help_texts = {
            'description': _('Helpful text for a description'),
        }

    def clean(self):
        cleaned_data = super(OrganisationForm, self).clean()
        logo = cleaned_data.get("logo")
        if logo and logo.size < 5000:
            del cleaned_data['logo']
            raise forms.ValidationError('The image selected for the logo is too small, please use a higher quality image.')
        return cleaned_data
