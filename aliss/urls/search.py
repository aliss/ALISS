from django.conf.urls import url

from aliss.views import SearchView

from django.views.generic import TemplateView

urlpatterns = [
    url(r'^$', SearchView.as_view(), name='search')
]
