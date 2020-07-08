
from django.core.management.base import BaseCommand, CommandError
from aliss.models import ServiceArea, Service
from aliss.models import *

from django.core.mail import EmailMultiAlternatives
from django.core.mail import send_mail
from django.contrib import messages
from django.conf import settings
from datetime import datetime, timedelta

class Command(BaseCommand):
    help = 'Delete objects older than 10 days'

    def handle(self, *args, **options):
        Service.objects.filter(last_edited=datetime.now()-timedelta(days=30)).delete()
        self.stdout.write('Delete objects older than 10 days')