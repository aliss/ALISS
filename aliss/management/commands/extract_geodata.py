import os
import zipfile
from django.core.management.base import BaseCommand, CommandError


class Command(BaseCommand):

    def all_exist(self, paths):
        for path in paths:
            if not os.path.exists(path):
                print(path + " not found, extracting geodata...")
                return False
        return True

    def handle(self, *args, **options):
        boundaries_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '../../data/boundaries'))
        boundaries_zip = boundaries_dir + '/boundaries.zip'
        boundary_paths = [
            boundaries_dir + '/scottish_local_authority.geojson',
            boundaries_dir + '/SG_NHS_HealthBoards_2019.geojson',
            boundaries_dir + '/SG_NHS_IntegrationAuthority_2019.geojson',
            boundaries_dir + '/Countries_December_2017_Ultra_Generalised_Clipped_Boundaries_in_UK.geojson'
        ]
        if self.all_exist(boundary_paths):
            print("Boundary data already extracted.")
        else:
            print("Extracting " + boundaries_zip + " to " + boundaries_dir)
            with zipfile.ZipFile(boundaries_zip,"r") as zip_ref:
                zip_ref.extractall(boundaries_dir)
            print("Done.")


