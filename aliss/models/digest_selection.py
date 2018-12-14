import uuid

from django.db import models
from django.utils import timezone
from django.conf import settings

# Import the necessary search codes
from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections
from aliss.search import filter_by_query, filter_by_postcode, sort_by_postcode,filter_by_location_type, filter_by_category, filter_by_last_edited, filter_by_created_on

# Datetime
from datetime import datetime
from datetime import timedelta
import pytz

from aliss.models import *

class DigestSelection(models.Model):

    user = models.ForeignKey('aliss.ALISSUser', related_name='digest_selections')
    category = models.ForeignKey('aliss.Category', related_name='digest_selections', blank=True, null=True)
    postcode = models.ForeignKey('aliss.Postcode')

    def __str__(self):
        return self.user.name + " " + self.postcode.postcode

    def retrieve_updated_services(self, comparison_date):

        # Create connection to elastic search
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))

        queryset = Search(index='search', doc_type='service')
        queryset = filter_by_postcode(queryset, self.postcode)
        if (self.category):
            queryset = filter_by_category(queryset, self.category)
        queryset = queryset.sort({ "last_edited" : {"order" : "desc"}})
        comparison_date_string = comparison_date.strftime("%Y-%m-%d"'T'"%H:%M:%S")
        queryset = filter_by_last_edited(queryset, comparison_date_string)

        return queryset.execute()

    def retrieve_new_services(self, comparison_date):

        # Create connection to elastic search
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))

        queryset = Search(index='search', doc_type='service')
        queryset = filter_by_postcode(queryset, self.postcode)
        if (self.category):
            queryset = filter_by_category(queryset, self.category)
        queryset = queryset.sort({ "created_on" : {"order" : "desc"}})
        comparison_date_string = comparison_date.strftime("%Y-%m-%d"'T'"%H:%M:%S")
        queryset = filter_by_created_on(queryset, comparison_date_string)

        return queryset.execute()

    def create_digest_selection(self, postcode_string, category_slug, user_email):

        # Retrieve the category, postcode and user objects
        category_object = Category.objects.get(slug=category_slug)
        postcode_object = Postcode.objects.get(postcode=postcode_string)
        user_object = ALISSUser.objects.get(email=user_email)

        # Create new DigestSelection record
        DigestSelection.objects.create(postcode=postcode_object, category=category_object, user=user_object)
