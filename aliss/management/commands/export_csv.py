from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.db import models
from django.http import StreamingHttpResponse
from django.views.generic import View
import csv
from django.shortcuts import render
from urllib.request import Request, urlopen

class Command(BaseCommand):

    def write_result_csv(self, collection, filepath='aliss_service.csv'):
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(["Name", "Description", "URL", "Phone Number",
            "Email Address", "URL", "Last Edited", "Organisation ID", "Organisation Name", "Organisation Categories", "Service Areas", "Locations"])
            for obj in collection:
                csv_writer.writerow([obj.name, obj.description, obj.url, obj.phone, obj.email, obj.url, obj.last_edited, obj.organisation_id, obj.organisation.name, obj.categories, obj.service_areas, obj.locations])


    def handle(self, *args, **options):
        self.stdout.write("\nWriting CSV\n")
        results = Service.objects.all()[:5]
        self.write_result_csv(results)


#name, description, url, phone, email, aliss_url, permalink, last_edited, organisation_id, organisation_name, organisation_permalink, categories, service_areas, location_ids
