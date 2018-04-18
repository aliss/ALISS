import uuid

from django.db import models
from django.utils import timezone
from django.conf import settings


class Claim(models.Model):
    UNREVIEWED = 0
    CONFIRMED = 10
    DENIED = 20
    REVOKED = 30

    STATUS_CHOICES = (
        (UNREVIEWED, "Un-reviewed"),
        (CONFIRMED, "Confirmed"),
        (DENIED, "Denied"),
        (REVOKED, "Revoked")
    )

    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    user = models.ForeignKey('aliss.ALISSUser')
    organisation = models.ForeignKey('aliss.Organisation')

    email = models.EmailField()
    phone = models.CharField(max_length=30)
    comment = models.TextField()
    status = models.IntegerField(choices=STATUS_CHOICES, default=UNREVIEWED)

    created_on = models.DateTimeField(default=timezone.now, editable=False)

    reviewed_on = models.DateTimeField(default=timezone.now, editable=False)
    reviewed_by = models.ForeignKey(
        settings.AUTH_USER_MODEL,
        related_name='reviewed_claims',
        editable=False,
        null=True,
        on_delete=models.SET_NULL
    )

    def quit_representation(self):
        o = self.organisation
        try:
            o.claimed_by=None
            o.save()
            self.delete()
            return True
        except IntegrityError:
            transaction.rollback()
            return False