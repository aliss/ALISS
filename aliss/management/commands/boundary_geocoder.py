from django.core.management.base import BaseCommand, CommandError
import json
from shapely.geometry import shape, Point
from aliss.models import Postcode

class Command(BaseCommand):
    def handle(self, *args, **options):

        with open('./aliss/fixtures/scottish_local_authority.json') as f:
            js = json.load(f)

        # construct point based on lon/lat returned by geocoder
        postcode = Postcode.objects.get(place_name='Glasgow')
        point = Point( postcode.longitude, postcode.latitude)

        # check each polygon to see if it contains the Point
        # matching_features = []
        for feature in js['features']:
            polygon = shape(feature['geometry'])
            if polygon.contains(point):
                print ('Found containing polygon:', feature['properties']['LAD13NM'])

        print('test')
