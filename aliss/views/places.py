import string
from django.http import HttpResponseRedirect, HttpResponse
from aliss.models import Postcode, Category, Service
from django.views.generic import View, TemplateView
from django.urls import reverse
from django.views.generic.list import MultipleObjectMixin
from elasticsearch_dsl.connections import connections
from django.conf import settings
from elasticsearch_dsl import Search
from aliss.search import (
    filter_by_query,
    filter_by_postcode,
    filter_by_location_type,
    filter_by_category,
    postcode_order,
    keyword_order,
    combined_order
)
import logging
from django.shortcuts import render

class PlacesView(MultipleObjectMixin, TemplateView):
    template_name = 'places/results.html'
    paginate_by = 10

    def get_context_data(self, **kwargs):
        context = super(PlacesView, self).get_context_data(**kwargs)
        context['postcode'] = self.postcode
        service_area = self.postcode.get_local_authority()
        if service_area:
            context['service_area'] = service_area.name
        context['category'] = self.category
        return context

    def get(self, request, place_slug, category_slug):
        self.category = Category.objects.get(slug=category_slug)
        self.postcode = Postcode.objects.get(slug=place_slug)
        self.radius = 10000
        return self.define_object_list_return_response()

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')
        if self.request.user.is_staff:
            queryset = queryset.extra(explain=True)
        return queryset

    def filter_queryset(self, queryset):
        queryset = filter_by_category(queryset, self.category)
        queryset = filter_by_postcode(queryset, self.postcode, self.radius)
        results = postcode_order(queryset, self.postcode)
        return Service.objects.filter(id__in=results["ids"]).order_by(results["order"])

    def define_object_list_return_response(self):
        self.object_list = self.filter_queryset(self.get_queryset())
        return self.render_to_response(self.get_context_data())
