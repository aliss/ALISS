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
            'name': _('Please enter the official name of the organisation you would like to add.'),
            'description': _('Please try to input a clear and concise description of the organisation you would like to add. Keywords can effect your listing poistion in the search results so please add them appropriately.'),
            'phone': _('Please add the best telephone number for a user to call if they wish to contact the organisation. '),
            'email': _('Please enter the best email address for a user to contact the organisation you are adding.'),
            'url': _('Please enter the URL of the official organisation webpage if available. Copy the link which should be in the format of https://www.example.com'),
            'facebook': _('Please provide the link to the official Facebook page for the organisation you would like to add.'),
            'twitter': _('Please provide the link to the official Twitter account of the organisation you would like to add'),
            'logo': _('Please select the image file for the logo of the organisation you would like to add.'),
        }

    def clean(self):
        cleaned_data = super(OrganisationForm, self).clean()
        logo = cleaned_data.get("logo")
        if logo and logo.size < 5000:
            del cleaned_data['logo']
            raise forms.ValidationError('The image selected for the logo is too small, please use a higher quality image.')
        return cleaned_data
