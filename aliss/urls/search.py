from django.conf.urls import url

from aliss.views import SearchView, SearchOrganisationsView


urlpatterns = [
    url(r'^organisations/$', SearchOrganisationsView.as_view(), name='organisation_results'),
    url(r'^$', SearchView.as_view(), name='search')

]
