from django.conf.urls import url

from aliss.views import (
    OrganisationCreateView,
    OrganisationUpdateView,
    OrganisationListView,
    OrganisationDetailView,
    OrganisationDeleteView,
    OrganisationSearchView,
    OrganisationUnpublishedView,
    OrganisationPublishView,
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
    url(r'^publish/(?P<pk>[0-9A-Za-z\-]+)/$',
        OrganisationPublishView.as_view(),
        name='organisation_publish'
    ),
]