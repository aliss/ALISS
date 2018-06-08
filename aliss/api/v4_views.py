from rest_framework.response import Response
from . import views as v3
from aliss.models import Category, ServiceArea
from collections import OrderedDict
from copy import deepcopy

from .serializers import (
    v4SearchSerializer,
    SearchInputSerializer,
    v4CategorySerializer,
    v4ServiceAreaSerializer
)

class APIv4():
    META = {
        'licence': 'https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode',
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
        data = OrderedDict()
        data['meta'] = deepcopy(APIv4.META)
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