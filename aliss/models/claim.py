import uuid

from django.db import IntegrityError, models, transaction
from django.utils import timezone
from django.conf import settings
from django.core.validators import RegexValidator



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
    name = models.CharField(max_length=100, default=' ')
    organisation = models.ForeignKey('aliss.Organisation')
    phone = models.CharField(max_length=30, default="", validators=[RegexValidator(regex='^.{14}$', message='Length has to be 11', code='nomatch')]);
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