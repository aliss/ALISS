from django.views.generic import (
    CreateView, UpdateView, DeleteView, DetailView, FormView
)
from django.contrib import messages
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect

from django_filters.views import FilterView
from braces.views import LoginRequiredMixin, StaffuserRequiredMixin

from aliss.models import Organisation, Claim
from aliss.forms import OrganisationClaimForm
from aliss.filters import OrganisationFilter
from aliss.search import index_organisation, delete_organisation


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
        if self.object.published:
            return reverse(
                'organisation_detail',
                kwargs={'pk': self.object.pk}
            )
        else:
            return reverse('organisation_create_thanks')

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.created_by = self.request.user

        if self.request.user.is_staff:
            self.object.published = True

        self.object.save()

        if self.object.published:
            index_object(self.object, 'organisation')
        else:
            delete_object(self.object.pk, 'organisation')

        messages.success(
            self.request,
            '{name} has been successfully created.'.format(
                name=self.object.name
            )
        )

        return HttpResponseRedirect(self.get_success_url())


class OrganisationUpdateView(StaffuserRequiredMixin, UpdateView):
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

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.pk}
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.updated_by = self.request.user
        self.object.save()

        if self.object.published:
            index_organisation(self.object)
        else:
            delete_organisation(self.object.pk)

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


class OrganisationDetailView(DetailView):
    model = Organisation
    template_name = 'organisation/detail.html'

    def get_queryset(self):
        if self.request.user.is_staff:
            return Organisation.objects.all()
        else:
            return Organisation.objects.filter(published=True)


class OrganisationDeleteView(StaffuserRequiredMixin, DeleteView):
    model = Organisation
    template_name = 'organisation/delete.html'
    success_url = reverse_lazy('organisation_list')

    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()
        success_url = self.get_success_url()

        # Delete from search index
        delete_organisation(self.object.pk)
        self.object.delete()

        messages.success(
            self.request,
            '{name} has been successfully deleted.'.format(
                name=self.object.name
            )
        )
        return HttpResponseRedirect(success_url)


class OrganisationClaimView(LoginRequiredMixin, FormView):
    form_class = OrganisationClaimForm
    template_name = 'organisation/claim.html'
    success_url = reverse_lazy('organisation_claim_thanks')

    def form_valid(self, form):
        organisation = get_object_or_404(Organisation, pk=self.kwargs.get('pk'))

        Claim.objects.create(
            user=self.request.user,
            organisation=organisation,
            email=form.cleaned_data.get('email'),
            phone=form.cleaned_data.get('phone'),
            comment=form.cleaned_data.get('comment'),
            created_by=self.request.user
        )

        return HttpResponseRedirect(self.get_success_url())

    def get(self, request, *args, **kwargs):
        organisation = get_object_or_404(Organisation, pk=self.kwargs.get('pk'))
        return self.render_to_response(
            self.get_context_data(organisation=organisation)
        )


class OrganisationSearchView(LoginRequiredMixin, FilterView):
    template_name = 'organisation/search.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=True)


class OrganisationUnPublishedView(StaffuserRequiredMixin, FilterView):
    template_name = 'organisation/unpublished_list.html'
    paginate_by = 10
    filterset_class = OrganisationFilter

    def get_queryset(self):
        return Organisation.objects.filter(published=False)
