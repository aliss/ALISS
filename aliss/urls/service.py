from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import (
    ServiceUpdateView,
    ServiceDetailView,
    ServiceDeleteView,
    ServiceReportProblemView,
    ServiceProblemListView,
    ServiceProblemUpdateView,
    ServiceCoverageView,
    ServiceEmailView,
    ServiceAtLocationDelete
)

urlpatterns = [
    url(r'^service-at-location/(?P<service_at_location_pk>.+)/delete/$',
        ServiceAtLocationDelete.as_view(),
        name='service_at_location_delete'
    ),
    url(r'^coverage/$',
        ServiceCoverageView.as_view(),
        name='service_coverage'
    ),
    url(r'^email/$',
        ServiceEmailView.as_view(),
        name='service_email'
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
        ServiceProblemUpdateView.as_view(),
        name='service_problem_update'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        ServiceDeleteView.as_view(),
        name='service_delete'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/report-problem/$',
        ServiceReportProblemView.as_view(),
        name='service_report_problem'
    ),
    url("(?P<pk>[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})",
        ServiceDetailView.as_view(),
        name='service_detail'
    ),
    url(r'^(?P<slug>[0-9A-Za-z\-]+)/$',
        ServiceDetailView.as_view(),
        name='service_detail_slug'
    ),
    url(r'^report-problem/thanks/$',
        TemplateView.as_view(
            template_name='service/report_problem_thanks.html'
        ),
        name='service_report_problem_thanks'
    ),

]
