from django.conf.urls import url
from django.views.generic import TemplateView

from aliss.views import ClaimListView, ClaimDetailView


urlpatterns = [
    url(r'^$',
        ClaimListView.as_view(),
        name='claim_list'
    ),
    url(r'^thanks/$',
        TemplateView.as_view(template_name="claim/thanks.html"),
        name='organisation_claim_thanks'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        ClaimDetailView.as_view(),
        name='claim_detail'
    )
]
