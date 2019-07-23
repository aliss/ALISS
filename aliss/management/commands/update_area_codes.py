import os
import zipfile
import csv

from django.core.management.base import BaseCommand, CommandError
from aliss.models import Postcode


class Command(BaseCommand):

    def extract_postcodes(self):
        postcodes_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes'))
        postcodes_zip = postcodes_dir + '/postcodes.zip'
        print("Extracting " + postcodes_zip + " to " + postcodes_dir)
        with zipfile.ZipFile(postcodes_zip,"r") as zip_ref:
            zip_ref.extractall(postcodes_dir)


    def smalluser_update_postcode(self, row):
        defaults = {
            'latitude': row[7],
            'longitude': row[8],
            'council_area_2011_code': row[10],
            'health_board_area_2014_code': row[15],
            'integration_authority_2016_code': row[18]
        }
        try:
            Postcode.objects.filter(postcode=row[0]).update(**defaults)
            return True
        except Exception as e:
            print(e)
            return False


    def largeuser_update_postcode(self, row):
        defaults = {
            'latitude': row[8],
            'longitude': row[9],
            'council_area_2011_code': row[11],
            'health_board_area_2014_code': row[16],
            'integration_authority_2016_code': row[19]
        }
        try:
            Postcode.objects.filter(postcode=row[0]).update(**defaults)
            return True
        except Exception as e:
            print(e)
            return False


    def handle_smalluser_row(self, row):
        try:
            Postcode.objects.get(
                postcode=row[0],
                integration_authority_2016_code=row[18],
                health_board_area_2014_code=row[15],
                council_area_2011_code=row[10]
            )
        except:
            print("Couldnt find postcode")
            self.smalluser_update_postcode(row)


    def handle_largeuser_row(self, row):
        try:
            Postcode.objects.get(
                postcode=row[0],
                integration_authority_2016_code=row[19],
                health_board_area_2014_code=row[16],
                council_area_2011_code=row[11]
            )
        except:
            self.largeuser_update_postcode(row)


    def handle(self, *args, **options):
        self.extract_postcodes()

        print("Updating from SmallUser.csv")
        i = 0;
        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/SmallUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)
            for row in reader:
                self.handle_smalluser_row(row)
                i = i + 1
                if (i % 1000) == 0:
                    print("Checked/updated " + str(i) + " postcodes")

        print("Updating from LargeUser.csv")
        i = 0;
        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/LargeUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)
            for row in reader:
                self.handle_largeuser_row(row)
                i = i + 1
                if (i % 1000) == 0:
                    print("Checked/updated " + str(i) + " postcodes")

        self.stdout.write(
            self.style.SUCCESS('Successfully updated postcodes')
        )