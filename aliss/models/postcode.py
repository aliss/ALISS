import uuid
from django.db import models
from aliss.models import ServiceArea
from django.db.models import Avg

class Postcode(models.Model):
    postcode = models.CharField(primary_key=True, max_length=9)
    postcode_district = models.CharField(max_length=4)
    postcode_sector = models.TextField(max_length=4)
    latitude = models.FloatField()
    longitude = models.FloatField()
    council_area_2011_code = models.CharField(max_length=10)
    health_board_area_2014_code = models.CharField(max_length=10)
    integration_authority_2016_code = models.CharField(max_length=10)

    def __str__(self):
        return self.postcode

    @property
    def codes(self):
        return [
            self.council_area_2011_code,
            self.health_board_area_2014_code,
            self.integration_authority_2016_code,
            'XS',
            'XB'
        ]

    def get_local_authority(self):
        return ServiceArea.objects.filter(type=2, code=self.council_area_2011_code).first()

    def get_by_district(district):
        try:
            lng_avg = Postcode.objects.filter(postcode_district=district).aggregate(Avg('longitude'))
            lat_avg = Postcode.objects.filter(postcode_district=district).aggregate(Avg('latitude'))
            #TODO: avg between lng and lat ordering
            return Postcode.objects.filter(
                postcode_district=district,
                longitude__lte=lng_avg['longitude__avg'],
                latitude__lte=lat_avg['latitude__avg']
            ).order_by('-longitude', '-latitude').first()
        except:
            raise Postcode.DoesNotExist("%s matching query does not exist." % Postcode._meta.object_name)
