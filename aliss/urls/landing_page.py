from django.conf.urls import url

from aliss.views import (
    LandingPageView,
)

urlpatterns = [
    url(r'^$',
        LandingPageView.as_view(),
        name='claim_list'
    ),
]
