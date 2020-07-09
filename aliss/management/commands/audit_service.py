
from django.core.management.base import BaseCommand, CommandError
from datetime import datetime, timedelta
from aliss.models import *


class Command(BaseCommand):
      
    def handle(self, *args, **options):
        number_of_weeks = 24
        current_date = datetime.now() 
        comparison_date = Service.objects.filter(last_edited=current_date-timedelta(weeks=number_of_weeks)).delete()

        self.stdout.write('Delete objects older than 6 months')