from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from django.contrib import messages
from django.conf import settings
from urllib.request import Request, urlopen
from urllib.error import URLError, HTTPError
from django.urls import reverse
import csv

class Command(BaseCommand):

    def check_url(self, url):
        req = Request(url)
        try:
            response = urlopen(req)
        except HTTPError as e:
            self.stderr.write('The server couldn\'t fulfill the request.')
            self.stderr.write("Error code: {0}\n".format(e.code))
            return e.code
        except URLError as e:
            self.stderr.write('We failed to reach a server.')
            self.stderr.write("Reason: {0}\n".format(e.reason))
            return e.reason
        except ConnectionResetError as e:
            self.stderr.write("Connection Reset Error\n")
            return "ConnectionResetError"
        else:
            return response.status

    def write_result_csv(self, collection, filepath='quality_check.csv'):
        self.stderr.write("Writing results to {0}\n".format(filepath))
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            for line in collection:
                csv_writer.writerow([line['type'], line['name'], line['url'], line['object_url']])

    def write_result_out(self, collection):
        self.stdout.write("\nType,Name,Failed Url,Object Url\n")
        for line in collection:
            self.stdout.write("{0},{1},{2},{3}\n".format(line['type'], line['name'], line['url'], line['object_url']))

    def check_organisations(self, results):
        self.stderr.write(self.style.SUCCESS('Checking organisation urls'))
        organisations = Organisation.objects.all()[:100]
        for org in organisations:
            if org.url == None or org.url == "":
                continue
            self.stderr.write("\tChecking {0}\n".format(org.url))
            result = self.check_url(org.url)
            request_failed = str(result) != "200"
            if request_failed:
                results.append({ 'type': 'organisation', 'name': org.name, 'object_url': "https://www.aliss.org" + reverse('organisation_detail', kwargs={ 'pk': org.pk }), 'url': org.url })
        return results

    def check_services(self, results):
        self.stderr.write(self.style.SUCCESS('Checking service urls'))
        services = Service.objects.all()[:100]
        for s in services:
            if s.url == None or s.url == "":
                continue
            self.stderr.write("\tChecking {0}\n".format(s.url))
            result = self.check_url(s.url)
            request_failed = str(result) != "200"
            if request_failed:
                results.append({ 'type': 'service', 'name': s.name, 'object_url': "https://www.aliss.org" + reverse('service_detail', kwargs={ 'pk': s.pk }), 'url': s.url })
        return results

    def handle(self, *args, **options):
        results = []
        results = self.check_organisations(results)
        results = self.check_services(results)
        self.write_result_out(results)