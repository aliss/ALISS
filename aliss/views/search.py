from django.views.generic import View, TemplateView
from django.views.generic.list import MultipleObjectMixin
from django.conf import settings
from django.urls import reverse
from django.http import HttpResponseRedirect

from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

from aliss.paginators import ESPaginator
from aliss.forms import SearchForm
from aliss.models import Postcode
from aliss.search import (
    filter_by_query,
    filter_by_postcode,
    filter_by_location_type,
    filter_by_category
)

class SearchView(MultipleObjectMixin, TemplateView):
    template_name = 'search/results.html'
    paginator_class = ESPaginator
    paginate_by = 10

    def get_context_data(self, **kwargs):
        context = super(SearchView, self).get_context_data(**kwargs)
        context['postcode'] = self.postcode
        context['category'] = self.category
        return context

    def get(self, request, *args, **kwargs):
        search_form = SearchForm(data=self.request.GET)

        if search_form.is_valid():
            self.q = search_form.cleaned_data.get('q', None)
            self.location_type = search_form.cleaned_data.get(
                'location_type',
                None
            )
            self.category = search_form.cleaned_data.get('category', None)

            postcode = search_form.cleaned_data.get('postcode', None)

            if postcode:
                try:
                    self.postcode = Postcode.objects.get(postcode=postcode)
                except Postcode.DoesNotExist:
                    return self.render_to_response(context={'invalid_area': True})

            self.object_list = self.filter_queryset(self.get_queryset())
            return self.render_to_response(self.get_context_data())
        else:
            return self.render_to_response(
                    context={'errors': search_form.errors}
            )

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')

        if self.request.user.is_staff:
            queryset = queryset.extra(explain=True)

        return queryset

    def filter_queryset(self, queryset):
        if self.q:
            queryset = filter_by_query(queryset, self.q)
        if self.postcode:
            queryset = filter_by_postcode(
                queryset, self.postcode
            )
        if self.location_type:
            queryset = filter_by_location_type(queryset, self.location_type)
        if self.category:
            queryset = filter_by_category(queryset, self.category)

        return queryset


class SearchShareView(View):
    def get(self, request, *args, **kwargs):
        return HttpResponseRedirect(
            "{url}?postcode={postcode}&q={query}".format(
                url=reverse('search'),
                postcode=self.kwargs.get('postcode'),
                query=self.kwargs.get('query')
            )
        )
