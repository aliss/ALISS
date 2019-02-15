from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.db import models
from django.http import StreamingHttpResponse
from django.views.generic import View
import csv
from django.shortcuts import render
from urllib.request import Request, urlopen

class Command(BaseCommand):




    def write_collection_csv(self, collection, filepath, object_dict):

        def get_values(record, value_names):
            list_values = list(value_names)
            results = []    
            for value_element in list_values:
                if isinstance(value_element, list):
                    results.append(getattr(getattr(record, value_element[0]), value_element[1]))
                else:
                    results.append(getattr(record, value_element))
            return results


        fieldnames = object_dict.keys()
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(fieldnames)
            for record in collection:
                csv_writer.writerow(get_values(record, object_dict.values()))


    def write_service_csv(self, services, filepath='aliss_service.csv'):
        fieldnames = ["Name", "Description", "URL", "Phone Number",
        "Email Address", "URL", "Last Edited", "Organisation ID", "Organisation Name", "Organisation Categories", "Service Areas", "Locations"]
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(fieldnames)
            for service in services:
                csv_writer.writerow([getattr(service, "name"), service.description, service.url, service.phone, service.email, service.url, service.last_edited, service.organisation_id, service.organisation.name, service.categories, service.service_areas, service.locations])

    def write_location_csv(self, locations, filepath='aliss_location.csv'):
        fieldnames = ["ID", "Formatted Address", "Name", "Locality", "Region", "Postal Code", "Country", "Latitude", "Longitude", "Organisation Name", "Organisation ID", "Organisation Permalink"]
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(fieldnames)
            for location in locations:
                csv_writer.writerow([location.id, location.formatted_address, location.name,  location.locality, location.region, location.postal_code, location.country, location.latitude, location.longitude, location.organisation.name, location.organisation.id, "permalink"])

    def handle(self, *args, **options):

        service_dict = {
            "Name": "name",
            "Description": "description",
            "URL": "url",
            "Phone Number": "phone",
            "Email Address": "email",
            "Last Edited": "last_edited",
            "Organisation ID": "organisation_id",
            "Organisation Name": ["organisation", "name"],
            "Service Areas": "service_areas",
            "Locations": "locations"
        }

        self.stdout.write("\nWriting Services CSV\n")
        services = Service.objects.all()[:5]
        self.write_collection_csv(services, "aliss_service_result.csv", service_dict)


#name, description, url, phone, email, aliss_url, permalink, last_edited, organisation_id, organisation_name, organisation_permalink, categories, service_areas, location_ids
