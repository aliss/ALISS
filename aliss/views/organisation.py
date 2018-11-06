from django.views.generic import (
    View, CreateView, UpdateView, DeleteView, DetailView
)
from django.contrib import messages
from django.conf import settings
from django.core.mail import send_mail
from django.core.exceptions import PermissionDenied
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
            'organisation_confirm',
            kwargs={'pk': self.object.pk }
        )

    def send_new_org_email(self, organisation):
        message = '{organisation} has been added to ALISS by {user}.'.format(organisation=organisation, user=organisation.created_by)
        message += '\n\nGo to {link} to approve it.'.format(link=self.request.build_absolute_uri(reverse('organisation_unpublished')))
        send_mail(
            'A new organisation: {organisation} requires approval'.format(organisation=organisation),
            message,
            settings.DEFAULT_FROM_EMAIL,
            ['hello@aliss.org'],
            fail_silently=True,
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.created_by = self.request.user

        if self.request.user.is_editor or self.request.user.is_staff:
            self.object.published = True
        else:
            self.object.published = False
            self.send_new_org_email(self.object)

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
        if (self.object.services.count() == 0):
            return reverse('organisation_confirm', kwargs={ 'pk': self.object.pk })
        else:
            return reverse('organisation_detail_slug', kwargs={ 'slug': self.object.slug })

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


class OrganisationDetailView(LoginRequiredMixin, ProgressMixin, DetailView):
    model = Organisation
    template_name = 'organisation/detail.html'

    def get_object(self, queryset=None):
        #import logging
        #logger = logging.getLogger(__name__)
        #logger.error('Detail view')
        obj = super(OrganisationDetailView, self).get_object(queryset=queryset)
        return obj


class OrganisationConfirmView(UserPassesTestMixin, ProgressMixin, DetailView):
    model = Organisation
    template_name = 'organisation/confirm.html'
    
    def test_func(self, user):
        return self.get_object().is_edited_by(user)

    def get_object(self, queryset=None):
        obj = super(OrganisationConfirmView, self).get_object(queryset=queryset)
        return obj


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
        if self.request.user.is_editor or self.request.user.is_staff:
            return Organisation.objects.order_by('-created_on')
        else:
            return Organisation.objects.filter(published=True).order_by('-created_on')


class OrganisationUnpublishedView(StaffuserRequiredMixin, FilterView):
    template_name = 'organisation/unpublished.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=False).order_by('-created_on')


class OrganisationPublishView(StaffuserRequiredMixin, View):

    def send_published_email(self, organisation):
        message = '{organisation} has been approved. Services added to the organisation will now be available via our search (https://www.aliss.org).'.format(organisation=organisation)
        message += "\n\n-----\n\n\nThanks from the ALISS team\n\nIf you need to get in touch please contact us at:\n\nhello@aliss.org or 0141 404 0239"
        send_mail(
            '{organisation} now on ALISS'.format(organisation=organisation),
            message,
            settings.DEFAULT_FROM_EMAIL,
            [organisation.created_by.email, organisation.claimed_by],
            fail_silently=True,
        )

    def post(self, request, *args, **kwargs):
        organisation = get_object_or_404(Organisation, pk=self.kwargs['pk'])

        if Organisation.objects.filter(pk=organisation.pk).update(published=True, updated_by=self.request.user):
            messages.success(self.request, '{name} has been successfully published.'.format(name=organisation.name))
            self.send_published_email(organisation)
        else:
            messages.error(self.request, 'Could not publish {name}.'.format(name=organisation.name))

        return HttpResponseRedirect(reverse('organisation_detail_slug', kwargs={'slug': organisation.slug}))