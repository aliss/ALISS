
from django.core import management
from django.http import HttpResponse

management.call_command("generate_location_report")

