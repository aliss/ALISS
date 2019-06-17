import uuid

from django.db import models

class ContentBlock(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    slug = models.SlugField()
    body = models.TextField()
