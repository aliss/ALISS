import string
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
    sort_by_postcode,
    filter_by_location_type,
    filter_by_category
)

from elasticsearch_dsl import Search
from django.conf import settings
from elasticsearch_dsl.connections import connections
from aliss.search import filter_organisations_by_query_all, filter_organisations_by_query_published, get_organisation_by_id, order_organistations_by_created_on, filter_by_claimed_status, filter_by_has_services

class SearchView(MultipleObjectMixin, TemplateView):
    template_name = 'search/results.html'
    paginator_class = ESPaginator
    paginate_by = 10

    def get_context_data(self, **kwargs):
        context = super(SearchView, self).get_context_data(**kwargs)
        context['postcode'] = self.postcode
        service_area = self.postcode.get_local_authority()
        if service_area:
            context['service_area'] = service_area.name
        context['category'] = self.category
        context['expanded_radius'] = self.radius * 2
        return context

    def get(self, request, *args, **kwargs):
        search_form = SearchForm(data=self.request.GET)

        if search_form.is_valid():
            self.q = search_form.cleaned_data.get('q', None)
            puncstripper = str.maketrans('', '', string.punctuation.replace('-', '')) #keep -
            self.q = self.q.translate(puncstripper)
            self.location_type = search_form.cleaned_data.get('location_type',None)
            self.keyword_sort = search_form.cleaned_data.get('keyword_sort', None)
            self.category = search_form.cleaned_data.get('category', None)
            self.radius = search_form.cleaned_data.get('radius', None)
            if self.radius == None:
                self.radius = 20000

            postcode = search_form.cleaned_data.get('postcode', None)

            if postcode:
                try:
                    self.postcode = Postcode.objects.get(postcode=postcode)
                except Postcode.DoesNotExist:
                    return self.render_to_response(context={'invalid_area': True})

            self.object_list = self.filter_queryset(self.get_queryset())
            return self.render_to_response(self.get_context_data())
        else:
            invalid_area = search_form.cleaned_data.get('postcode', None) == None
            return self.render_to_response(
                    context={'errors': search_form.errors, 'invalid_area': invalid_area}
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
        if self.location_type:
            queryset = filter_by_location_type(queryset, self.location_type)
        if self.postcode:
            queryset = filter_by_postcode(queryset, self.postcode, self.radius)
        if self.postcode and not self.keyword_sort:
            queryset = sort_by_postcode(queryset, self.postcode)
        if self.category:
            queryset = filter_by_category(queryset, self.category)

        return queryset


class SearchShareView(View):
    def get(self, request, *args, **kwargs):
        postcode = self.kwargs.get('postcode')
        query = self.kwargs.get('query')
        return HttpResponseRedirect(
            "{url}?postcode={postcode}{query}".format(
                url=reverse('search'),
                postcode=postcode,
                query="&q={query}".format(query=query) if query else ''
            )
        )

class SearchOrganisationsView(MultipleObjectMixin, TemplateView):
    template_name = 'search/organisation-results.html'
    paginator_class = ESPaginator
    paginate_by = 10

    def get(self, request, *args, **kwargs):
        self.object_list = self.get_queryset()
        return self.render_to_response(self.get_context_data())

    def filter_queryset(self, queryset):
        claimed_status = self.request.GET.get('is_claimed')
        has_services = self.request.GET.get('has_services')
        if claimed_status:
            queryset = filter_by_claimed_status(queryset, claimed_status)
        if has_services:
            queryset = filter_by_has_services(queryset, has_services)

        return queryset

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='organisation_search', doc_type='organisation')
        queryset = self.filter_queryset(queryset)
        query = self.request.GET.get('q')

        if query:
            if self.request.user.is_authenticated() and (self.request.user.is_editor or self.request.user.is_staff):
                queryset = filter_organisations_by_query_all(queryset, query)
            else:
                queryset = filter_organisations_by_query_published(queryset, query)

        return queryset
