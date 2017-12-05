from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import (
    OrganisationCreateView,
    OrganisationUpdateView,
    OrganisationListView,
    OrganisationDetailView,
    OrganisationDeleteView,
    OrganisationSearchView,
    LocationCreateView,
    ServiceCreateView,
    OrganisationClaimView,
    OrganisationUnPublishedView
)


urlpatterns = [
    url(r'^create/$',
        OrganisationCreateView.as_view(),
        name='organisation_create'
    ),
    url(r'^create/thanks/$',
        TemplateView.as_view(template_name="organisation/create_thanks.html"),
        name='organisation_create_thanks'
    ),
    url(r'^edit/(?P<pk>[0-9A-Za-z\-]+)/$',
        OrganisationUpdateView.as_view(),
        name='organisation_edit'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        OrganisationDeleteView.as_view(),
        name='organisation_delete'
    ),
    url(r'^$',
        OrganisationListView.as_view(),
        name='organisation_list'
    ),
    url(r'^unpublished/$',
        OrganisationUnPublishedView.as_view(),
        name='organisation_unpublished_list'
    ),
    url(r'^search/$',
        OrganisationSearchView.as_view(),
        name='organisation_search'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/locations/create/$',
        LocationCreateView.as_view(),
        name='location_create'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/services/create/$',
        ServiceCreateView.as_view(),
        name='service_create'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        OrganisationDetailView.as_view(),
        name='organisation_detail'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/claim/$',
        OrganisationClaimView.as_view(),
        name='organisation_claim'
    ),
]
