
from django.core.management.base import BaseCommand, CommandError
from aliss.models import ServiceArea, Service
from aliss.models import service
from datetime import datetime, timedelta
import pytz

class Command(BaseCommand):
      
    def handle(self, *args, **options):
        number_of_weeks = 6
        current_date = datetime.now() 
        comparison_date = Service.objects.filter(last_edited=current_date-timedelta(weeks=number_of_weeks)).delete()

        self.stdout.write('Delete objects older than 6 weeks')
        
    # def check_last_reviewed(self):
    #     utc = pytz.UTC
    #     current_date = datetime.now()
    #     current_date = utc.localize(current_date)
    #     number_of_weeks = 6
    #     comparison_date = current_date - timedelta(weeks=number_of_weeks)
    #     if self.last_edited == None:
    #         return self.id
    #     elif self.last_edited < comparison_date:
    #         return self.id
    #     else:
    #         return None