from django.conf.urls import url, include

from .views import SearchView, CategoryListView, ServiceAreaListView

urlpatterns = [
    url(r'^v3/search/$', SearchView.as_view()),
    url(r'^v3/categories/$', CategoryListView.as_view()),
    url(r'^v3/service-areas/$', ServiceAreaListView.as_view()),
]