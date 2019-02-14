from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.db import models
from django.http import StreamingHttpResponse
from django.views.generic import View
import csv
from django.shortcuts import render
from urllib.request import Request, urlopen

class Command(BaseCommand):

    def write_result_csv(self, response, filepath='all_services.csv'):
        with open(filepath, mode='w') as output_file:
            urlopen(response)

    def handle(self, *args, **options):

        class Echo(object):
            """An object that implements just the write method of the file-like interface.
            """
            def write(self, value):
                """Write the value by returning it, instead of storing in a buffer."""
                return value

        class ContactLogExportCsvView(View):
            def get(self, *args, **kwargs):
                services_qs = Service.objects.all() # Assume 50,000 objects inside
                model = services_qs.model
                model_fields = model._meta.fields + model._meta.many_to_many
                headers = [field.name for field in model_fields] # Create CSV headers
                def get_row(obj):
                    row = []
                    for field in model_fields:
                        if type(field) == models.ForeignKey:
                            val = getattr(obj, field.name)
                            if val:
                                val = val.__unicode__()
                        elif type(field) == models.ManyToManyField:
                            val = u', '.join([item.__unicode__() for item in getattr(obj, field.name).all()])
                        elif field.choices:
                            val = getattr(obj, 'get_%s_display'%field.name)()
                        else:
                            val = getattr(obj, field.name)
                        row.append(unicode(val).encode("utf-8"))
                    return row
                def stream(headers, data): # Helper function to inject headers
                    if headers:
                        yield headers
                    for obj in data:
                        yield get_row(obj)
                pseudo_buffer = Echo()
                writer = csv.writer(pseudo_buffer)
                response = StreamingHttpResponse(
                    (writer.writerow(row) for row in stream(headers, services_qs)),
                    content_type="text/csv")
                response['Content-Disposition'] = 'file; filename="all_services.csv"'
                print(response.status_code)
                print(response.streaming_content)

                return response

        self.write_result_csv(ContactLogExportCsvView.get(self))

        self.stdout.write(
        self.style.SUCCESS('Successfully export database records as a CSV file.')
        )
