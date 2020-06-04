import os.path
import sys
from django.core import management


class Command(BaseCommand):  
 local_exp = management.call_command("generate_location_report")
with open('location.txt', 'w') as f:
    sys.stdout = f
