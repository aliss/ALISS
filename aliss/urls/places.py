from django.conf.urls import url
from django.views.generic import TemplateView
from aliss.views import (
    PlacesView,
    PlaceView
)

urlpatterns = [
    url(r'^$',
        PlacesView.as_view(),
        name='places'
    ),
    url(r'^(?P<place_slug>[\w-]+)/$',
        PlaceView.as_view(template_name="places/place.html"),
        name='place'
    ),
    url(r'^(?P<place_slug>[\w-]+)/(?P<category_slug>[\w-]+)/$',
        PlacesView.as_view(),
        name='place-category'
    ),
]
