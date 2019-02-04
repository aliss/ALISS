from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.tests.fixtures import Fixtures
from aliss.models import *
from aliss.search import *


class SearchTestCase(TestCase):
    fixtures = ['service_areas.json', 'g2_postcodes.json']

    def setUp(self):
        t, u, c, _ = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        close = Fixtures.create_location(self.org)
        far = Fixtures.create_another_location(self.org)
        self.s1 = Service.objects.create(name="Food For All", description="A handy food service", organisation=self.org, created_by=t, updated_by=u)
        self.s2 = Service.objects.create(name="Step Fit", description="Regular healthy activity", organisation=self.org, created_by=t, updated_by=u)
        self.s3 = Service.objects.create(name="Physically Active", description="Physical activity classes", organisation=self.org, created_by=t, updated_by=u)
        self.s4 = Service.objects.create(name="Exercise for All", description="Activity: get physical in our classes", organisation=self.org, created_by=t, updated_by=u)
        self.s1.locations.add(close); self.s1.save()
        self.s2.locations.add(close); self.s2.save()
        self.s3.locations.add(far); self.s3.save()
        self.s4.locations.add(close); self.s4.save()
        pks = [self.s1.pk, self.s2.pk, self.s3.pk, self.s4.pk]
        self.queryset = get_services(Fixtures.es_connection(), pks)

    def test_filter_by_postcode(self):
        p = Postcode.objects.get(pk="G2 4AA")
        result = filter_by_postcode(self.queryset, p, 100)
        self.assertNotEqual(result.count(), self.queryset.count())

    def test_postcode_order(self):
        p = Postcode.objects.get(pk="G2 4AA")
        result = filter_by_postcode(self.queryset, p, 100000)
        order  = postcode_order(result, p)
        self.assertTrue(order["ids"][0] in [str(self.s1.pk), str(self.s2.pk), str(self.s4.pk)])
        self.assertTrue(order["ids"][3] in [str(self.s3.pk)])
        self.assertEqual(result.count(), self.queryset.count())

    def test_keyword_order(self):
        result = filter_by_query(self.queryset, "Physical Activity")
        order  = keyword_order(result)
        self.assertTrue(order["ids"][0] in [str(self.s3.pk)])
        self.assertTrue(order["ids"][result.count()-1] in [str(self.s2.pk), str(self.s4.pk)])
        self.assertEqual(result.count(), 3)

    def test_combined_order(self):
        p = Postcode.objects.get(pk="G2 4AA")
        result = filter_by_postcode(self.queryset, p, 100000)
        result = filter_by_query(result, "Physical Activity")
        order  = combined_order(result, p)
        services = Service.objects.filter(id__in=order["ids"]).order_by(order["order"])
        self.assertEqual(self.s4, services[0])
        self.assertEqual(self.s2, services[2])

    def tearDown(self):
        Fixtures.organisation_teardown()