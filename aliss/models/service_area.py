import uuid

from django.db import models
from django.urls import reverse
from django.dispatch import receiver

class ServiceArea(models.Model):
    COUNTRY = 0
    REGION = 1
    LOCAL_AUTHORITY = 2
    HEALTH_BOARD = 3
    INTEGRATION_AUTHORITY = 4

    AREA_TYPES = (
        (COUNTRY, "Country"),
        (REGION, "Region"),
        (LOCAL_AUTHORITY, "Local Authority"),
        (HEALTH_BOARD, "Health Board"),
        (INTEGRATION_AUTHORITY, "Integration Authority (HSCP)")
    )

    name = models.CharField(max_length=100)
    alternative_name = models.CharField(max_length=100, blank=True)
    code = models.CharField(max_length=9)
    type = models.IntegerField(choices=AREA_TYPES, blank=True)

    def __str__(self):
        return self.name +" ("+ str(ServiceArea.AREA_TYPES[self.type][1]) + ")"
