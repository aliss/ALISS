from django.views.generic import View, TemplateView, FormView
from django.contrib import messages
from django.conf import settings
from django.contrib.sites.shortcuts import get_current_site
from django.http import HttpResponseRedirect
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.db.models import Count

from braces.views import LoginRequiredMixin, StaffuserRequiredMixin

from aliss.models import ALISSUser, Service, ServiceArea, Organisation, RecommendedServiceList, ServiceProblem, Claim
from datetime import datetime
import pytz


class ReportsView(StaffuserRequiredMixin, TemplateView):
    template_name = 'reports/reports.html'

    def get(self, request, *args, **kwargs):
        return self.render_to_response(self.get_context_data())

    def get_context_data(self, **kwargs):
        context = super(ReportsView, self).get_context_data(**kwargs)

        start_str = self.request.GET.get('start_date_submit',None)
        end_str   = self.request.GET.get('end_date_submit',None)
        context['filter_unpublished'] = self.request.GET.get('filter_unpublished', '')

        context['start_date'] = datetime.now().replace(day=1)
        context['end_date'] = datetime.now()

        if start_str:
            context['start_date'] = datetime.strptime(start_str, '%Y/%m/%d')
        if end_str:
            context['end_date'] = datetime.strptime(end_str, '%Y/%m/%d')

        context['start_date'] = context['start_date'].replace(tzinfo=pytz.UTC)
        context['end_date'] = context['end_date'].replace(tzinfo=pytz.UTC)

        orgs = Organisation.objects.filter(created_on__gte=context['start_date']).filter(created_on__lte=context['end_date'])
        services = Service.objects.filter(created_on__gte=context['start_date']).filter(created_on__lte=context['end_date'])

        if context['filter_unpublished'] == 'true':
            orgs = orgs.exclude(published=False)
            services = services.exclude(organisation__published=False)

        context['orgs_count'] = orgs.count()
        context['service_count'] = services.count()

        context['user_count']    = ALISSUser.objects.filter(date_joined__gte=context['start_date']).filter(date_joined__lte=context['end_date']).count()
        context['problem_count'] = ServiceProblem.objects.filter(created_on__gte=context['start_date']).filter(created_on__lte=context['end_date']).count()
        context['claim_request_count'] = Claim.objects.filter(created_on__gte=context['start_date']).filter(created_on__lte=context['end_date']).count()

        context['helpful_services'] = Service.objects.annotate(num_helped=Count('helped_users')).order_by('-num_helped')[:10]
        return context