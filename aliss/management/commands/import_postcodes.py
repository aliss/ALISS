import os
import zipfile
import csv

from django.core.management.base import BaseCommand, CommandError
from aliss.models import Postcode


class Command(BaseCommand):
    def add_arguments(self, parser):
        parser.add_argument('-u', '--update', action='store_true', help='Update all postcodes',)

    def extract_postcodes(self):
        postcodes_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes'))
        postcodes_zip = postcodes_dir + '/postcodes.zip'
        print("Extracting " + postcodes_zip + " to " + postcodes_dir)
        with zipfile.ZipFile(postcodes_zip,"r") as zip_ref:
            zip_ref.extractall(postcodes_dir)

    def handle(self, *args, **options):
        if options['update']:
            print("-- Updating all postcodes --")
        else:
            print("-- Importing new postcodes --")

        self.extract_postcodes()

        print("Importing from SmallUser.csv")
        i = 0;
        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/SmallUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)
            for row in reader:
                # If postcode is not deleted
                if not row[4]:
                    defaults = {
                        'postcode_district': row[1],
                        'postcode_sector': row[2],
                        'latitude': row[7],
                        'longitude': row[8],
                        'council_area_2011_code': row[10],
                        'health_board_area_2014_code': row[15],
                        'integration_authority_2016_code': row[18]
                    }
                    if options['update']:
                        Postcode.objects.update_or_create(postcode=row[0], defaults=defaults)
                    else:
                        Postcode.objects.get_or_create(postcode=row[0], defaults=defaults)
                    i = i + 1
                    if (i % 1000) == 0:
                        print("Checked/updated " + str(i) + " postcodes")

        print("Importing from LargeUser.csv")
        i = 0;
        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/LargeUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)
            for row in reader:
                # If postcode is not deleted
                if not row[4]:
                    defaults = {
                        'postcode_district': row[1],
                        'postcode_sector': row[2],
                        'latitude': row[8],
                        'longitude': row[9],
                        'council_area_2011_code': row[11],
                        'health_board_area_2014_code': row[16],
                        'integration_authority_2016_code': row[19]
                    }
                    if options['update']:
                        Postcode.objects.update_or_create(postcode=row[0], defaults=defaults)
                    else:
                        Postcode.objects.get_or_create(postcode=row[0], defaults=defaults)
                    i = i + 1
                    if (i % 1000) == 0:
                        print("Checked/updated " + str(i) + " postcodes")

        self.stdout.write(
            self.style.SUCCESS('Successfully updated postcodes')
        )