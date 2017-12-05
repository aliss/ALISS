import uuid

from django.db import models


class Location(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    organisation = models.ForeignKey('aliss.Organisation', related_name='locations')

    name = models.CharField(max_length=100, blank=True)
    description = models.TextField(blank=True)

    street_address = models.CharField(max_length=100)
    locality = models.CharField(max_length=30)
    region = models.CharField(max_length=30, blank=True)
    state = models.CharField(max_length=30, blank=True)
    postal_code = models.CharField(max_length=10)
    country = models.CharField(max_length=2, default="GB")

    latitude = models.FloatField()
    longitude = models.FloatField()

    created_on = models.DateTimeField(auto_now_add=True)
    created_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='created_locations'
    )
    updated_on = models.DateTimeField(auto_now=True)
    updated_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='updated_locations',
        null=True
    )

    def __str__(self):
        return self.formatted_address

    @property
    def formatted_address(self):
        return ", ".join(filter(None, [self.name, self.street_address, self.locality, self.postal_code, self.state]))
