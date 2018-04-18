from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import (
    ClaimListView,
    ClaimDetailView,
    ClaimCreateView,
    ClaimDeleteView,
)

urlpatterns = [
    url(r'^$',
        ClaimListView.as_view(),
        name='claim_list'
    ),
    url(r'^thanks/$',
        TemplateView.as_view(template_name="claim/thanks.html"),
        name='claim_thanks'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        ClaimDetailView.as_view(),
        name='claim_detail'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/claim/$',
        ClaimCreateView.as_view(),
        name='claim_create'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        ClaimDeleteView.as_view(),
        name='claim_delete'
    ),
]