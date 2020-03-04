from django import forms
from django.forms.formsets import BaseFormSet
from django.forms.models import formset_factory
from django.template.defaultfilters import mark_safe
from django.utils.translation import ugettext_lazy as _
from aliss.models import Organisation, Service, Property, AssignedProperty
from cloudinary import CloudinaryResource


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

    def __init__(self, *args, **kwargs):
        updated_by_user = kwargs.pop('updated_by')
        created_by_user = None
        if 'created_by' in kwargs:
            created_by_user = kwargs.pop('created_by')
        super(OrganisationForm, self).__init__(*args, **kwargs)
        self.instance.updated_by = updated_by_user
        if created_by_user:
            self.instance.created_by = created_by_user
            self.instance.published = created_by_user.is_editor or created_by_user.is_staff

    def clean(self):
        cleaned_data = super(OrganisationForm, self).clean()
        logo = cleaned_data.get("logo")
        if (not isinstance(logo, CloudinaryResource)) and logo and logo.size < 5000:
            del cleaned_data['logo']
            raise forms.ValidationError('The image selected for the logo is too small, please use a higher quality image.')
        return cleaned_data

    def save(self, commit=True):
        if self.errors:
            raise ValueError(
                "The %s could not be %s because the data didn't validate." % (
                    self.instance._meta.object_name,
                    'created' if self.instance._state.adding else 'changed',
                )
            )
        if commit:
            self.instance.save(kwargs={'skip_index': True})
            self._save_m2m()
            self.instance.update_index()
        else:
            self.save_m2m = self._save_m2m
        return self.instance