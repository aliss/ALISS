import os
import zipfile
import csv

from django.core.management.base import BaseCommand, CommandError
from aliss.models import Postcode, ServiceArea


class Command(BaseCommand):

    health_boards = {
        'S08000027': 'S08000030',
        'S08000023': 'S08000032',
        'S08000021': 'S08000031',
        'S08000018': 'S08000029'
    }

    hscp = {
        'S37000014': 'S37000032',
        'S37000015': 'S37000034',
        'S37000021': 'S37000035',
        'S37000023': 'S37000033'
    }

    def smalluser_update_postcode(self, row):
        defaults = {
            'latitude': row[7],
            'longitude': row[8],
            'council_area_2011_code': row[10],
            'health_board_area_2014_code': row[15],
            'integration_authority_2016_code': row[18]
        }
        try:
            u=Postcode.objects.filter(postcode=row[0]).update(**defaults)
            if u:
                print("Updated ", row[0])
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
            u=Postcode.objects.filter(postcode=row[0]).update(**defaults)
            if u:
                print("Updated ", row[0])
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
            self.smalluser_update_postcode(row)
        return True


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
        return True


    def update_service_area_codes(self):
        print("Updating service area codes")
        #Updated codes sourced from: https://www.opendata.nhs.scot/dataset/geography-codes-and-labels/resource/944765d7-d0d9-46a0-b377-abb3de51d08e
        for key in self.health_boards.keys():
            if ServiceArea.objects.filter(code=key).update(code=self.health_boards[key]):
                print("Updated service area", key, " -> ", self.health_boards[key])

        for key in self.hscp.keys():
            if ServiceArea.objects.filter(code=key).update(code=self.hscp[key]):
                print("Updated service area", key, " -> ", self.hscp[key])


    def update_postcode_area_codes(self):
        for key in self.health_boards.keys():
            if Postcode.objects.filter(health_board_area_2014_code=key).update(health_board_area_2014_code=self.health_boards[key]):
                print("Updated postcode", key, " -> ", self.health_boards[key])

        for key in self.hscp.keys():
            if Postcode.objects.filter(integration_authority_2016_code=key).update(integration_authority_2016_code=self.hscp[key]):
                print("Updated postcode", key, " -> ", self.hscp[key])


    def handle(self, *args, **options):
        self.update_service_area_codes()
        self.update_postcode_area_codes()
        self.stdout.write(
            self.style.SUCCESS('Successfully updated postcodes')
        )


    def extract_postcodes(self):
        postcodes_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes'))
        postcodes_zip = postcodes_dir + '/postcodes.zip'
        print("Extracting " + postcodes_zip + " to " + postcodes_dir)
        with zipfile.ZipFile(postcodes_zip,"r") as zip_ref:
            zip_ref.extractall(postcodes_dir)


    def update_from_postcodes(self):
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


