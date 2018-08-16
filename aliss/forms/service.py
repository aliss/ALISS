from itertools import groupby

from django import forms

from aliss.models import Service, ServiceArea, ServiceProblem, Location

def service_areas_as_choices():
    choices = []
    data = ServiceArea.objects.order_by('type', 'code')

    for k, g in groupby(data, lambda x: x.get_type_display()):
        choices.append([k, list([[item.pk, item.name] for item in g])])

    return choices

class ServiceForm(forms.ModelForm):
    class Meta:
        model = Service
        fields = [
            'name',
            'description',
            'phone',
            'email',
            'url',
            'categories',
            'locations',
            'service_areas'
        ]
        labels = {
            'name': 'Service name'
        }

    def __init__(self, *args, **kwargs):
        organisation = kwargs.pop('organisation')

        super(ServiceForm, self).__init__(*args, **kwargs)

        self.fields['locations'].queryset = Location.objects.filter(organisation=organisation.pk)
        self.fields['service_areas'].choices = service_areas_as_choices()

    def clean(self):
        cleaned_data = super(ServiceForm, self).clean()
        locations = cleaned_data.get("locations")
        service_areas = cleaned_data.get("service_areas")

        if (locations.count() == 0) and (service_areas.count() == 0):
            raise forms.ValidationError('Please provide a location and/or a service area for this service.')
        return cleaned_data


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
