from rest_framework import serializers

from aliss.models import Postcode, Category, ServiceArea


class SearchInputSerializer(serializers.Serializer):
    query = serializers.CharField(required=False)
    postcode = serializers.SlugRelatedField(
        required=True,
        slug_field='postcode',
        queryset=Postcode.objects.all()
    )

    categories = serializers.SlugRelatedField(
        required=False,
        many=True,
        slug_field='slug',
        queryset=Category.objects.all()
    )
    service_areas = serializers.SlugRelatedField(
        required=False,
        many=True,
        slug_field='code',
        queryset=ServiceArea.objects.all()
    )

    radius = serializers.IntegerField(default=5000)


class OrganisationSerializer(serializers.Serializer):
    id = serializers.UUIDField()
    name = serializers.CharField()


class CategorySerializer(serializers.Serializer):
    name = serializers.CharField()
    slug = serializers.SlugField()


class LocationSerializer(serializers.Serializer):
    id = serializers.UUIDField()
    formatted_address = serializers.CharField()
    name = serializers.CharField()
    description = serializers.CharField()
    street_address = serializers.CharField()
    locality = serializers.CharField()
    region = serializers.CharField()
    state = serializers.CharField()
    postal_code = serializers.CharField()
    country = serializers.CharField()
    latitude = serializers.FloatField(source='point.lat')
    longitude = serializers.FloatField(source='point.lon')


class ServiceAreaSerializer(serializers.Serializer):
    code = serializers.CharField()
    type = serializers.CharField(source='get_type_display')
    name = serializers.CharField()


class SearchSerializer(serializers.Serializer):
    id = serializers.UUIDField()
    organisation = OrganisationSerializer()
    name = serializers.CharField()
    description = serializers.CharField()
    url = serializers.URLField(required=False)
    phone = serializers.CharField(required=False)
    email = serializers.CharField(required=False)
    categories = CategorySerializer(many=True, required=False)
    locations = LocationSerializer(many=True, required=False)
    service_areas = ServiceAreaSerializer(many=True, required=False)
