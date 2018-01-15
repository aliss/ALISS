from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import ClaimListView, ClaimDetailView, ClaimCreateView


urlpatterns = [
    url(r'^$',
        ClaimListView.as_view(),
        name='claim_list'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        ClaimDetailView.as_view(),
        name='claim_detail'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/claim/$',
        ClaimCreateView.as_view(),
        name='claim_create'
    ),
]
