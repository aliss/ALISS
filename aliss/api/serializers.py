from rest_framework import serializers
from django.urls import reverse
from aliss.models import Postcode, Category, ServiceArea


class SearchInputSerializer(serializers.Serializer):
    LOCAL = 'local'
    NATIONAL = 'national'
    LOCATION_TYPE_CHOICES = (
        (LOCAL, LOCAL),
        (NATIONAL, NATIONAL)
    )

    q = serializers.CharField(required=False)
    postcode = serializers.SlugRelatedField(
        required=True,
        slug_field='postcode',
        queryset=Postcode.objects.all()
    )

    category = serializers.SlugRelatedField(
        required=False,
        many=False,
        slug_field='slug',
        queryset=Category.objects.all()
    )

    location_type = serializers.ChoiceField(
        choices=LOCATION_TYPE_CHOICES,
        required=False
    )

    radius = serializers.IntegerField(default=5000)


class OrganisationSerializer(serializers.Serializer):
    id = serializers.UUIDField()
    name = serializers.CharField()
    aliss_url = serializers.SerializerMethodField()
    permalink = serializers.SerializerMethodField()
    is_claimed = serializers.BooleanField()
    slug = serializers.CharField()

    def get_aliss_url(self, obj):
        return self.context['request'].build_absolute_uri(reverse('organisation_detail_slug', args=[obj.slug]))

    def get_permalink(self, obj):
        return self.context['request'].build_absolute_uri(reverse('organisation_detail', args=[obj.id]))


class CategorySearchSerializer(serializers.Serializer):
    name = serializers.CharField()
    slug = serializers.SlugField()


class CategorySerializer(serializers.Serializer):
    id = serializers.IntegerField()
    name = serializers.CharField()
    slug = serializers.SlugField()
    parent = serializers.PrimaryKeyRelatedField(read_only=True)


class BaseLocationSerializer(serializers.Serializer):
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


class SearchLocationSerializer(BaseLocationSerializer):
    #serializes location from elasticsearch
    latitude = serializers.FloatField(source='point.lat')
    longitude = serializers.FloatField(source='point.lon')


class LocationSerializer(BaseLocationSerializer):
    #serializes location from db
    latitude = serializers.FloatField()
    longitude = serializers.FloatField()


class ServiceAreaSerializer(serializers.Serializer):
    code = serializers.CharField()
    type = serializers.CharField()
    name = serializers.CharField()


class v4ServiceAreaSerializer(ServiceAreaSerializer):
    type = serializers.SerializerMethodField()

    def get_type(self, obj):
        return obj.type_name


class BaseServiceSerializer(serializers.Serializer):
    id = serializers.UUIDField()
    name = serializers.CharField()
    description = serializers.CharField()
    url = serializers.URLField(required=False)
    phone = serializers.CharField(required=False)
    email = serializers.CharField(required=False)
    categories = CategorySearchSerializer(many=True, required=False)
    service_areas = ServiceAreaSerializer(many=True, required=False)


class SearchSerializer(BaseServiceSerializer):
    locations = SearchLocationSerializer(many=True, required=False)
    organisation = OrganisationSerializer()


class v4OrganisationDetailSerializer(OrganisationSerializer):
    description = serializers.CharField()
    facebook = serializers.URLField()
    twitter  = serializers.URLField()
    url = serializers.URLField()
    phone = serializers.CharField()
    email = serializers.CharField()
    last_edited = serializers.DateTimeField()
    services = BaseServiceSerializer(many=True)
    locations = LocationSerializer(many=True)


class v4ServiceSerializer(BaseServiceSerializer):
    organisation = OrganisationSerializer()
    slug         = serializers.CharField()
    aliss_url    = serializers.SerializerMethodField()
    permalink    = serializers.SerializerMethodField()
    last_updated = serializers.SerializerMethodField()
    locations = LocationSerializer(many=True)

    def get_aliss_url(self, obj):
        return self.context['request'].build_absolute_uri(reverse('service_detail_slug', args=[obj.slug]))

    def get_permalink(self, obj):
        return self.context['request'].build_absolute_uri(reverse('service_detail', args=[obj.id]))

    def get_last_updated(self, obj):
        return obj.last_edited


class v4SearchSerializer(v4ServiceSerializer):
    organisation = OrganisationSerializer()
    locations = SearchLocationSerializer(many=True, required=False)


class RecursiveField(serializers.Serializer):
    def to_representation(self, value):
        serializer = self.parent.parent.__class__(value, context=self.context)
        return serializer.data


class v4CategorySerializer(serializers.Serializer):
    name = serializers.CharField()
    slug = serializers.SlugField()
    sub_categories = RecursiveField(many=True, source='children')


class PostcodeLocationSerializer(serializers.Serializer):
    place_name = serializers.CharField()
    postcode = serializers.CharField()


class PostcodeLocationSearchSerializer(serializers.Serializer):
    q = serializers.CharField(required=False)


class ServiceAreaSpatialSearchSerializer(serializers.Serializer):
    service_id = serializers.UUIDField(required=True)
