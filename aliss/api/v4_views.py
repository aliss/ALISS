from rest_framework.views import APIView
from rest_framework.response import Response
from rest_framework import generics
from . import views as v3
from aliss.models import Category, ServiceArea, Organisation, Service, Postcode
from collections import OrderedDict
from copy import deepcopy
from django.shortcuts import get_object_or_404
from aliss.search import return_feature

from .serializers import (
    v4SearchSerializer,
    SearchInputSerializer,
    v4CategorySerializer,
    v4ServiceAreaSerializer,
    v4OrganisationDetailSerializer,
    v4ServiceSerializer,
    PostcodeLocationSerializer,
    PostcodeLocationSearchSerializer,
    ServiceAreaSpatialSearchSerializer,
    ServiceAreaFullSpatialDataSetSearchSerializer
)

class APIv4():
    META = {
        'licence': 'https://creativecommons.org/licenses/by/4.0/',
        'attribution': [{
            'text': 'Contains National Statistics data © Crown copyright and database right 2018',
            'url': 'http://geoportal.statistics.gov.uk/datasets/local-authority-districts-december-2016-generalised-clipped-boundaries-in-the-uk/'
        },{
            'text': 'Contains information from the Scottish Charity Register supplied by the Office of the Scottish Charity Regulator and licensed under the Open Government Licence v2.0',
            'url': 'https://www.oscr.org.uk/about-charities/search-the-register/charity-register-download'
        },{
            'text': 'Contains National Records of Scotland data licensed under the Open Government Licence v3.0',
            'url': 'https://www.nrscotland.gov.uk/statistics-and-data/geography/nrs-postcode-extract'
        },{
            'text': 'Contains contributions from ALISS users',
            'url': 'https://www.aliss.org/terms-and-conditions'
        }]
    }

class ImportView(v3.ImportView):
    serializer_class = v4SearchSerializer

    def get_serializer_context(self):
        return {'request': self.request}

    
    def get(self, request, *args, **kwargs):
        queryset = self.get_queryset()
        res = queryset.query({ "match_all":{}}).execute()
        total = res.hits.total
        request._mutable = True
        request.query_params._mutable = True

        if 'format' not in request.query_params:
            request.query_params['format'] = 'json'
        
        if 'page_size' not in request.query_params:
            request.query_params['page_size'] = total
        
        if 'page' not in request.query_params:
            request.query_params['page'] = 1
        
        response = self.list(request, *args, **kwargs)
        data = OrderedDict({'meta': APIv4.META})   
        data['count'] = response.data['count']
        data['next'] = response.data['next']
        data['previous'] = response.data['previous']
        data['data'] = response.data['results']
        return Response(data)

class SearchView(v3.SearchView):
    serializer_class = v4SearchSerializer

    def get_serializer_context(self):
        return {'request': self.request}

    def get(self, request, *args, **kwargs):
        response = self.list(request, *args, **kwargs)
        data = OrderedDict({'meta': APIv4.META})
        data['count'] = response.data['count']
        data['next'] = response.data['next']
        data['previous'] = response.data['previous']
        data['data'] = response.data['results']
        return Response(data)


class ServiceAreaListView(v3.ServiceAreaListView):
    def list(self, request):
        queryset = self.get_queryset()
        serializer = v4ServiceAreaSerializer(queryset, many=True)
        data = OrderedDict()
        data['meta'] = deepcopy(APIv4.META)
        data['meta']['attribution'].pop()
        data['meta']['attribution'].pop()
        data['data'] = serializer.data
        return Response(data)


class CategoryListView(v3.CategoryListView):

    def get_queryset(self):
        return Category.objects.filter(parent=None)

    def list(self, request):
        queryset = self.get_queryset()
        serializer = v4CategorySerializer(queryset, many=True)
        data = OrderedDict()
        data['meta'] = deepcopy(APIv4.META)
        data['meta']['attribution'] = []
        data['data'] = serializer.data
        return Response(data)


class OrganisationDetailView(v3.TrackUsageMixin, APIView):

    def get(self, request, pk=None, slug=None):
        if pk==None:
            organisation = get_object_or_404(Organisation, slug=slug)
        else:
            organisation = get_object_or_404(Organisation, pk=pk)
        context = { 'request': request }
        data = OrderedDict({'meta': APIv4.META})
        data['data'] = v4OrganisationDetailSerializer(organisation, many=False, context=context).data
        return Response(data)


class ServiceDetailView(v3.TrackUsageMixin, APIView):

    def get(self, request, pk=None, slug=None):
        if pk==None:
            service = get_object_or_404(Service, slug=slug)
        else:
            service = get_object_or_404(Service, pk=pk)
        context = { 'request': request }
        data = OrderedDict({'meta': APIv4.META})
        data['data'] = v4ServiceSerializer(service, many=False, context=context).data
        return Response(data)


class PostcodeLocationData(generics.ListAPIView):
    serializer_class = PostcodeLocationSerializer

    def get_queryset(self, *args, **kwargs):
        queryset = Postcode.objects.exclude(place_name=None)
        return queryset

    def list(self, request, *args, **kwargs):
        input_serializer = PostcodeLocationSearchSerializer(data=request.query_params)
        input_serializer.is_valid(raise_exception=True)
        self.input_data = input_serializer.validated_data
        return super(PostcodeLocationData, self).list(request, *args, **kwargs)

    def filter_queryset(self, queryset):
        query = self.input_data.get('q', None)
        if query and len(query) > 2:
            queryset = queryset.filter(place_name__istartswith = query)
            return queryset
        else:
            return None


class ServiceAreaSpatialData(APIView):
    # serializer_class = ServiceAreaSpatialSerializer

    def get(self, request, *args, **kwargs):
        input_serializer = ServiceAreaSpatialSearchSerializer(data=request.query_params)
        input_serializer.is_valid(raise_exception=True)
        input_data = input_serializer.validated_data
        service_id = input_data.get('service_id')
        service_area_objs = Service.objects.get(pk=service_id).service_areas.all()
        service_area_features = []
        for service_area_obj in service_area_objs:
            type = service_area_obj.type
            code = service_area_obj.code
            returned = []
            if type == 0:
                if code == "XS":
                    returned.append(return_feature(type, "S92000003"))
                else:
                    uk_codes = ["S92000003", "E92000001", "W92000004", "N92000002"]
                    for uk_code in uk_codes:
                        returned.append(return_feature(type, uk_code))
            else:
                returned.append(return_feature(type, code))

            service_area_features += returned
        queryset = list(service_area_features)
        return Response(queryset)

class ServiceAreaFullSpatialDataSet(APIView):
    def get(self, request, *args, **kwargs):

        dataset_name_keys = {
            0: "ctry17nm",
            2: "lad18nm",
            3: "HBName",
            4: "HIAName",
        }

        input_serializer = ServiceAreaFullSpatialDataSetSearchSerializer(data=request.query_params)
        input_serializer.is_valid(raise_exception=True)
        input_data = input_serializer.validated_data
        area_type = input_data.get('type')
        service_area_features = return_feature(area_type, 0, True)
        queryset = list(service_area_features)
        result = {"name_key": dataset_name_keys.get(area_type), "data": queryset}
        return Response(result)
