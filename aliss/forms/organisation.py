from itertools import groupby
from django import forms
from cloudinary import CloudinaryResource
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
            'name': _('The official name of the organisation you would like to add.'),
            'description': _('A clear and concise description of the organisation you would like to add. The description will be used for the search feature so keep in mind key words or phrases.'),
            'phone': _('The most appropriate telephone number for someone to contact the organisation with.'),
            'email': _('The most appropriate email address for someone to contact the organisation with.'),
            'url': _('The URL (website address) of the organisation\'s offical webpage e.g. https://www.organisation.com'),
            'facebook': _('A link to the official Facebook page for the organisation you would like to add e.g. https://en-gb.facebook.com/rnibuk'),
            'twitter': _('A link to the official Twitter account of the organisation you would like to add e.g. https://twitter.com/rnibuk'),
            'logo': _('An image file for the logo of the organisation you would like to add.'),
        }

    def clean(self):
        cleaned_data = super(OrganisationForm, self).clean()
        logo = cleaned_data.get("logo")
        if (not isinstance(logo, CloudinaryResource)) and logo and logo.size < 5000:
            del cleaned_data['logo']
            raise forms.ValidationError('The image selected for the logo is too small, please use a higher quality image.')
        return cleaned_data
