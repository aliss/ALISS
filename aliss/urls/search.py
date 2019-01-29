from django.conf.urls import url

from aliss.views import SearchView, SearchOrganisationsView

from django.views.generic import TemplateView

urlpatterns = [
    url(r'^organisations/results/$', SearchOrganisationsView.as_view(), name='results'),
    url(r'^organisations/$', TemplateView.as_view(template_name="search/organisation-search.html"), name='organisations'),
    url(r'^$', SearchView.as_view(), name='search')

]
