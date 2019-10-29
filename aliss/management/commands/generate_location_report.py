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

        # print("\n---------- Categories in Service Area -----------")
        # category_in_service_area()
        # print("\n---------- Location IDs in Regions -----------")
        # location_objects = Location.objects.all()
        # boundaries_data_mappings = setup_data_set_doubles()
        # locations_in_boundaries(location_objects, boundaries_data_mappings)
        # print("\n # ---------- Services by Region -----------")
        # services_in_service_area = services_in_service_area_regions('local_authority', 2)
        # for key, value in services_in_service_area.items():
        #     print("#### " + key + ": " + str(value.count()))
        print("\n # ---------- Category Breakdown Service by Region -----------")
        service_area_region_category_top_ten('local_authority', 2, 'Aberdeen City')



def postcodes_in_service_area(service_area):
    field_names = {
        ServiceArea.LOCAL_AUTHORITY: 'council_area_2011_code',
        ServiceArea.HEALTH_BOARD: 'health_board_area_2014_code',
        ServiceArea.INTEGRATION_AUTHORITY: 'integration_authority_2016_code'
    }
    kwargs = { '{0}'.format(field_names[service_area.type]): service_area.code }
    return Postcode.objects.filter(**kwargs)

def category_in_service_area(category=Category.objects.get(slug='physical-activity'), location_objects=Location.objects.all(), service_area='health_integration_authority'):
    print("Setting up area mappings")
    service_area_mappings = setup_data_set_doubles()
    boundary = service_area_mappings[service_area]
    print("Checking for " + service_area + " boundary")
    service_area_distributions = locations_in_service_area(location_objects, boundary)
    #print("Checking for " + service_area + " boundary")
    for service_area_name, location_ids in service_area_distributions.items():
        if (service_area_name == 'Unmatched'):
            print("Unmatched IDs: ")
            for unmatched_location_id in location_ids:
                print(" ", unmatched_location_id)
        print("\n"+ service_area_name)
        services = Service.objects.filter(locations__in=location_ids).distinct()
        filtered_services = category.filter_by_family(services).distinct()
        print(" ", category.name+ ":", str(filtered_services.count()))
        exact_parent_matches = services.filter(categories__name=category)
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
    service_area['Unmatched'] = []
    with open(boundary['data_file_path']) as f:
        js = json.load(f)
    for feature in js['features']:
        service_area[feature['properties'][boundary['data_set_keys']['name']]] = []
    for location_id, long_lat in location_long_lats.items():
        match = find_boundary_matches(boundary, long_lat)
        if len(match) > 0:
            region = match[0]['name']
            service_area[region].append(location_id)
        else:
            service_area['Unmatched'].append(location_id)
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

def services_by_service_area_attribute(type):
    services_by_service_area_region_service_area_match = {}
    service_areas_of_type = ServiceArea.objects.filter(type=type)
    services_in_service_area_type = Service.objects.filter(service_areas__type=type, organisation__published=True)
    for service_area in service_areas_of_type.all():
        services_in_service_area = services_in_service_area_type.filter(service_areas__name__icontains=service_area.name).all()
        services_by_service_area_region_service_area_match[service_area.name] = services_in_service_area.distinct()
    return services_by_service_area_region_service_area_match

def services_by_location_match_in_service_area(service_area_boundary, type):
    services_by_service_area_region_location_match = {}
    service_area_regions = []
    service_areas_of_type = ServiceArea.objects.filter(type=type)
    for service_area_region in service_areas_of_type:
        service_area_regions.append(service_area_region.name)
    locations_of_services = Location.objects.all()
    service_area_mappings = setup_data_set_doubles()
    boundary = service_area_mappings[service_area_boundary]
    service_area_distributions = locations_in_service_area(locations_of_services, boundary)
    for key, value in service_area_distributions.items():
        if key in service_area_regions:
            services_by_service_area_region_location_match[key] = Service.objects.filter(locations__in=value, organisation__published=True).distinct()
    return services_by_service_area_region_location_match

def services_in_service_area_regions(service_area_boundary='local_authority', type=2, verbose=False):
    services_by_service_area_region_service_area_match = services_by_service_area_attribute(type)
    services_by_service_area_region_location_match = services_by_location_match_in_service_area(service_area_boundary, type)
    merged_services = {}
    services_by_service_area = {}
    for key, value in services_by_service_area_region_location_match.items():
        merged_services[key] = services_by_service_area_region_service_area_match[key] | value
    service_count = 0
    for key, value in merged_services.items():
        services_by_service_area[key] = value.distinct()
        service_count = service_count + value.distinct().count()
        if verbose:
            print("## " + str(key) + ": ")
            print("#### Services in region matched by service area: ",  services_by_service_area_region_service_area_match[key].count())
            print("#### Services in region matched by location in service area geospatial data: ", services_by_service_area_region_location_match[key].count())
            print("#### Combined services deduplicated: ", str(value.distinct().count()) + "\n")
    if verbose:
        print("## Meta")
        print("#### Total number of available services per region aggregate (service counted every time it's found available in a region can be duplicate)", service_count)
        print("#### Total number of unique published services", str(Service.objects.filter(organisation__published=True).distinct().count()) + "\n")
    return services_by_service_area

def service_area_region_category_top_ten(service_area_boundary, type, region, limit=10):
    services_in_service_area = services_in_service_area_regions(service_area_boundary, type)
    region_queryset = services_in_service_area[region]
    categories_count = {}
    for category in Category.objects.all():
        services_of_category_count = region_queryset.filter(categories__name__icontains=category.name).distinct()
        categories_count[category.name] = services_of_category_count
    for key, value in categories_count.items():
        count = value.count()
        if count > 0:
            print("#### " + key + ": " + str(count))



def locations_in_boundaries(location_objects, boundaries):
    service_areas = {}
    for service_area, boundary in boundaries.items():
        results = locations_in_service_area(location_objects, boundary)
        service_areas[boundary['data_set_keys']['data_set_name']] = results
    return service_areas
