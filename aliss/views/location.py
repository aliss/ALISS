from django.views.generic import CreateView, UpdateView, DetailView, DeleteView
from django.contrib import messages
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect, JsonResponse

from braces.views import (
    LoginRequiredMixin,
    StaffuserRequiredMixin,
    UserPassesTestMixin
)

from aliss.models import Location, Organisation
from aliss.forms import LocationForm, AssignedPropertiesFormSet, AssignedPropertyForm
from aliss.views import OrganisationMixin, ProgressMixin


class LocationCreateView(
    LoginRequiredMixin,
    UserPassesTestMixin,
    OrganisationMixin,
    ProgressMixin,
    CreateView
):
    model = Location
    form_class = LocationForm
    template_name = 'location/create.html'

    def test_func(self, user):
        return self.get_organisation().is_edited_by(user)

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.organisation.pk}
        )

    def get_context_data(self, **kwargs):
        context = super(LocationCreateView, self).get_context_data(**kwargs)
        if 'formset' in kwargs:
            context['assigned_properties_formset'] = kwargs['formset']
        else:
            context['assigned_properties_formset'] = AssignedPropertiesFormSet(context['form'].instance)
        return context

    def post(self, request, *args, **kwargs):
        self.organisation = self.get_organisation()
        self.object = None
        form = self.get_form()
        formset = AssignedPropertiesFormSet(form.instance, self.request.POST)
        is_ajax = self.request.is_ajax()
        form_valid    = form.is_valid()
        formset_valid = formset.is_valid()
        if (form_valid and is_ajax) or (form_valid and formset_valid):
            return self.form_valid(form, formset)
        else:
            return self.form_invalid(form, formset)

    def form_valid(self, form, formset):
        self.object = form.save(commit=False)
        self.object.organisation = self.organisation
        self.object.created_by = self.request.user
        self.object.save()
        self.assigned_properties = formset.save(self.object)

        services = self.object.services.all()
        for service in services:
            service.update_index()
        if self.request.is_ajax():
            return JsonResponse({ 'pk': self.object.pk,'address': self.object.formatted_address })
        else:
            messages.success(self.request, 'Location has been successfully created.')
            return HttpResponseRedirect(self.get_success_url())

    def form_invalid(self, form, formset):
        return self.render_to_response(self.get_context_data(form=form, formset=formset))


class LocationUpdateView(LoginRequiredMixin, UserPassesTestMixin, UpdateView):
    model = Location
    form_class = LocationForm
    template_name = 'location/update.html'

    def test_func(self, user):
        return self.get_object().is_edited_by(user)

    def get_success_url(self):
        return reverse('organisation_detail', kwargs={'pk': self.object.organisation.pk})

    def form_valid(self, form, formset):
        self.object = form.save(commit=False)
        self.object.updated_by = self.request.user
        self.object.save()
        self.assigned_properties = formset.save()
        services = self.object.services.all()
        for service in services:
            service.update_index()
        messages.success(self.request, 'Location has been successfully updated.')
        return HttpResponseRedirect(self.get_success_url())

    def form_invalid(self, form, formset):
        return self.render_to_response(self.get_context_data(form=form, formset=formset))

    def get_context_data(self, **kwargs):
        context = super(LocationUpdateView, self).get_context_data(**kwargs)
        context['organisation'] = self.object.organisation
        if 'formset' in kwargs:
            context['assigned_properties_formset'] = kwargs['formset']
        else:
            context['assigned_properties_formset'] = AssignedPropertiesFormSet(self.object)
        return context

    def post(self, request, *args, **kwargs):
        self.object = self.get_object()
        form = self.get_form()
        formset = AssignedPropertiesFormSet(form.instance, self.request.POST)
        if form.is_valid() and formset.is_valid():
            return self.form_valid(form, formset)
        else:
            return self.form_invalid(form, formset)


class LocationDetailView(StaffuserRequiredMixin, DetailView):
    model = Location
    template_name = 'location/detail.html'


class LocationDeleteView(LoginRequiredMixin, UserPassesTestMixin, DeleteView):

    model = Location
    template_name = 'location/delete.html'

    def test_func(self, user):
        return self.get_object().is_edited_by(user)

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()

        # Get services to reindex
        services = self.object.services.all()

        success_url = self.get_success_url()
        self.object.delete()

        for service in services:
            service.add_to_index()

        messages.success(
            self.request,
            'Location has been successfully deleted.'
        )
        return HttpResponseRedirect(success_url)
