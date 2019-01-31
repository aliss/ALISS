from django.conf.urls import url

from aliss.views.organisation import *

from aliss.views import (
    LocationCreateView,
    ServiceCreateView
)

urlpatterns = [
    url(r'^create/$',
        OrganisationCreateView.as_view(),
        name='organisation_create'
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
        OrganisationUnpublishedView.as_view(),
        name='organisation_unpublished'
    ),
    url(r'^potential-create$',
        OrganisationPotentialCreateView.as_view(),
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
    url("(?P<pk>[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})/publish/",
        OrganisationPublishView.as_view(),
        name='organisation_publish'
    ),
    url(r'^confirm/(?P<pk>[0-9A-Za-z\-]+)/$',
        OrganisationConfirmView.as_view(),
        name='organisation_confirm'
    ),
    url("(?P<pk>[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})",
        OrganisationDetailView.as_view(),
        name='organisation_detail'
    ),
    url(r'^(?P<slug>[0-9A-Za-z\-]+)/$',
        OrganisationDetailView.as_view(),
        name='organisation_detail_slug'
    ),
]
