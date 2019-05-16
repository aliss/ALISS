from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from django.contrib import messages
from django.conf import settings
from django.urls import reverse

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-p', '--verbose', type=bool, help='Print more details -p 1',)

    def handle(self, *args, **options):
        self.stdout.write("\nGenerating Report\n")
        #self.stderr.write(self.style.SUCCESS('Checking service urls'))
        print(options)
        self.verbose = options['verbose']
        print("\nOrganisation details")
        print("  Total number:", Organisation.objects.count())
        print("  Total published:", Organisation.with_services().filter(published=True).count())
        print("  Published & claimed:", Organisation.with_services().exclude(claimed_by=None).filter(published=True).count())
