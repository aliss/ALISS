from django import forms
from django.forms.formsets import BaseFormSet
from django.forms.models import formset_factory
from django.template.defaultfilters import mark_safe
from django.utils.translation import ugettext_lazy as _
from aliss.models import Organisation, Service, Location, Property, AssignedProperty


class AssignedPropertyForm(forms.Form):
    selected    = forms.BooleanField(required=False)
    description = forms.CharField(required=False)
    property_pk = forms.UUIDField(required=True, widget=forms.HiddenInput)


class BaseAssignedPropertyFormSet(BaseFormSet):
    def __init__(self, property_holder, *args, **kwargs):
        super(BaseAssignedPropertyFormSet, self).__init__(*args, **kwargs)
        self.property_holder = property_holder
        properties = Property.relevant_properties(property_holder)
        self.initial = [{'property_pk': p.pk} for p in properties]
        i = 0;
        for p in properties:
            self[i].fields['selected'].label = mark_safe("%s %s" % (p.icon_html(), p.name))
            assigned_prop = property_holder.assigned_properties.filter(property_definition=p.pk).first()
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
        #NB: this will not trigger any re-index on organisation/service search index
        self.property_holder.assigned_properties = []
        for f in self:
            if not f.cleaned_data.get('selected'):
                continue
            AssignedProperty.objects.create(
                property_definition=Property.objects.get(pk=f.cleaned_data.get("property_pk")),
                description=f.cleaned_data.get("description"),
                holder=self.property_holder
            )
        return self.property_holder.assigned_properties


AssignedPropertiesFormSet = formset_factory(AssignedPropertyForm, extra=0, formset=BaseAssignedPropertyFormSet)
