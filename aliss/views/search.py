from django.views.generic import TemplateView
from django.views.generic.list import MultipleObjectMixin
from django.conf import settings

from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

from aliss.paginators import ESPaginator
from aliss.forms import SearchForm
from aliss.models import Postcode
from aliss.search import (
    filter_by_query,
    filter_by_postcode
)

class SearchView(MultipleObjectMixin, TemplateView):
    template_name = 'search/results.html'
    paginator_class = ESPaginator
    paginate_by = 20

    def get_context_data(self, **kwargs):
        context = super(SearchView, self).get_context_data(**kwargs)
        context['postcode'] = self.postcode
        return context

    def postcode_valid(self):
        if not self.postcode or \
        self.postcode.health_board_area_2014_code not in \
        ['S08000021', 'S08000023', 'S08000020', 'S08000015']:
            return False
        else:
            return True

    def get(self, request, *args, **kwargs):
        search_form = SearchForm(data=self.request.GET)

        if search_form.is_valid():
            self.q = search_form.cleaned_data.get('q', None)

            postcode = search_form.cleaned_data.get('postcode', None)

            if postcode:
                try:
                    self.postcode = Postcode.objects.get(postcode=postcode)
                except Postcode.DoesNotExist:
                    self.postcode = None

                if not self.postcode_valid():
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

        return queryset

