from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import (
    ServiceUpdateView,
    ServiceDetailView,
    ServiceDeleteView,
    ServiceReportProblemView,
    ServiceProblemListView,
    ServiceProblemDetailView,
    ServiceCoverageView

)

urlpatterns = [
    url(r'^coverage/$',
        ServiceCoverageView.as_view(),
        name='service_coverage'
    ),
    url(r'^edit/(?P<pk>[0-9A-Za-z\-]+)/$',
        ServiceUpdateView.as_view(),
        name='service_edit'
    ),
    url(r'^problems/$',
        ServiceProblemListView.as_view(),
        name='service_problem_list'
    ),
    url(r'^problems/(?P<pk>[0-9A-Za-z\-]+)/$',
        ServiceProblemDetailView.as_view(),
        name='service_problem_detail'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        ServiceDetailView.as_view(),
        name='service_detail'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        ServiceDeleteView.as_view(),
        name='service_delete'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/report-problem/$',
        ServiceReportProblemView.as_view(),
        name='service_report_problem'
    )
]
