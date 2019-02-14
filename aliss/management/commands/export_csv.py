from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.db import models
from django.http import StreamingHttpResponse
from django.views.generic import View
import csv
from django.shortcuts import render
from urllib.request import Request, urlopen

class Command(BaseCommand):

    def write_result_csv(self, collection, filepath='aliss_service.csv'):
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            for obj in collection:
                csv_writer.writerow([obj.id, obj.name])


    def handle(self, *args, **options):
        self.stdout.write("\nWriting CSV\n")
        results = Service.objects.all()[:5]
        self.write_result_csv(results)
