from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.db import models
from django.http import StreamingHttpResponse
from django.views.generic import View
import csv
from django.shortcuts import render
from urllib.request import Request, urlopen

class Command(BaseCommand):

    def write_service_csv(self, services, filepath='aliss_service.csv'):
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(["Name", "Description", "URL", "Phone Number",
            "Email Address", "URL", "Last Edited", "Organisation ID", "Organisation Name", "Organisation Categories", "Service Areas", "Locations"])
            for service in services:
                csv_writer.writerow([service.name, service.description, service.url, service.phone, service.email, service.url, service.last_edited, service.organisation_id, service.organisation.name, service.categories, service.service_areas, service.locations])


    def handle(self, *args, **options):
        self.stdout.write("\nWriting Services CSV\n")
        services = Service.objects.all()[:5]
        self.write_service_csv(services)


#name, description, url, phone, email, aliss_url, permalink, last_edited, organisation_id, organisation_name, organisation_permalink, categories, service_areas, location_ids
