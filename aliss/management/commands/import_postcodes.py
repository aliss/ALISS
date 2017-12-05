import os
import csv

from django.core.management.base import BaseCommand, CommandError
from aliss.models import Postcode


class Command(BaseCommand):
    def handle(self, *args, **options):
        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/SmallUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)

            for row in reader:
                # If postcode is not deleted
                if not row[4]:
                    Postcode.objects.update_or_create(
                        postcode=row[0],
                        defaults={
                            'postcode_district': row[1],
                            'postcode_sector': row[2],
                            'latitude': row[7],
                            'longitude': row[8],
                            'council_area_2011_code': row[10],
                            'health_board_area_2014_code': row[15],
                            'integration_authority_2016_code': row[18]
                        }
                    )

        with open(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/postcodes/LargeUser.csv'))) as csvfile:
            reader = csv.reader(csvfile)
            for row in reader:
                # If postcode is not deleted
                if not row[4]:
                    Postcode.objects.update_or_create(
                        postcode=row[0],
                        defaults={
                            'postcode_district': row[1],
                            'postcode_sector': row[2],
                            'latitude': row[8],
                            'longitude': row[9],
                            'council_area_2011_code': row[11],
                            'health_board_area_2014_code': row[16],
                            'integration_authority_2016_code': row[19]
                        }
                    )

            self.stdout.write(
                self.style.SUCCESS('Successfully updated postcodes')
            )
