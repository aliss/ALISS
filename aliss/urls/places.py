from django.conf.urls import url

from aliss.views import (
    PlacesView,
)

urlpatterns = [
    url(r'^$',
        PlacesView.as_view(),
        name='places'
    ),
    url(r'^(?P<place_slug>[\w-]+)/(?P<category_slug>[\w-]+)/$',
        PlacesView.as_view(),
        name='place-category'
    ),
]
