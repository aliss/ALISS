import csv
from django.http import HttpResponse
from django.core import management
location_export = management.call_command("generate_location_report")

def some_view(request):


    # Create the HttpResponse object with the appropriate CSV header.
    response = HttpResponse(content_type='text/csv')
    response['Content-Disposition'] = 'attachment; filename="somefilename.csv"'

    writer = csv.writer(response)
    writer.writerow([location_export])


    return response