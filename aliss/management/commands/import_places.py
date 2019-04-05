import csv
import requests
import io
from django.core.management.base import BaseCommand, CommandError
from aliss.models import Postcode


class Command(BaseCommand):
    def handle(self, *args, **options):
        r = requests.get('https://gist.githubusercontent.com/digitalWestie/7fc02a45070bd862ea30cf229d162f21/raw/606af18ca363558135d3952b9d793bfb8cac3088/the-merged-dirty.csv')
        if r.status_code != 200:
            self.stdout.write(self.style.ERROR('Quitting, status code: ' + r.status_code))
            return None

        header = False
        buff = io.StringIO(r.text)
        reader = csv.reader(buff)

        for row in reader:
            place = { 'locality': row[2], 'postcode': row[5] }
            if place['postcode'] == 'Postcode':
                continue
                header = True
            elif header == True:
                self.stdout.write(self.style.ERROR('Quitting, wrong header: ' + place['postcode']))
                return None
            try:
                p = Postcode.objects.get(pk=place['postcode'])
                p.place_name = place['locality']
                p.save()
                self.stdout.write(self.style.SUCCESS('Successfully updated postcode ' + p.place_name))
            except:
                self.stdout.write(self.style.ERROR('Couldn\'t  find/save ' + place['postcode']))
        self.stdout.write(self.style.SUCCESS('Successfully updated postcodes'))
