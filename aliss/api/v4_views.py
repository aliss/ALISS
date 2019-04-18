from rest_framework.views import APIView
from rest_framework.response import Response
from rest_framework import generics
from . import views as v3
from aliss.models import Category, ServiceArea, Organisation, Service, Postcode
from collections import OrderedDict
from copy import deepcopy
from django.shortcuts import get_object_or_404

from .serializers import (
    v4SearchSerializer,
    SearchInputSerializer,
    v4CategorySerializer,
    v4ServiceAreaSerializer,
    v4OrganisationDetailSerializer,
    v4ServiceSerializer,
    PostcodeLocationSerializer
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


'''
Need to create a new api endpoint which will be queried with three characters which will then return json holding matching postcodes. Look to categories for inspiration.
'''

class PostcodeLocationData(generics.ListAPIView):
    def get(self, request, q=None):
        queryset = Postcode.objects.exclude(place_name=None)
        import logging
        logger = logging.getLogger(__name__)
        logger.error(str(q))
        if q == None:
            queryset = queryset.filter(place_name__startswith="Por")
        serializer = PostcodeLocationSerializer(queryset, many=True)
        data = OrderedDict()
        data['data'] = serializer.data
        return Response(data)
