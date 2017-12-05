from django.conf.urls import url, include
from django.views.generic import TemplateView

urlpatterns = [
    url(r'^$',
        TemplateView.as_view(template_name="homepage.html"),
        name='homepage'
    ),
    url(r'^terms-and-conditions/$',
        TemplateView.as_view(template_name="terms-and-conditions.html"),
        name='terms_and_conditions'
    ),
    url(r'^privacy-policy/$',
        TemplateView.as_view(template_name="privacy-policy.html"),
        name='privacy_policy'
    ),
    url(r'^about/$',
        TemplateView.as_view(template_name="about.html"),
        name='about'
    ),
    url(r'^account/', include('aliss.urls.account')),
    url(r'^search/', include('aliss.urls.search')),
    url(r'^organisations/', include('aliss.urls.organisation')),
    url(r'^locations/', include('aliss.urls.location')),
    url(r'^services/', include('aliss.urls.service')),
    url(r'^claims/', include('aliss.urls.claim')),
]
