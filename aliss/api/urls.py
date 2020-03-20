from django.conf.urls import url, include
from . import views as v3
from . import v4_views as v4

urlpatterns = [
    url(r'^v3/search/$', v3.SearchView.as_view()),
    url(r'^v3/categories/$', v3.CategoryListView.as_view()),
    url(r'^v3/service-areas/$', v3.ServiceAreaListView.as_view()),

    url(r'^v4/services/$', v4.SearchView.as_view()),
    url(r'^v4/categories/$', v4.CategoryListView.as_view()),
    url(r'^v4/service-areas/$', v4.ServiceAreaListView.as_view()),
    url('v4/organisations/(?P<pk>[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})/', v4.OrganisationDetailView.as_view()),
    url(r'^v4/organisations/(?P<slug>[0-9A-Za-z\-]+)/$', v4.OrganisationDetailView.as_view()),
    url('v4/services/(?P<pk>[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})/', v4.ServiceDetailView.as_view()),
    url(r'^v4/services/(?P<slug>[0-9A-Za-z\-]+)/$',  v4.ServiceDetailView.as_view()),
    url(r'^v4/postcode-locations/$', v4.PostcodeLocationData.as_view()),
    url(r'^v4/service-area-spatial/full-set/$', v4.ServiceAreaFullSpatialDataSet.as_view()),
    url(r'^v4/service-area-spatial/$', v4.ServiceAreaSpatialData.as_view()),
    url(r'^v4/import/$', v4.ImportView.as_view()),
]
