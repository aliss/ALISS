from django.views.generic import CreateView, UpdateView, ListView, DetailView, FormView, DeleteView, TemplateView
from django.contrib import messages
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect

from braces.views import StaffuserRequiredMixin

from aliss.search import index_service, delete_service
from aliss.models import Service, ServiceProblem, ServiceArea, Organisation
from aliss.forms import (
    ServiceForm,
    ServiceProblemForm,
    ServiceProblemUpdateForm
)


class ServiceCreateView(StaffuserRequiredMixin, CreateView):
    model = Service
    form_class = ServiceForm
    template_name = 'service/create.html'

    def get_organisation(self):
        return get_object_or_404(
            Organisation,
            pk=self.kwargs.get('pk')
        )

    def get(self, request, *args, **kwargs):
        self.organisation = self.get_organisation()
        return super(ServiceCreateView, self).get(request, *args, **kwargs)

    def post(self, request, *args, **kwargs):
        self.organisation = self.get_organisation()
        return super(ServiceCreateView, self).post(request, *args, **kwargs)

    def get_form_kwargs(self):
        kwargs = super(ServiceCreateView, self).get_form_kwargs()
        kwargs.update({'organisation': self.organisation})
        return kwargs

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.organisation = self.organisation
        self.object.created_by = self.request.user
        self.object.save()
        form.save_m2m()

        index_service(self.object)

        messages.success(
            self.request,
            '{name} has been successfully created.'.format(
                name=self.object.name
            )
        )

        return HttpResponseRedirect(self.get_success_url())

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )


class ServiceUpdateView(StaffuserRequiredMixin, UpdateView):
    model = Service
    form_class = ServiceForm
    template_name = 'service/update.html'

    def get_form_kwargs(self):
        kwargs = super(ServiceUpdateView, self).get_form_kwargs()
        kwargs.update({'organisation': self.object.organisation})
        return kwargs

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.updated_by = self.request.user
        self.object.save()
        form.save_m2m()

        index_service(self.object)

        messages.success(
            self.request,
            '{name} has been successfully updated.'.format(
                name=self.object.name
            )
        )

        return HttpResponseRedirect(self.get_success_url())


class ServiceDetailView(DetailView):
    model = Service
    template_name = 'service/detail.html'


class ServiceDeleteView(StaffuserRequiredMixin, DeleteView):
    model = Service
    template_name = 'service/delete.html'

    def get_success_url(self):
        return reverse(
            'organisation_detail',
            kwargs={'pk': self.object.organisation.pk}
        )

    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()
        success_url = self.get_success_url()

        # Delete from search index
        delete_service(self.object.pk)
        self.object.delete()

        messages.success(
            self.request,
            '{name} has been successfully deleted.'.format(
                name=self.object.name
            )
        )
        return HttpResponseRedirect(success_url)

class ServiceReportProblemView(CreateView):
    model = ServiceProblem
    form_class = ServiceProblemForm
    template_name = 'service/report-problem.html'

    def get_service(self):
        return get_object_or_404(
            Service,
            pk=self.kwargs.get('pk')
        )

    def get(self, request, *args, **kwargs):
        self.service = self.get_service()
        return super(ServiceReportProblemView, self).get(request, *args, **kwargs)

    def post(self, request, *args, **kwargs):
        self.service = self.get_service()
        return super(ServiceReportProblemView, self).post(request, *args, **kwargs)

    def get_context_data(self, **kwargs):
        context = super(ServiceReportProblemView, self).get_context_data(**kwargs)
        context['service'] = self.service
        return context

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.service = self.service
        self.object.save()

        return HttpResponseRedirect(self.get_success_url())


class ServiceProblemListView(StaffuserRequiredMixin, ListView):
    model = ServiceProblem
    template_name = 'service/problem-list.html'
    paginate_by = 10


class ServiceProblemDetailView(StaffuserRequiredMixin, UpdateView):
    model = ServiceProblem
    template_name = 'service/problem-detail.html'
    form_class = ServiceProblemUpdateForm


class ServiceCoverageView(StaffuserRequiredMixin, TemplateView):
    template_name = 'service/coverage.html'

    def get_context_data(self, **kwargs):
        context = super(ServiceCoverageView, self).get_context_data(**kwargs)

        services = Service.objects.prefetch_related(
                                'locations', 'service_areas'
                              ).all()
        context['services'] = services
        context['service_areas'] = ServiceArea.objects.prefetch_related('services').order_by('type', 'code')

        return context
