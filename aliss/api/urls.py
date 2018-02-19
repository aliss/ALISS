from django.conf.urls import url, include

from .views import SearchView, CategoryListView

urlpatterns = [
    url(r'^v3/search/$', SearchView.as_view()),
    url(r'^v3/categories/$', CategoryListView.as_view()),
]
