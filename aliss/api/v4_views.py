from rest_framework.response import Response
from . import views as v3
from aliss.models import Category, ServiceArea

from .serializers import (
    SearchSerializer,
    SearchInputSerializer,
    CategorySerializer,
    ServiceAreaSerializer
)

class APIv4():
    META = { 'licence': 'tbc' }


class SearchView(v3.SearchView):
    def get(self, request, *args, **kwargs):
        response = self.list(request, *args, **kwargs)
        data = {}
        data.update(APIv4.META)
        data.update(response.data)
        return Response(data)


class ServiceAreaListView(v3.ServiceAreaListView):
    def list(self, request):
        queryset = self.get_queryset()
        serializer = ServiceAreaSerializer(queryset, many=True)
        data = { 'results': serializer.data }
        data.update(APIv4.META)
        return Response(data)


class CategoryListView(v3.CategoryListView):
    def list(self, request):
        queryset = self.get_queryset()
        serializer = CategorySerializer(queryset, many=True)
        data = { 'results': serializer.data }
        data.update(APIv4.META)
        return Response(data)