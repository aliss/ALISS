from django.core.management.base import BaseCommand, CommandError
from aliss.models import *
from django.contrib import messages
from django.conf import settings
from urllib.request import Request, urlopen
from urllib.error import URLError, HTTPError
import ssl
from django.urls import reverse
import csv
import socket

class Command(BaseCommand):

    def add_arguments(self, parser):
        parser.add_argument('-p', '--verbose', type=bool, help='Print more details -p 1',)
        parser.add_argument('-w', '--csv', type=bool, help='Write to csv e.g. -w 1 ',)
        parser.add_argument('-o', '--offset', type=float, help='Percent of results to offset by e.g. -o 50',)

    def check_url(self, url):
        req = Request(url)
        try:
            response = urlopen(req, timeout=7)
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
        except socket.timeout as e:
            self.stderr.write("Socket timeout\n")
            return "Timeout (too slow)"
        else:
            return response.status

    def write_result_csv(self, collection, filepath='quality_check.csv'):
        self.stderr.write("Writing results to {0}\n".format(filepath))
        with open(filepath, mode='w') as output_file:
            csv_writer = csv.writer(output_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
            for line in collection:
                csv_writer.writerow([line['type'], line['name'], line['error'], line['url'], line['object_url']])

    def perform_check(self, results, collection, type='organisation'):
        for obj in collection:
            if obj.url == None or obj.url == "":
                continue
            self.stderr.write("\tChecking {0}\n".format(obj.url))
            result = self.check_url(obj.url)
            request_failed = (str(result) not in ["200", "403"]) and ("SSL" not in str(result))
            if request_failed:
                rd = { 'type': type, 'name': obj.name, 'error': str(result), 'object_url': "https://www.aliss.org" + reverse((type+'_detail'), kwargs={ 'pk': obj.pk }), 'url': obj.url }
                results.append(rd)
                if self.verbose:
                    self.stdout.write("{0},{1},{2},{3},{4}\n".format(rd['type'], rd['name'], rd['error'], rd['url'], rd['object_url']))
        return results

    def handle(self, *args, **options):
        print(options)
        self.verbose = options['verbose']
        self.write_csv = options['csv']
        if options['offset']:
            offset = options['offset']
        else:
            offset = 0

        results = []
        self.stderr.write(self.style.SUCCESS('Checking service urls'))
        service_offset = int(Service.objects.exclude(organisation__published=False).count() * (offset/100))
        services = Service.objects.exclude(organisation__published=False).order_by('created_on').all()[service_offset:]
        if self.verbose:
            self.stdout.write("\nType,Name,Failed Url,Object Url\n")
        results = self.perform_check(results, services, 'service')

        self.stderr.write(self.style.SUCCESS('Checking organisation urls'))
        org_offset = int(Organisation.objects.exclude(published=False).count() * (offset/100))
        organisations = Organisation.objects.exclude(published=False).order_by('created_on').all()[org_offset:]
        if self.verbose:
            self.stdout.write("\nType,Name,Failed Url,Object Url\n")
        results = self.perform_check(results, organisations, 'organisation')

        if self.write_csv:
            self.stdout.write("\nWriting CSV\n")
            self.write_result_csv(results)