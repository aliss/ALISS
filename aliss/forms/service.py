from itertools import groupby

from django import forms

from datetime import datetime, timedelta
import pytz

from aliss.models import Service, ServiceArea, ServiceProblem, Location

def service_areas_as_choices():
    choices = []
    data = ServiceArea.objects.order_by('type', 'code')

    for k, g in groupby(data, lambda x: x.get_type_display()):
        choices.append([k, list([[item.pk, (item.name + " (" + item.get_type_display() + ")")] for item in g])])

    return choices

class ServiceForm(forms.ModelForm):
    class Meta:
        model = Service
        url = forms.URLField()

        fields = [
            'name',
            'description',
            'phone',
            'email',
            'url',
            'categories',
            'locations',
            'service_areas',
            'start_date',
            'end_date',
        ]

        labels = {
            'name': 'Service name',
            'description': 'Service description',
            'phone': 'Service phone number',
            'url': 'Service web address'
        }

        help_texts = {
            'name': 'Please enter the official name of the service you would like to add.',
            # 'description': 'Service description',
            'phone': 'The most appropriate telephone number for someone to be able to access or find out more about the service.',
            'email': 'The most appropriate email address for someone to access or find out more about this service.',
            'url': 'The URL (web address) for the webpage of the service you would like to add. Copy and paste the link e.g. https://www.example.com/a-service',
            # 'categories': "Which Categories",
            'locations': "Select or add the specific locations where the service is delivered. If it's not delivered in a specific location you can indicate service coverage by selecting 'service areas' below.",
            # 'service_areas': "Any regions",
        }

    def __init__(self, *args, **kwargs):
        organisation = kwargs.pop('organisation')
        updated_by_user = kwargs.pop('updated_by')
        created_by_user = None
        if 'created_by' in kwargs:
            created_by_user = kwargs.pop('created_by')

        super(ServiceForm, self).__init__(*args, **kwargs)

        self.fields['locations'].queryset = Location.objects.filter(organisation=organisation.pk)
        self.fields['service_areas'].choices = service_areas_as_choices()
        self.instance.updated_by = updated_by_user
        self.instance.organisation = organisation
        if created_by_user:
            self.instance.created_by = created_by_user

    def clean(self):
        cleaned_data = super(ServiceForm, self).clean()
        locations = cleaned_data.get("locations")
        service_areas = cleaned_data.get("service_areas")
        start_date = cleaned_data.get("start_date")
        end_date = cleaned_data.get("end_date")
        if ((start_date != None) and (end_date != None)):
            end_date_valid = end_date - timedelta(days=2)
            if (start_date >= end_date_valid):
                raise forms.ValidationError('Please ensure this service starts at least three days before it ends.')

        if (locations.count() == 0) and (service_areas.count() == 0):
            raise forms.ValidationError('Please provide a location and/or a service area for this service.')
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


class ServiceProblemForm(forms.ModelForm):
    class Meta:
        model = ServiceProblem
        fields = ('type', 'message')


class ServiceProblemUpdateForm(forms.ModelForm):
    class Meta:
        model = ServiceProblem
        fields = ('status',)


class ServiceEmailForm(forms.Form):
    email = forms.EmailField()
    service = forms.ModelChoiceField(queryset=Service.objects.all())
