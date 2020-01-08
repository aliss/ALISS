import uuid
from django.db import models


class Property(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    icon = models.CharField(max_length=100, default="fa fa-square")
    name = models.CharField(max_length=100, unique=True)
    description = models.CharField(max_length=140, null=False, blank=False)
    for_organisations = models.BooleanField(default=True)
    for_services      = models.BooleanField(default=True)
    for_locations     = models.BooleanField(default=True)

    def __str__(self):
        return self.name
