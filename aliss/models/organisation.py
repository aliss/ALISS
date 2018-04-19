import uuid

from django.db import models
from django.dispatch import receiver


class Organisation(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    name = models.CharField(max_length=100)
    description = models.TextField()
    phone = models.CharField(max_length=15, blank=True)
    email = models.EmailField(blank=True)
    url = models.URLField(blank=True)
    facebook = models.URLField(blank=True)
    twitter = models.URLField(blank=True)

    claimed_by = models.ForeignKey(
        'aliss.ALISSUser',
        null=True,
        on_delete=models.SET_NULL
    )
    created_on = models.DateTimeField(auto_now_add=True)
    created_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='created_organisations',
        null=True,
        on_delete=models.SET_NULL
    )
    updated_on = models.DateTimeField(auto_now=True)
    updated_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='updated_organisations',
        null=True,
        on_delete=models.SET_NULL
    )

    published = models.BooleanField(default=True)

    def is_edited_by(self, user):
        if user == None:
            return False
        return (
            user.is_staff or \
            user.is_editor or \
            self.created_by == user or \
            self.claimed_by == user
        )

    def __str__(self):
        return self.name
