from django.conf.urls import url, include
from . import views as v3
from . import v4_views as v4

urlpatterns = [
    url(r'^v3/search/$', v3.SearchView.as_view()),
    url(r'^v3/categories/$', v3.CategoryListView.as_view()),
    url(r'^v3/service-areas/$', v3.ServiceAreaListView.as_view()),

    url(r'^v4/search/$', v4.SearchView.as_view()),
    url(r'^v4/categories/$', v4.CategoryListView.as_view()),
    url(r'^v4/service-areas/$', v4.ServiceAreaListView.as_view()),
]