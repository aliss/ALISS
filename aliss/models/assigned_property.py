import uuid
from django.db import models
from django.contrib.contenttypes.fields import GenericForeignKey
from django.contrib.contenttypes.models import ContentType
from django.core.exceptions import ValidationError
from django.utils.translation import ugettext_lazy as _


class AssignedProperty(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    property_definition = models.ForeignKey('aliss.Property', related_name='assigned_properties')
    description  = models.CharField(max_length=140)
    content_type = models.ForeignKey(ContentType, on_delete=models.CASCADE)
    object_id = models.UUIDField(primary_key=False)
    holder    = GenericForeignKey('content_type', 'object_id')

    def clean(self):
        try:
            idx = ["Organisation", "Service", "Location"].index(self.holder.__class__.__name__)
        except Exception as e:
            idx = 3

        applicable = [
            self.property_definition.for_organisations,
            self.property_definition.for_services,
            self.property_definition.for_locations,
            False
        ]

        if not applicable[idx]:
            raise ValidationError(
                _('Property definition is not appropriate for content_type %(value)s'),
                params={'value': self.holder.__class__.__name__},
            )

    @property
    def name(self):
        return self.property_definition.name

    def __str__(self):
        return self.name
