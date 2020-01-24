from django import forms
from django.forms.formsets import BaseFormSet
from django.forms.models import formset_factory
from django.template.defaultfilters import mark_safe
from django.utils.translation import ugettext_lazy as _
from aliss.models import Organisation, Service, Property, AssignedProperty
from cloudinary import CloudinaryResource
import logging
logger = logging.getLogger(__name__)


class AssignedPropertyForm(forms.Form):
    selected    = forms.BooleanField(required=False)
    description = forms.CharField(required=False)
    property_pk = forms.UUIDField(required=True, widget=forms.HiddenInput)


class BaseAssignedPropertyFormSet(BaseFormSet):
    def __init__(self, organisation, *args, **kwargs):
        super(BaseAssignedPropertyFormSet, self).__init__(*args, **kwargs)
        self.organisation = organisation
        i = 0;
        properties = Property.objects.filter(for_organisations=True)
        self.initial = [{'property_pk': p.pk} for p in properties]
        for p in properties:
            self[i].fields['selected'].label = mark_safe("%s %s" % (p.icon_html(), p.name))
            assigned_prop = organisation.assigned_properties.filter(property_definition=p.pk).first()
            if assigned_prop:
                self[i].fields['description'].initial = assigned_prop.description
                self[i].fields['selected'].initial = True
            i = i + 1

    def clean(self):
        if any(self.errors):# Don't bother validating the formset unless each form is valid on its own
            return
        #raise forms.ValidationError("Something wrong with collection.")

    def save(self):
        #Replaces old assigned properties with ones contained in formset
        #NB: this will not trigger any re-index on organisation search index
        self.organisation.assigned_properties = []
        for f in self:
            logger.error(f.cleaned_data)
            if not f.cleaned_data.get('selected'):
                continue
            AssignedProperty.objects.create(
                property_definition=Property.objects.get(pk=f.cleaned_data.get("property_pk")),
                description=f.cleaned_data.get("description"),
                holder=self.organisation
            )
        return self.organisation.assigned_properties


AssignedPropertiesFormSet = formset_factory(AssignedPropertyForm, extra=0, formset=BaseAssignedPropertyFormSet)


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