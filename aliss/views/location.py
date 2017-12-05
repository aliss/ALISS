from django.views.generic import CreateView, UpdateView, DetailView, DeleteView
from django.contrib import messages
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect

from braces.views import StaffuserRequiredMixin

from aliss.search import index_object
from aliss.models import Location, Organisation
from aliss.forms import LocationForm


class LocationCreateView(StaffuserRequiredMixin, CreateView):
    model = Location
    form_class = LocationForm
    template_name = 'location/create.html'

    def get_initial(self):
        organisation = get_object_or_404(
            Organisation,
            pk=self.kwargs.get('pk')
        )
        return {
            'organisation': organisation
        }

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.created_by = self.request.user
        self.object.save()

        messages.success(
            self.request,
            'Location has been successfully created.'
        )

        return HttpResponseRedirect(self.get_success_url())


class LocationUpdateView(StaffuserRequiredMixin, UpdateView):
    model = Location
    form_class = LocationForm
    template_name = 'location/update.html'

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.updated_by = self.request.user
        self.object.save()

        messages.success(
            self.request,
            'Location has been successfully updated.'
        )

        return HttpResponseRedirect(self.get_success_url())


class LocationDetailView(StaffuserRequiredMixin, DetailView):
    model = Location
    template_name = 'location/detail.html'


class LocationDeleteView(StaffuserRequiredMixin, DeleteView):
    model = Location
    template_name = 'location/delete.html'

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()
        success_url = self.get_success_url()
        self.object.delete()

        for service in self.object.services:
            index_object(service, 'service')

        messages.success(
            self.request,
            'Location has been successfully deleted.'
        )
        return HttpResponseRedirect(success_url)
