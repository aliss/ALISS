import uuid
from django.db import models
from aliss.models import ServiceArea
from django.db.models import Avg
from django.utils.text import slugify

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
            #TODO: avg between lng and lat ordering
            # lte_postcodes = Postcode.objects.filter(
            #     postcode_district=district,
            #     longitude__lte=lng_avg['longitude__avg'],
            #     latitude__lte=lat_avg['latitude__avg']
            # ).order_by('-longitude', '-latitude')
            # if (lte_postcodes.count() > 0):
            #     return lte_postcodes.first()
            # else:
            #     gte_postcodes = Postcode.objects.filter(
            #         postcode_district=district,
            #         longitude__gte=lng_avg['longitude__avg'],
            #         latitude__gte=lat_avg['latitude__avg']
            #     ).order_by('longitude', 'latitude')
            #     return gte_postcodes.first()
            return Postcode.objects.filter(
                postcode_district=district,
                longitude__gte=lng_avg['longitude__avg'],
                latitude__lte=lat_avg['latitude__avg']
            ).order_by('-longitude', '-latitude').first()
        except:
            raise self.model.DoesNotExist("%s matching query does not exist." % self.model._meta.object_name)

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
