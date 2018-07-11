from django.views.generic import View, TemplateView, FormView
from django.contrib import messages
from django.conf import settings
from django.contrib.sites.shortcuts import get_current_site
from django.http import HttpResponseRedirect
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404

from braces.views import LoginRequiredMixin, StaffuserRequiredMixin

from aliss.models import ALISSUser, Service, ServiceArea, Organisation, RecommendedServiceList, ServiceProblem, Claim
from datetime import datetime

class ReportsView(StaffuserRequiredMixin, TemplateView):
    template_name = 'reports/reports.html'

    def get_context_data(self, **kwargs):
        context = super(ReportsView, self).get_context_data(**kwargs)

        current_month = datetime.now().month
        current_year = datetime.now().year

        context['service_month_count'] = Service.objects.filter(created_on__month=current_month, created_on__year=current_year).count()
        context['orgs_month_count'] = Organisation.objects.filter(created_on__month=current_month, created_on__year=current_year).count()
        context['user_month_count'] = ALISSUser.objects.filter(date_joined__month=current_month, date_joined__year=current_year).count()
        context['problem_month_count'] = ServiceProblem.objects.filter(created_on__month=current_month, created_on__year=current_year).count()
        context['claim_request_count'] = Claim.objects.filter(created_on__month=current_month, created_on__year=current_year).count()

        context['services'] = Service.objects.prefetch_related('locations', 'service_areas').all()
        context['service_areas'] = ServiceArea.objects\
            .prefetch_related('services')\
            .order_by('type', 'code')
        context['recently_added'] = Service.objects.all().order_by('-created_on')[:10]

        return context