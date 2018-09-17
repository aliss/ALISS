import uuid

from django.db import models
from django.dispatch import receiver
from django.utils.text import slugify


class Organisation(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)
    name = models.CharField(max_length=100)
    description = models.TextField()
    phone = models.CharField(max_length=15, blank=True)
    email = models.EmailField(blank=True)
    url = models.URLField(blank=True)
    facebook = models.URLField(blank=True)
    twitter  = models.URLField(blank=True)
    slug     = models.CharField(max_length=120, null=True, blank=True, default=None)

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
        if user == None or user.pk == None:
            return False
        return (
            user.is_staff or \
            user.is_editor or \
            self.created_by == user or \
            self.claimed_by == user
        )

    def generate_slug(self, force=False):
        if force or self.slug == None:
            s = slugify(self.name)
            sCount = Organisation.objects.filter(slug__icontains=s).count()
            self.slug = s + "-" + str(sCount)
            return self.slug
        return False

    def save(self, *args, **kwargs):
        self.generate_slug()
        super(Organisation, self).save(*args, **kwargs)

    @property
    def is_claimed(self):
        return not (self.claimed_by == None)

    def __str__(self):
        return self.name
