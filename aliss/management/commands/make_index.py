from django.core.management.base import BaseCommand, CommandError
from aliss.search_tasks import delete_index, create_index, index_all

class Command(BaseCommand):

    def handle(self, *args, **options):
        delete_index()
        create_index()
        index_all()