import uuid

from django.db import models
from django.urls import reverse
from django.dispatch import receiver
from django.utils.text import slugify

from aliss.models import ServiceArea
from elasticsearch_dsl import Search
from aliss.search import get_connection, service_to_body

from aliss.utils import unique_slug_generator

import pytz
from datetime import datetime, timedelta

class ServiceProblem(models.Model):
    UNRESOLVED = 0
    RESOLVED = 1

    STATUS_TYPES = (
        (UNRESOLVED, "Unresolved"),
        (RESOLVED, "Resolved")
    )

    INFORMATION_UNCLEAR = 0
    CONTACT_INFORMATION_INCORRECT = 10
    SERVICE_CLOSED = 20
    AREA_INCORRECT = 30

    REPORT_TYPES = (
        (INFORMATION_UNCLEAR, "Information provided is unclear"),
        (CONTACT_INFORMATION_INCORRECT, "Contact information incorrect or no response"),
        (SERVICE_CLOSED, "This service no longer exists"),
        (AREA_INCORRECT, "This service does not operate in my area")
    )

    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    service = models.ForeignKey('aliss.Service')
    user = models.ForeignKey('aliss.ALISSUser')

    type = models.IntegerField(choices=REPORT_TYPES)

    message = models.TextField(blank=True)

    status = models.IntegerField(
        choices=STATUS_TYPES,
        default=UNRESOLVED,
        blank=True
    )

    created_on = models.DateTimeField(auto_now_add=True)
    updated_on = models.DateTimeField(auto_now=True)


    def __str__(self):
        return "Problem with {0}".format(self.service)

    def get_absolute_url(self):
        return reverse('service_problem_detail', args=[str(self.id)])


class Service(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    organisation = models.ForeignKey('aliss.Organisation', related_name='services')
    slug = models.CharField(max_length=120, null=True, blank=True, default=None)
    name = models.CharField(max_length=100)
    description = models.TextField()
    url = models.URLField(blank=True)
    email = models.EmailField(blank=True)
    phone = models.CharField(max_length=15, blank=True)

    categories = models.ManyToManyField('aliss.Category', related_name='services')

    locations = models.ManyToManyField(
        'aliss.Location',
        blank=True,
        related_name='services'
    )
    service_areas = models.ManyToManyField(ServiceArea, blank=True, related_name='services')

    created_on = models.DateTimeField(auto_now_add=True)
    created_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='created_services',
        null=True,
        on_delete=models.SET_NULL
    )
    updated_on = models.DateTimeField(auto_now=True)
    updated_by = models.ForeignKey(
        'aliss.ALISSUser',
        related_name='updated_services',
        null=True,
        on_delete=models.SET_NULL
    )
    last_edited = models.DateTimeField(null=True, blank=True, default=None)

    @classmethod
    def published(cls):
        return cls.objects.filter(organisation__published=True)

    def is_published(self):
        return self.organisation.published

    def is_edited_by(self, user):
        if user == None or user.pk == None:
            return False
        elif user.is_staff:
            return True
        elif self.organisation.claimed_by == None:
            return (user.is_editor or (self.organisation.created_by == user))
        else:
            return self.organisation.claimed_by == user

    def generate_slug(self, force=False):
        name_changed = False
        if self.pk:
            result = Service.objects.filter(pk=self.pk).values('name').first()
            name_changed = (result != None) and (result != self.name)
        if force or self.slug == None or name_changed:
            self.slug = unique_slug_generator(self, 'name')
            return self.slug
        return False

    def update_index(self):
        connection = get_connection()
        if self.is_published():
            return connection.index(index='search', doc_type='service',
                id=self.id, body=service_to_body(self), refresh=True
            )
        else:
            return self.remove_from_index()

    def remove_from_index(self, connection=None):
        if connection == None:
            connection = get_connection()
        return connection.delete(index='search', doc_type='service',
            id=self.id, refresh=True, ignore=404
        )

    def generate_last_edited(self, force=False):
        if force or self.last_edited == None:
            if self.updated_on == None:
                self.update_last_edited()
            else:
                self.last_edited = self.updated_on
            return self.last_edited
        return False

    def update_last_edited(self):
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        self.last_edited = current_date

    def check_last_reviewed(self):
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        number_of_weeks = 6
        comparison_date = current_date - timedelta(weeks=number_of_weeks)
        if self.last_edited == None:
            return self.id
        elif self.last_edited < comparison_date:
            return self.id
        else:
            return None

    def save(self, *args, **kwargs):
        self.generate_slug()
        self.generate_last_edited()
        do_index = True
        if 'skip_index' in kwargs:
            do_index = False; kwargs.pop('skip_index')
        super(Service, self).save(*args)
        if do_index:
            self.update_index()

    def delete(self, *args, **kwargs):
        self.remove_from_index()
        super(Service, self).delete(*args, **kwargs)

    def generate_permalink(self):
        id = str(self.id)
        start_url = "www.aliss.org/services/"
        permalink = start_url + id + "/"
        return permalink



    @property
    def is_claimed(self):
        return self.organisation.is_claimed

    def __str__(self):
        return self.name
