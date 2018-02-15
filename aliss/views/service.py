from django.views.generic import CreateView, UpdateView, ListView, DetailView, FormView, DeleteView, TemplateView
from django.contrib import messages
from django.conf import settings
from django.contrib.messages.views import SuccessMessageMixin
from django.contrib.sites.shortcuts import get_current_site
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.http import HttpResponseRedirect
from django.core.mail import EmailMultiAlternatives
from django.template import loader

from braces.views import (
    LoginRequiredMixin,
    StaffuserRequiredMixin,
    UserPassesTestMixin
)

from aliss.search import index_service, delete_service
from aliss.models import Service, ServiceProblem, ServiceArea, Organisation, RecommendedServiceList
from aliss.forms import (
    ServiceForm,
    ServiceProblemForm,
    ServiceProblemUpdateForm,
    ServiceEmailForm
)
from aliss.views import OrganisationMixin


class ServiceCreateView(
    LoginRequiredMixin,
    UserPassesTestMixin,
    OrganisationMixin,
    CreateView
):
    model = Service
    form_class = ServiceForm
    template_name = 'service/create.html'

    def test_func(self, user):
        return (user.is_staff or self.get_organisation().claimed_by == user)

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


class ServiceUpdateView(
    LoginRequiredMixin,
    UserPassesTestMixin,
    UpdateView
):
    model = Service
    form_class = ServiceForm
    template_name = 'service/update.html'

    def test_func(self, user):
        return (
            user.is_staff or self.get_object().organisation.claimed_by == user
        )

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

    def get_context_data(self, **kwargs):
        context = super(ServiceUpdateView, self).get_context_data(**kwargs)
        context['organisation'] = self.object.organisation
        return context


class ServiceDetailView(DetailView):
    model = Service
    template_name = 'service/detail.html'

    def get_context_data(self, **kwargs):
        context = super(ServiceDetailView, self).get_context_data(**kwargs)
        if self.request.user.is_authenticated():
            context['recommended_service_lists'] = RecommendedServiceList.objects.filter(user=self.request.user)
        return context


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


class ServiceReportProblemView(StaffuserRequiredMixin, CreateView):
    model = ServiceProblem
    form_class = ServiceProblemForm
    template_name = 'service/report_problem.html'
    success_url = reverse_lazy('service_report_problem_thanks')

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
        self.object.user = self.request.user
        self.object.save()

        return HttpResponseRedirect(self.get_success_url())


class ServiceProblemListView(StaffuserRequiredMixin, ListView):
    model = ServiceProblem
    template_name = 'service/problem_list.html'
    paginate_by = 10

    def get_queryset(self):
        return ServiceProblem.objects.all().order_by('status', 'created_on')


class ServiceProblemUpdateView(StaffuserRequiredMixin, UpdateView):
    model = ServiceProblem
    form_class = ServiceProblemUpdateForm
    success_url = reverse_lazy('service_problem_list')


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


class ServiceEmailView(SuccessMessageMixin, FormView):
    form_class = ServiceEmailForm
    success_message = "Service emailed successfully"

    def get_success_url(self):
        return reverse(
            'service_detail',
            kwargs={'pk': self.service.pk}
        )

    def form_valid(self, form):
        self.service = form.cleaned_data.get('service')

        current_site = get_current_site(self.request)
        site_name = current_site.name
        domain = current_site.domain

        context = {
            'service': self.service,
            'domain': domain,
            'protocol': 'https'
        }
        subject = "Someone has emailed you a resource from ALISS"
        body = loader.render_to_string("service/email/single.txt", context)

        email_message = EmailMultiAlternatives(
            subject,
            body,
            settings.DEFAULT_FROM_EMAIL,
            [form.cleaned_data.get('email')]
        )
        html_email = loader.render_to_string("service/email/single.html", context)
        email_message.attach_alternative(html_email, 'text/html')

        email_message.send()

        return super(ServiceEmailView, self).form_valid(form)
