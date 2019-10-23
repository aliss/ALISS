from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from django.db.models import F, Count
from django.contrib import messages
from django.conf import settings
from django.urls import reverse
import json

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-p', '--verbose', type=bool, help='Print more details -p 1',)

    def handle(self, *args, **options):
        self.stdout.write("\nGenerating User Report\n")
        #self.stderr.write(self.style.SUCCESS('Checking service urls'))
        print(options)
        self.verbose = options['verbose']

        print("# Contributions")
        user_contributions()
        new_analytics_methods_results()


# Unused user related method?
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
    account_holders = published.exclude(created_by=import_user).exclude(created_by=None)

    claimed_services = Service.published().filter(organisation__in=claimed)

    print("\n## Users\n")
    print(" - Total number:",    ALISSUser.objects.count())
    print(" - Admin status:",    admins.count())
    print(" - Editor status:",   editors.count())
    print(" - [email]@alliance-scotland.org.uk:", alliance_users.count())
    print(" - Basic users:", other_users.count())

    print("\n## Organisations\n")
    print("  Total number:", Organisation.objects.count())
    print("  Total published:", published.count())
    print("\n### Published organisations\n")
    print(" - Total claimed:", claimed.count())
    print(" - Created by basic users:", other_created.count())
    print(" - Created by OSCR import:", oscr_imported.count())
    print(" - Created by account holders: ", account_holders.count())
    print(" - Created by admins:",      admin_created.count())
    print(" - Created by staff:",       staff_created.count())
    print(" - Created by editor:",      editor_created.count())
    print(" - Last updated by an editor creator: ", editor_created.filter(created_by=F('updated_by')).count())
    print(" - Created by their claimant:",    claimant_created.count())
    print(" - Not created but updated by claimant: ", claimed.exclude(created_by=F('updated_by')).count())

    print("\n## Services\n")
    print(" - Total number:",    Service.objects.count())
    print(" - Total published:", Service.published().count())
    print("\n### Published services\n")
    print(" - Created by basic users:",    Service.published().filter(created_by__in=other_users).count())
    print(" - Created by staff:",    Service.published().filter(created_by__in=alliance_users).count())
    print(" - Created by admins:",   Service.published().filter(created_by__in=admins).count())
    print(" - Created by editor:",   Service.published().filter(created_by__in=editors).count())
    print(" - Last updated by an editor creator: ", services_last_updated_by_editor_creator().count())
    print(" - Belong to claimed org.:", claimed_services.count())
    created_by_claimant_services = claimed_services.filter(created_by=F('organisation__claimed_by'))
    print(" - Created by org. claimant:", created_by_claimant_services.count())
    print(" - Not created but updated by org. by claimant: ", created_by_claimant_services.filter(updated_by=F('organisation__claimed_by')).count())


def new_analytics_methods_results():
    print("\n## Other activity\n")
    print(" - Distinct claimants who last updated the organisation they claimed:",len(total_number_of_claimants_who_last_updated_their_claimed_org()))
    print(" - Distinct claimants who last updated the service they claimed:", len(total_number_of_claimants_who_last_updated_their_claimed_service()))
    total_number_of_claimants_who_added_services_to_org()
    print(" - Distinct editors who last updated content they added: ", len(distinct_editors_who_last_updated_content_they_added()))
    print("\n## Categories\n")
    top_categories()


def top_categories(limit=10):
    for category in Category.objects.all().annotate(service_count=Count('services')).order_by('-service_count')[:limit]:
        print(" - " + category.name + "(" + str(category.services.count()) + ")")


def services_last_updated_by_editor_creator():
    editors = ALISSUser.objects.filter(is_editor=True).exclude(email="technical@aliss.org").exclude(is_staff=True)
    services_last_updated_by_editor_creator = Service.published().filter(created_by__in=editors).filter(created_by=F('updated_by'))
    return services_last_updated_by_editor_creator


def distinct_editors_who_last_updated_content_they_added():
    editors = ALISSUser.objects.filter(is_editor=True).exclude(email="technical@aliss.org").exclude(is_staff=True)
    published      = Organisation.with_services().filter(published=True)
    editor_created = published.filter(created_by__in=editors)
    editors_who_last_updated_content_they_added = []
    for organisation in editor_created.filter(created_by=F('updated_by')):
        editors_who_last_updated_content_they_added.append(organisation.created_by)

    for service in services_last_updated_by_editor_creator():
        editors_who_last_updated_content_they_added.append(service.created_by)

    editors_who_last_updated_content_they_added = set(editors_who_last_updated_content_they_added)
    return editors_who_last_updated_content_they_added


def total_number_of_claimants_who_last_updated_their_claimed_org():
    published = Organisation.with_services().filter(published=True)
    orgs_updated_by_claimant = published.filter(claimed_by=F('updated_by'))
    claimants_who_updated = []
    for org in orgs_updated_by_claimant:
        claimants_who_updated.append(org.claimed_by)
    claimants_who_updated = set(claimants_who_updated)
    return claimants_who_updated


def total_number_of_claimants_who_last_updated_their_claimed_service():
    published = Organisation.with_services().filter(published=True)
    claimed = published.exclude(claimed_by=None)
    claimed_services = Service.objects.filter(organisation__in=claimed)
    services_updated_by_claimant = []
    claimants_who_updated_their_service = []
    for service in claimed_services:
        if service.updated_by == service.organisation.claimed_by:
            services_updated_by_claimant.append(service)
    for service in services_updated_by_claimant:
        claimants_who_updated_their_service.append(service.updated_by)
    claimants_who_updated_their_service = set(claimants_who_updated_their_service)
    return claimants_who_updated_their_service


def total_number_of_claimants_who_added_services_to_org():
    published = Organisation.with_services().filter(published=True)
    claimed_orgs = published.exclude(claimed_by=None)
    claimants = []
    for org in claimed_orgs:
        claimants.append(org.claimed_by)
    distinct_claimants = set(claimants)

    print(" - Users who have claimed at least one organisation:", len(distinct_claimants))

    claimants_who_added_services = []
    for claimant in distinct_claimants:
        services_by_claimant = Service.objects.filter(organisation__created_by=claimant).filter(created_by=claimant)
        if services_by_claimant.count() > 0:
            claimants_who_added_services.append(claimant)

    distinct_claimants_added_service_to_org = set(claimants_who_added_services)

    print(" - Users who have claimed at least one organisation and added at least one service to one of those orgs:", len(distinct_claimants_added_service_to_org))
