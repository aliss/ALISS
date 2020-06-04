import sys
from django.core import management

location_export = management.call_command("generate_location_report")

sys.stdout = open("static/location/location.txt", "w")
print ("location_export sys.stdout")