# import uuid

from django.db import models
from django.utils import timezone
from django.conf import settings

class DigestSelection(models.Model):
    # id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    user = models.ForeignKey('aliss.ALISSUser')
    categories = models.ManyToManyField('aliss.Category', related_name='digest_selections')
    postcode = models.ForeignKey('aliss.Postcode')
