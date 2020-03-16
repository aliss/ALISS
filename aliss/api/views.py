from django.conf import settings
from django.http import HttpResponse

from rest_framework import generics
from rest_framework.exceptions import ParseError
from rest_framework.renderers import JSONRenderer
from rest_framework.views import APIView

from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

from aliss.paginators import ESPageNumberPagination
from .serializers import (
    SearchSerializer,
    SearchInputSerializer,
    CategorySerializer,
    ServiceAreaSerializer
)

from aliss.models import Category, ServiceArea
from aliss.search import (
    filter_by_query,
    filter_by_postcode,
    filter_by_category,
    filter_by_location_type,
    sort_by_postcode
)

from threading import Thread

import uuid
import requests


class TrackUsageMixin(object):

    def send_request(payload):
        try:
            headers = { 'User-Agent': 'ALISS Production' }
            return requests.post('https://www.google-analytics.com/collect', params=payload, headers=headers)
        except:
          return None

    def build_request(request):
        measure_protocol_str = "tid="+ settings.ANALYTICS_ID +"&v=1&t=event&ec=api&ea=" + request.path
        query_strings = request.META.get('QUERY_STRING').replace("&", ",")
        if len(query_strings) > 0:
            measure_protocol_str = measure_protocol_str + "," + query_strings
        if request.user.is_authenticated:
            measure_protocol_str = measure_protocol_str + "&uid=" + str(request.user.id)
        else:
            cid = str(uuid.uuid5(uuid.NAMESPACE_DNS, request.META.get('HTTP_USER_AGENT', 'no-user-agent') + request.META.get('REMOTE_ADDR')))
            measure_protocol_str = measure_protocol_str + "&cid=" + cid
        return measure_protocol_str

    def dispatch(self, request, *args, **kwargs):
        measure_protocol_str = TrackUsageMixin.build_request(request)
        if not settings.DEBUG:
            Thread(target=TrackUsageMixin.send_request, args=[measure_protocol_str]).start()
        else:
            print("DEBUG: no measure protocol request sent")
            print("\tPayload: " + measure_protocol_str)
        return super(TrackUsageMixin, self).dispatch(request, *args, **kwargs)


class SearchView(TrackUsageMixin, generics.ListAPIView):
    serializer_class = SearchSerializer
    pagination_class = ESPageNumberPagination

    def list(self, request, *args, **kwargs):
        input_serializer = SearchInputSerializer(data=request.query_params)
        input_serializer.is_valid(raise_exception=True)

        self.input_data = input_serializer.validated_data

        return super(SearchView, self).list(request, *args, **kwargs)

    def filter_queryset(self, queryset):
        radius = self.input_data.get('radius')
        postcode = self.input_data.get('postcode')
        query = self.input_data.get('q', None)
        category = self.input_data.get('category', None)
        location_type = self.input_data.get('location_type', None)

        if query:
            queryset = filter_by_query(queryset, query)
        if category:
            queryset = filter_by_category(queryset, category)
        if location_type:
            queryset = filter_by_location_type(queryset, location_type)

        queryset = filter_by_postcode(queryset, postcode, radius)
        queryset = sort_by_postcode(queryset, postcode)
        return queryset

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')
        return queryset


class CategoryListView(TrackUsageMixin, generics.ListAPIView):
    queryset = Category.objects.all()
    serializer_class = CategorySerializer
    pagination_class = None


class ServiceAreaListView(TrackUsageMixin, generics.ListAPIView):
    queryset = ServiceArea.objects.all()
    serializer_class = ServiceAreaSerializer
    pagination_class = None

class ImportView(generics.ListAPIView):
    serializer_class = SearchSerializer
    pagination_class = ESPageNumberPagination

    def list(self, request, *args, **kwargs):
        return super(ImportView, self).list(request, *args, **kwargs)

    def filter_queryset(self, queryset):
        return queryset

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')
        return queryset 
