from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import (
    LocationUpdateView,
    LocationDeleteView,
    LocationDetailView
)

urlpatterns = [
    url(r'^edit/(?P<pk>[0-9A-Za-z\-]+)/$',
        LocationUpdateView.as_view(),
        name='location_edit'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        LocationDeleteView.as_view(),
        name='location_delete'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        LocationDetailView.as_view(),
        name='location_detail'
    ),
]
