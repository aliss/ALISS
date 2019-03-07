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

        def generate_permalink(record, model):
            id = str(record.id)
            start_url = "www.aliss.org/" + model + "/"
            permalink = start_url + id + "/"
            return permalink

        def get_value(record, key):
            return getattr(record, key, "")

        def get_nested_value(record, keys):
            object = get_value(record, keys[0])
            value = get_value(object, keys[1])
            return value

        def get_nested_value_list(record, keys):
            values = ""
            objects_list = get_value(record, keys[0])
            for object in objects_list.all():
                values += "\"" + str(get_value(object, keys[1])) + "\" "
            return values

        def get_nested_permalink(record, keys):
            object = get_value(record, keys[0])
            nested_permalink = generate_permalink(object, keys[1])
            return nested_permalink

        def get_nested_permalinks_list(record, keys):
            values = ""
            objects_list = get_value(record, keys[0])
            for object in objects_list.all():
                values += "\"" + generate_permalink(object, keys[0]) + "\" "
            return values

        def generate_aliss_url(record, model):
            aliss_url = ""
            if record.slug:
                slug = str(record.slug)
                start_url = "www.aliss.org/" + model + "/"
                aliss_url = start_url + slug + "/"
            return aliss_url

        def write_row_per_record(csv_writer, collection):
            for record in collection:
                csv_writer.writerow(get_values_dict(record, object_dict.values()))

        def write_row_per_nexted(csv_writer, collection):
            for location in collection:
                for service in location.services.all():
                    csv_writer.writerow([service.id, location.id, service.name, "www.aliss.org/services/" + str(service.id), location.formatted_address, service.organisation_id])

        def get_values_dict(record, value_names):
            list_values = list(value_names)
            results = []
            for value_element in list_values:
                if isinstance(value_element, list):
                    if "list" in value_element:
                        results.append(get_nested_value_list(record, value_element))
                    elif "permalink" in value_element:
                        results.append(generate_permalink(record, value_element[0]))
                    elif "nested_url" in value_element:
                        results.append(get_nested_permalinks_list(record, value_element))
                    elif "aliss_url" in value_element:
                        results.append(generate_aliss_url(record, value_element[0]))
                    elif "nested_permalink" in value_element:
                        results.append(get_nested_permalink(record, value_element))
                    else:
                        results.append(get_nested_value(record, value_element))
                else:
                    results.append(get_value(record, value_element))
            return results


        fieldnames = object_dict.keys()
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(fieldnames)
            for record in collection:
                csv_writer.writerow(get_values_dict(record, object_dict.values()))

    def write_joining_table_csv(self, collection, filepath, object_dict):
        #Need to get the collection of locations and for every service that the location has write a new row in the
        fieldnames = object_dict.keys()
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_ALL)
            csv_writer.writerow(fieldnames)
            for location in collection:
                for service in location.services.all():
                    csv_writer.writerow([service.id, location.id, service.name, "www.aliss.org/services/" + str(service.id), location.formatted_address, service.organisation_id])

        '''
        you need to loop through each service for each location
        and each of those is one row
        so for each location
        loop through services and add a row
        then go on to next location
        is that what you meant?

        "service_id": ["service", "id"],
        "location_id": ["location", "id"],
        "service_name": ["service", "name"],
        "service_permalink": ["service", "permalink"],
        "formatted_address": "formatted_address",
        "organisation_id": "organisation_id",
        '''





    def handle(self, *args, **options):

        service_dict = {
            "id": "id",
            "name": "name",
            "description": "description",
            "url": "url",
            "phone_number": "phone",
            "email_address": "email",
            "aliss_url": ["services", "aliss_url"],
            "permalink": ["services", "permalink"],
            "last_edited": "last_edited",
            "organisation_id": "organisation_id",
            "organisation_name": ["organisation", "name"],
            "orgasnisation_permalink": ["organisation", "organisations", "nested_permalink"],
            "categories": ["categories", "name", "list"],
            "service_areas": "service_areas",
            "locations_ids": ["locations", "id", "list"],
        }

        #id, name, description, url, phone, email, aliss_url, permalink, last_edited, organisation_id, organisation_name, organisation_permalink, categories, service_areas, location_ids

        location_dict = {
            "id": "id",
            "formatted_address": "formatted_address",
            "name": "name",
            "locality": "locality",
            "region": "region",
            "postal_code": "postal_code",
            "country": "country",
            "latitude": "latitude",
            "longitude": "longitude",
            "organisation_name": ["organisation", "name"],
            "organisation_id": "organisation_id",
            "organisation_permalink": ["organisation", "organisations", "nested_permalink"],
        }

        #id, formatted_address, name, locality, region, postal_code, country, latitude, longitude, organisation_name, organisation_id, organisation_permalink

        organisation_dict = {
            "id": "id",
            "name": "name",
            "description": "description",
            "aliss_url": ["organisations", "aliss_url"],
            "permalink": ["organisations", "permalink"],
            "url": "url",
            "twitter": "twitter",
            "facebook": "facebook",
            "phone": "phone",
            "email": "email",
            "last_edited": "last_edited",
            "service_names": ["services", "name", "list"],
            "service_ids": ["services", "id", "list"],
        }

        #id, name, description, aliss_url, permalink, url, twitter, facebook, phone, email, last_edited, service_names, service_ids, service_permalinks

        services_at_location_dict = {
            "service_id": ["service", "id"],
            "location_id": ["location", "id"],
            "service_name": ["service", "name"],
            "service_permalink": ["service", "permalink"],
            "formatted_address": "formatted_address",
            "organisation_id": "organisation_id",
        }

        #service_id, location_id, service_name, service_permalink, formatted_address, organisation_id

        def check_record_count(collection, filepath, model):
            db_count = collection.count()
            row_count = -1
            with open(filepath, 'r+') as data_file:
                data = csv.reader(data_file, delimiter=',')
                for row in data:
                    row_count += 1
            if db_count == row_count:
                self.stdout.write("- Successfully wrote " + str(row_count) + " rows for " + str(model) + "\n")
            else:
                self.stdout.write("- Incorrect number or rows written for " + model + "\n")
                self.stdout.write("- " + str(db_count) + " does not equal " + str(row_count))

        def check_services_at_location_count(collection, filepath, model):
            db_count = 0
            for location in collection:
                for service in location.services.all():
                    db_count += 1
            row_count = -1
            with open(filepath, 'r+') as data_file:
                data = csv.reader(data_file, delimiter=',')
                for row in data:
                    row_count += 1
            if db_count == row_count:
                self.stdout.write("- Successfully wrote " + str(row_count) + " rows for " + str(model) + "\n")
            else:
                self.stdout.write("- Incorrect number or rows written for " + model + "\n")
                self.stdout.write("- " + str(db_count) + " does not equal " + str(row_count))

        self.stdout.write("\nWriting Services CSV\n")
        services_collection = Service.objects.all()
        self.write_collection_csv(services_collection, "aliss_service_result.csv", service_dict)
        check_record_count(services_collection, "aliss_service_result.csv", "Service")

        self.stdout.write("\nWriting Locations CSV\n")
        locations_collection = Location.objects.all()
        self.write_collection_csv(locations_collection, "aliss_location_result.csv", location_dict)
        check_record_count(locations_collection, "aliss_location_result.csv", "Location")

        self.stdout.write("\nWriting Organisations CSV\n")
        organisations_collection = Organisation.objects.all()
        self.write_collection_csv(organisations_collection, "aliss_organisation_result.csv", organisation_dict)
        check_record_count(organisations_collection,"aliss_organisation_result.csv", "Organisation")

        self.stdout.write("\nWriting Services at Location CSV\n")
        services_at_location_collection = Location.objects.all()
        self.write_joining_table_csv(services_at_location_collection, "aliss_services_at_location_result.csv", services_at_location_dict)
        check_services_at_location_count(services_at_location_collection, "aliss_services_at_location_result.csv", "Services at Location")
