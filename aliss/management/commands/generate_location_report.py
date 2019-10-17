from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from aliss.search import filter_by_category, check_boundaries, find_boundary_matches, setup_data_set_doubles
from django.db.models import F
from django.contrib import messages
from django.conf import settings
from django.urls import reverse
import json

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-p', '--verbose', type=bool, help='Print more details -p 1',)

    def handle(self, *args, **options):
        self.stdout.write("\nGenerating Report\n")
        #self.stderr.write(self.style.SUCCESS('Checking service urls'))
        print(options)
        self.verbose = options['verbose']

        print("\n---------- Categories in Service Area -----------")
        category_in_service_area()
        # print("\n---------- Location IDs in Regions -----------")
        # location_objects = Location.objects.all()
        # boundaries_data_mappings = setup_data_set_doubles()
        # locations_in_boundaries(location_objects, boundaries_data_mappings)


def postcodes_in_service_area(service_area):
    field_names = {
        ServiceArea.LOCAL_AUTHORITY: 'council_area_2011_code',
        ServiceArea.HEALTH_BOARD: 'health_board_area_2014_code',
        ServiceArea.INTEGRATION_AUTHORITY: 'integration_authority_2016_code'
    }
    kwargs = { '{0}'.format(field_names[service_area.type]): service_area.code }
    return Postcode.objects.filter(**kwargs)

def category_in_service_area(category=Category.objects.get(slug='physical-activity'), location_objects=Location.objects.all(), service_area='health_board'):
    print("Setting up area mappings")
    service_area_mappings = setup_data_set_doubles()
    boundary = service_area_mappings[service_area]
    print("Checking for " + service_area + " boundary")
    service_area_distributions = locations_in_service_area(location_objects, boundary)
    #print("Checking for " + service_area + " boundary")
    for service_area_name, location_ids in service_area_distributions.items():
        print("\n"+ service_area_name)
        services = Service.objects.filter(locations__in=location_ids).distinct()
        filtered_services = category.filter_by_family(services).distinct()
        print(" ", category.name+ ":", str(filtered_services.count()))
        exact_parent_matches = services.filter(categories__name="Money")
        if exact_parent_matches.count() > 0:
            print(" ", "Specific tags:", str(exact_parent_matches.count()))
        for c in category.all_children:
            filtered_services = c.filter_by_family(services)
            print(" ",str(filtered_services.count()), "categorised as", c.name)
            if filtered_services.count() > 0:
                exact_matches = services.filter(categories__name=c.name).distinct()
                if exact_matches.count() > 0:
                    print("   ", "Specific Tags: " + str(exact_matches.count()))

def locations_in_service_area(location_objects, boundary, verbose=False):
    location_long_lats = {}
    for location in location_objects:
        long_lat = (location.longitude, location.latitude)
        location_long_lats[location.id] = long_lat
    service_area = {}
    with open(boundary['data_file_path']) as f:
        js = json.load(f)
    for feature in js['features']:
        service_area[feature['properties'][boundary['data_set_keys']['name']]] = []
    for location_id, long_lat in location_long_lats.items():
        match = find_boundary_matches(boundary, long_lat)
        if len(match) > 0:
            region = match[0]['name']
            service_area[region].append(location_id)
    boundary_string = boundary['data_set_keys']['data_set_name']
    if verbose == True:
        print("\n")
        print(boundary_string)
        for region_name, ids in service_area.items():
            print("\n" + "  Region: " + str(region_name))
            print("    Location IDs: ")
            for id in ids:
                print("      " + str(id))
        print("\n")
    return service_area

def locations_in_boundaries(location_objects, boundaries):
    service_areas = {}
    for service_area, boundary in boundaries.items():
        results = locations_in_service_area(location_objects, boundary)
        service_areas[boundary['data_set_keys']['data_set_name']] = results
    return service_areas
