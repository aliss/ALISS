
from django.core.management.base import BaseCommand, CommandError
from aliss.models import *

from django.core.mail import EmailMultiAlternatives
from django.core.mail import send_mail
from django.contrib import messages
from django.conf import settings
from datetime import datetime, timedelta

class Command(BaseCommand):
    help = 'Hide objects older than 10 days'

    def handle(self, *args, **options):
        Service.objects.filter(last_update=datetime.now()-timedelta(days=10)).delete()
        self.stdout.write('Hide objects older than 10 days')