from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from django.contrib import messages
from django.conf import settings
from urllib.request import Request, urlopen
from urllib.error import URLError, HTTPError
import ssl
from django.urls import reverse
import csv

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-p', '--verbose', type=bool, help='Print more details',)
        parser.add_argument('-w', '--csv', type=bool, help='Write to csv',)

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
        except ssl.CertificateError as e:
            self.stderr.write("SSL Certificate Error\n")
            return "SSL Certificate Error"
        else:
            return response.status

    def write_result_csv(self, collection, filepath='quality_check.csv'):
        self.stderr.write("Writing results to {0}\n".format(filepath))
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            for line in collection:
                csv_writer.writerow([line['type'], line['name'], line['url'], line['object_url']])

    def perform_check(self, results, collection, type='organisation'):
        for obj in collection:
            if obj.url == None or obj.url == "":
                continue
            self.stderr.write("\tChecking {0}\n".format(obj.url))
            result = self.check_url(obj.url)
            request_failed = (str(result) != "200") or ("SSL" not in str(result))
            if request_failed:
                rd = { 'type': type, 'name': obj.name, 'object_url': "https://www.aliss.org" + reverse((type+'_detail'), kwargs={ 'pk': obj.pk }), 'url': obj.url }
                results.append(rd)
                if self.verbose:
                    self.stdout.write("{0},{1},{2},{3}\n".format(rd['type'], rd['name'], rd['url'], rd['object_url']))
        return results

    def handle(self, *args, **options):
        print(options)
        self.verbose = options['verbose']
        self.write_csv = options['csv']

        results = []
        self.stderr.write(self.style.SUCCESS('Checking service urls'))
        services = Service.objects.order_by('-created_on').all()[:20]
        if self.verbose:
            self.stdout.write("\nType,Name,Failed Url,Object Url\n")
        results = self.perform_check(results, services, 'service')

        self.stderr.write(self.style.SUCCESS('Checking organisation urls'))
        organisations = Organisation.objects.order_by('-created_on').all()[:20]
        if self.verbose:
            self.stdout.write("\nType,Name,Failed Url,Object Url\n")
        results = self.perform_check(results, organisations, 'organisation')

        if self.write_csv:
            self.stdout.write("\nWriting CSV\n")
            self.write_result_csv(results)