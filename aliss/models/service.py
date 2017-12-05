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
        return self.name


class ServiceProblem(models.Model):
    UNRESOLVED = 0
    RESOLVED = 1

    STATUS_TYPES = (
        (UNRESOLVED, "Unresolved"),
        (RESOLVED, "Resolved")
    )

    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    service = models.ForeignKey('aliss.Service')
    email = models.EmailField()
    message = models.TextField()

    status = models.IntegerField(
        choices=STATUS_TYPES,
        default=UNRESOLVED,
        blank=True
    )

    def __str__(self):
        return "Problem with {0}".format(self.service)

    def get_absolute_url(self):
        return reverse('service_problem_detail', args=[str(self.id)])


class Service(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    organisation = models.ForeignKey('aliss.Organisation', related_name='services')
    program = models.ForeignKey('aliss.Program', null=True)

    name = models.CharField(max_length=100)
    description = models.TextField()
    url = models.URLField(blank=True)
    email = models.EmailField(blank=True)
    phone = models.CharField(max_length=15, blank=True)

    categories = models.ManyToManyField('aliss.Category')

    locations = models.ManyToManyField(
        'aliss.Location',
        blank=True,
        related_name='services'
    )
    service_areas = models.ManyToManyField(ServiceArea, blank=True, related_name='services')

    created_on = models.DateTimeField(auto_now_add=True)
    created_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='created_services'
    )
    updated_on = models.DateTimeField(auto_now=True)
    updated_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='updated_services',
        null=True
    )

    def __str__(self):
        return self.name
