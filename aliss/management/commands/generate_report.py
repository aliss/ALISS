from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from aliss.search import filter_by_category, check_boundaries, find_boundary_matches
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

        print("---------- User Contributions -----------")
        user_contributions()
        print("\n---------- Categories in Service Area -----------")
        category_in_service_area()
        print("\n---------- Location IDs in Regions -----------")
        location_objects = Location.objects.all()
        boundary = {
            'data_file_path':'./aliss/boundary_data_sets/scottish_local_authority.json',
            'data_set_keys':{
                'data_set_name': 'local_authority',
                'code':'LAD13CD',
                'name':'LAD13NM',
            }
        }
        locations_in_service_area(location_objects, boundary)

def graph(qs=ALISSUser.objects, field='date_joined', bins=5):
    #from aliss.models import *
    import matplotlib.pyplot as plt
    import pandas as pd

    #setup dataframe
    fig, ax = plt.subplots()
    values = qs.values(field)
    res = []
    for x in values:
        res.append(x[field])
    dataset=pd.DataFrame({ 'date':res })
    #plot by bins
    ax = dataset["date"].hist(bins=bins, color='teal', alpha=0.8, rwidth=0.999)
    ax.set(xlabel='Date', ylabel='Count')
    plt.show()

def user_contributions():
    import_user    = ALISSUser.objects.get(email="technical@aliss.org")
    admins         = ALISSUser.objects.exclude(email="technical@aliss.org").filter(is_staff=True)
    alliance_users = ALISSUser.objects.filter(email__icontains="alliance-scotland.org.uk")
    editors        = ALISSUser.objects.filter(is_editor=True).exclude(email="technical@aliss.org").exclude(is_staff=True)
    other_users    = ALISSUser.objects.exclude(email="technical@aliss.org").difference(alliance_users, editors)

    published      = Organisation.with_services().filter(published=True)
    oscr_imported  = published.filter(created_by=import_user)
    admin_created  = published.filter(created_by__in=admins)
    staff_created  = published.filter(created_by__in=alliance_users)
    editor_created = published.filter(created_by__in=editors)
    other_created  = published.filter(created_by__in=other_users)
    claimed        = published.exclude(claimed_by=None)
    claimant_created = claimed.filter(created_by=F('claimed_by'))

    print("\nUsers:")
    print("  Total number:",    ALISSUser.objects.count())
    print("  Admin status:",    admins.count())
    print("  Editor status:",   editors.count())
    print("  [email]@alliance-scotland.org.uk:", alliance_users.count())
    print("  Other:", other_users.count())

    print("\nOrganisations:")
    print("  Total number:", Organisation.objects.count())
    print("  Total published:", published.count())
    print("\nPublished organisations")
    print("  Total claimed:", claimed.count())
    print("  Created by OSCR import:", oscr_imported.count())
    print("  Created by admins:",      admin_created.count())
    print("  Created by staff:",       staff_created.count())
    print("  Created by editor:",      editor_created.count())
    print("  Created by other:",       other_created.count())
    print("  Created by claimant:",    claimant_created.count())

    print("\nServices:")
    print("  Total number:",    Service.objects.count())
    print("  Total published:", Service.published().count())
    print("\nPublished services")
    print("  Belong to claimed org:", Service.published().filter(organisation__in=claimed).count())
    print("  Created by staff:",    Service.published().filter(created_by__in=alliance_users).count())
    print("  Created by admins:",   Service.published().filter(created_by__in=admins).count())
    print("  Created by editor:",   Service.published().filter(created_by__in=editors).count())
    print("  Created by other:",    Service.published().filter(created_by__in=other_users).count())


def postcodes_in_service_area(service_area):
    field_names = {
        ServiceArea.LOCAL_AUTHORITY: 'council_area_2011_code',
        ServiceArea.HEALTH_BOARD: 'health_board_area_2014_code',
        ServiceArea.INTEGRATION_AUTHORITY: 'integration_authority_2016_code'
    }
    kwargs = { '{0}'.format(field_names[service_area.type]): service_area.code }
    return Postcode.objects.filter(**kwargs)

def category_in_service_area(category=Category.objects.get(slug='physical-activity'), area_type=ServiceArea.HEALTH_BOARD):
    for service_area in ServiceArea.objects.filter(type=area_type):
        print("\n"+service_area.name)
        postcodes = postcodes_in_service_area(service_area)
        locations = Location.objects.filter(postal_code__in=postcodes.values_list('postcode')).distinct()
        services = Service.objects.filter(locations__in=locations).distinct()
        services |= service_area.services.distinct()
        filtered_services = category.filter_by_family(services)
        print(" ", category.name+ ":", str(filtered_services.count()))
        for c in category.all_children:
            filtered_services = c.filter_by_family(services)
            print(" ",str(filtered_services.count()), "categorised as", c.name)

def locations_in_service_area(location_objects, boundary):
    location_long_lats = {}
    for location in location_objects:
        long_lat = (location.longitude, location.latitude)
        location_long_lats[location.id] = long_lat
    local_authority = {}
    with open(boundary['data_file_path']) as f:
        js = json.load(f)
    for feature in js['features']:
        local_authority[feature['properties'][boundary['data_set_keys']['name']]] = []
    for item in location_long_lats.items():
        match = find_boundary_matches(boundary, item[1])
        region = match[0]['name']
        local_authority[region].append(item[0])
    boundary_string = boundary['data_set_keys']['data_set_name']
    print(boundary_string)
    for region in local_authority.items():
        print("\n" + "  Region: " + str(region[0]))
        print("    Location IDs: ")
        for id in region[1]:
            print("      " + str(id))
    print("\n")
    print(local_authority)
