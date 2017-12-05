import uuid

from django.db import models
from django.utils import timezone
from django.conf import settings


class Claim(models.Model):
    UNREVIEWED = 0
    CONFIRMED = 10
    DENIED = 20

    STATUS_CHOICES = (
        (UNREVIEWED, "Un-reviewed"),
        (CONFIRMED, "Confirmed"),
        (DENIED, "Denied")
    )

    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    user = models.ForeignKey('aliss.ALISSUser')
    organisation = models.ForeignKey('aliss.Organisation')

    email = models.EmailField()
    phone = models.CharField(max_length=30)
    comment = models.TextField()
    status = models.IntegerField(choices=STATUS_CHOICES, default=UNREVIEWED)

    created_on = models.DateTimeField(default=timezone.now, editable=False)
    created_by = models.ForeignKey(
        settings.AUTH_USER_MODEL,
        related_name='claims',
        editable=False
    )

    reviewed_on = models.DateTimeField(default=timezone.now, editable=False)
    reviewed_by = models.ForeignKey(
        settings.AUTH_USER_MODEL,
        related_name='reviewed_claims',
        editable=False,
        null=True
    )
