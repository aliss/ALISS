from django.views.generic import (
    View, CreateView, UpdateView, DeleteView, DetailView
)
from django.contrib import messages
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect

from django_filters.views import FilterView
from braces.views import (
    LoginRequiredMixin,
    StaffuserRequiredMixin,
    UserPassesTestMixin
)

from aliss.models import Organisation
from aliss.filters import OrganisationFilter
from aliss.search import delete_service
from django.utils import timezone
from datetime import timedelta
from django.db.models import Q
from aliss.views import ProgressMixin


class OrganisationCreateView(LoginRequiredMixin, CreateView):
    model = Organisation
    template_name = 'organisation/create.html'
    fields = [
        'name',
        'description',
        'phone',
        'email',
        'url',
        'facebook',
        'twitter',
    ]

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.pk }
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.created_by = self.request.user

        if self.request.user.is_editor or self.request.user.is_staff:
            self.object.published = True
        else:
            self.object.published = False

        self.object.save()
        messages.success(
            self.request, '<p>{name} has been successfully created.</p>'.format(
                name=self.object.name,
                url=reverse(
                    'claim_create',
                    kwargs={'pk': self.object.pk}
                )
            )
        )

        return HttpResponseRedirect(self.get_success_url())


class OrganisationUpdateView(LoginRequiredMixin, UserPassesTestMixin, UpdateView):
    model = Organisation
    template_name = 'organisation/update.html'
    fields = [
        'name',
        'description',
        'phone',
        'email',
        'url',
        'facebook',
        'twitter',
    ]

    def test_func(self, user):
        return self.get_object().is_edited_by(user)

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.pk}
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.updated_by = self.request.user
        self.object.save()

        messages.success(
            self.request,
            '{name} has been successfully updated.'.format(
                name=self.object.name
            )
        )

        return HttpResponseRedirect(self.get_success_url())


class OrganisationListView(StaffuserRequiredMixin, FilterView):
    template_name = 'organisation/list.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=True)


class OrganisationDetailView(ProgressMixin, DetailView):
    model = Organisation
    template_name = 'organisation/detail.html'

    def get_queryset(self):
        if self.request.user.is_staff:
            return Organisation.objects.all()
        else:
            return Organisation.objects.filter(Q(published=True) | Q(created_by=self.request.user) | Q(claimed_by=self.request.user))


class OrganisationDeleteView(UserPassesTestMixin, DeleteView):
    model = Organisation
    template_name = 'organisation/delete.html'

    def test_func(self, user):
        return self.get_object().is_edited_by(user)

    def get_success_url(self):
        if self.request.user.is_staff:
            return reverse_lazy('account_my_organisations')
        else:
            return reverse_lazy('account_my_organisations')

    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()
        # Delete services from search index
        for service in self.object.services.all():
            delete_service(service.pk)

        success_url = self.get_success_url()
        self.object.delete()

        messages.success(
            self.request,
            '{name} has been successfully deleted.'.format(
                name=self.object.name
            )
        )
        return HttpResponseRedirect(success_url)


class OrganisationSearchView(LoginRequiredMixin, FilterView):
    template_name = 'organisation/search.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=True).order_by('-created_on')


class OrganisationUnpublishedView(StaffuserRequiredMixin, FilterView):
    template_name = 'organisation/unpublished.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=False).order_by('-created_on')


class OrganisationPublishView(StaffuserRequiredMixin, View):
    def post(self, request, *args, **kwargs):
        organisation = get_object_or_404(Organisation, pk=self.kwargs['pk'])

        if Organisation.objects.filter(pk=organisation.pk).update(published=True, updated_by=self.request.user):
            messages.success(self.request, '{name} has been successfully published.'.format(name=organisation.name))
        else:
            messages.error(self.request, 'Could not publish {name}.'.format(name=organisation.name))

        return HttpResponseRedirect(reverse('organisation_detail', kwargs={'pk': organisation.pk}))