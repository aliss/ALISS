import uuid

from django.db import models
from django.dispatch import receiver
from django.utils.text import slugify
from aliss.models import ALISSCloudinaryField

import pytz
from datetime import datetime

class Organisation(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    name = models.CharField(max_length=100)
    description = models.TextField()
    phone = models.CharField(max_length=15, blank=True)
    email = models.EmailField(blank=True)
    url      = models.URLField(blank=True, verbose_name="Web address")
    facebook = models.URLField(blank=True)
    twitter  = models.URLField(blank=True)
    slug     = models.CharField(max_length=120, null=True, blank=True, default=None)
    logo     = ALISSCloudinaryField('image', null=True, blank=True)

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

    last_edited = models.DateTimeField(null=True, blank=True, default=None)
    published = models.BooleanField(default=True)

    def is_edited_by(self, user):
        if user == None or user.pk == None:
            return False
        return (
            user.is_staff or \
            user.is_editor or \
            self.created_by == user or \
            self.claimed_by == user
        )

    def can_add_logo(self, user):
        if user == None or user.pk == None:
            return False
        return (
            user.is_staff or \
            user.is_editor or \
            self.claimed_by == user
        )

    def generate_slug(self, force=False):
        if force or self.slug == None:
            s = slugify(self.name)
            sCount = Organisation.objects.filter(slug__icontains=s).count()
            self.slug = s + "-" + str(sCount)
            return self.slug
        return False

    def generate_last_edited(self, force=False):
        if  force or self.last_edited == None:
            if self.updated_on == None:
                self.update_organisation_last_edited()
            else:
                self.last_edited = self.updated_on
            return self.last_edited
        return False

    def update_organisation_last_edited(self):
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        self.last_edited = current_date

    def save(self, *args, **kwargs):
        self.generate_slug()
        self.generate_last_edited()
        super(Organisation, self).save(*args, **kwargs)
        for s in self.services.all():
            s.add_to_index()

    def delete(self, *args, **kwargs):
        for s in self.services.all():
            s.remove_from_index()
        super(Organisation, self).delete(*args, **kwargs)

    @property
    def is_claimed(self):
        return not (self.claimed_by == None)

    def __str__(self):
        return self.name
