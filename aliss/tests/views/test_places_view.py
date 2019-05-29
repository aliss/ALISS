from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *

class SearchViewTestCase(TestCase):
    fixtures = ['categories.json','service_areas.json', ]

    def setUp(self):
        t, u, c, _ = Fixtures.create_users()
        o = Fixtures.create_organisation(t, u, c)
        s = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        s.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))

        '''
        '''

        '''
        '''

        '''
        '''
