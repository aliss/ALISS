from django.conf.urls import url

from aliss.views import (
    PlacesView,
)

urlpatterns = [
    url(r'^$',
        PlacesView.as_view(),
        name='places'
    ),
    url(r'^(?P<slug>)/(?P<slug>)/$',
        PlacesView.as_view(),
        name='place-category'
    ),
]
