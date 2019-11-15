import uuid
from django.db import models
from aliss.models import ServiceArea
from django.db.models import Avg
from django.utils.text import slugify
from django.contrib.gis.geos import Point
import logging

class Postcode(models.Model):
    postcode = models.CharField(primary_key=True, max_length=9)
    postcode_district = models.CharField(max_length=4)
    postcode_sector = models.TextField(max_length=6)
    latitude = models.FloatField()
    longitude = models.FloatField()
    council_area_2011_code = models.CharField(max_length=10)
    health_board_area_2014_code = models.CharField(max_length=10)
    integration_authority_2016_code = models.CharField(max_length=10)
    place_name = models.CharField(max_length=100, blank=True, null=True, default=None)
    slug = models.SlugField(default='None')

    def __str__(self):
        s = self.postcode
        if self.place_name:
            s = s + ", " + self.place_name
        return s

    @property
    def codes(self):
        return [
            self.council_area_2011_code,
            self.health_board_area_2014_code,
            self.integration_authority_2016_code,
            'XS',
            'XB'
        ]

    @property
    def service_areas(self):
        return ServiceArea.objects.filter(code__in=self.codes)

    def get_local_authority(self):
        return ServiceArea.objects.filter(type=2, code=self.council_area_2011_code).first()

    def get_by_district(district):
        try:
            lng_avg = Postcode.objects.filter(postcode_district=district).aggregate(Avg('longitude'))
            lat_avg = Postcode.objects.filter(postcode_district=district).aggregate(Avg('latitude'))

            centroid = Point(lng_avg['longitude__avg'], lat_avg['latitude__avg'])
            postcodes_in_district_qs = Postcode.objects.filter(
                postcode_district=district)

            lng_gte_lat_gte = postcodes_in_district_qs.filter(
                longitude__gte=lng_avg['longitude__avg'],
                latitude__gte=lat_avg['latitude__avg']
            ).order_by('longitude', 'latitude').first()

            lng_lte_lat_lte = postcodes_in_district_qs.filter(
                longitude__lte=lng_avg['longitude__avg'],
                latitude__lte=lat_avg['latitude__avg']
            ).order_by('-longitude', '-latitude').first()

            lng_gte_lat_lte = postcodes_in_district_qs.filter(
                longitude__gte=lng_avg['longitude__avg'],
                latitude__lte=lat_avg['latitude__avg']
            ).order_by('longitude', '-latitude').first()

            lng_lte_lat_gte = postcodes_in_district_qs.filter(
                longitude__lte=lng_avg['longitude__avg'],
                latitude__gte=lat_avg['latitude__avg']
            ).order_by('-longitude', 'latitude').first()

            postcodes_and_distances = []
            lng_lat_quadrants = [lng_gte_lat_gte, lng_lte_lat_lte, lng_gte_lat_lte, lng_lte_lat_gte]

            for quadrant in lng_lat_quadrants:
                if quadrant is not None:
                    postcodes_and_distances.append([quadrant, centroid.distance(Point(quadrant.longitude, quadrant.latitude))])

            postcodes_and_distances.sort(key=lambda x:x[1])
            return postcodes_and_distances[0][0]

        except:
            raise Postcode.DoesNotExist("%s matching query does not exist." % Postcode._meta.object_name)

    def generate_place_name_slug(self):
        if self.place_name:
            s = slugify(self.place_name)
            if self.slug != s:
                self.slug = s
        else:
            s = slugify(self.postcode)
            if self.slug != s:
                self.slug = s
        return False


    def save(self, *args, **kwargs):
        self.generate_place_name_slug()
        super(Postcode, self).save(*args, **kwargs)
