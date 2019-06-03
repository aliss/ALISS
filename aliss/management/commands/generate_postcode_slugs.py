from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from aliss.search import filter_by_category
from django.db.models import F
from django.contrib import messages
from django.conf import settings
from django.urls import reverse

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-f', '--force', type=bool, help='Force regenerate slugs: -f 1',)

    def handle(self, *args, **options):
        self.force = options['force']

        if self.force:
            postcodes = Postcode.objects.all()
        else:
            postcodes = Postcode.objects.filter(slug='None').all()
        print("No. of postcode slugs to update: ", postcodes.count())
        for p in postcodes:
            p.generate_place_name_slug()
            p.save()
            print("Successfully updated all records.")