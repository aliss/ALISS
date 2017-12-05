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

    def __init__(self, *args, **kwargs):
        organisation = kwargs.pop('organisation')

        super(ServiceForm, self).__init__(*args, **kwargs)

        self.fields['locations'].queryset = Location.objects.filter(
            organisation=organisation.pk
        )

        self.fields['service_areas'].choices = service_areas_as_choices()


class ServiceProblemForm(forms.ModelForm):
    email = forms.EmailField(label='Your email')
    message = forms.CharField(widget=forms.Textarea, help_text='Tell us about the problem')

    class Meta:
        model = ServiceProblem
        fields = ('email', 'message')


class ServiceProblemUpdateForm(forms.ModelForm):
    class Meta:
        model = ServiceProblem
        fields = ('status',)

