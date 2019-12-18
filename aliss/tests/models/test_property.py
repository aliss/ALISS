from django.test import TestCase
from aliss.models import Property, AssignedProperty, Claim
from aliss.tests.fixtures import Fixtures
from django.db.models import Count
from django.db.utils import IntegrityError
from django.db import transaction
from django.core.exceptions import ValidationError


class PropertyTestCase(TestCase):

    def setUp(self):
        t,u,c,s = Fixtures.create_users()
        self.org = Fixtures.create_organisation(t, u, c)
        self.service = Fixtures.create_service(self.org)
        self.location = self.service.locations.first()
        self.properties = Fixtures.create_properties()


    def test_properties_created(self):
        self.assertEqual(len(self.properties), Property.objects.count())


    def test_properties_unique(self):
        p1 = Property.objects.create(name="Another prop")
        p2 = Property(name="Another prop")
        with self.assertRaises(IntegrityError):
            with transaction.atomic():
                p2.save()


    def test_assigned_property_clean(self):
        self.assertFalse(self.properties[1].for_services)
        self.assertTrue(self.properties[1].for_locations)
        self.assertFalse(self.properties[1].for_organisations)

        c = Claim.objects.create(comment="I in charge", organisation=self.org, user=self.org.claimed_by, phone="034343243")

        ap1 = AssignedProperty(property_definition=self.properties[1], holder=self.service)
        ap2 = AssignedProperty(property_definition=self.properties[1], holder=self.org)
        ap3 = AssignedProperty(property_definition=self.properties[1], holder=c)
        ap4 = AssignedProperty(property_definition=self.properties[1], holder=self.location)

        with self.assertRaises(ValidationError):
            ap1.clean()
        with self.assertRaises(ValidationError):
            ap2.clean()
        with self.assertRaises(ValidationError):
            ap3.clean()
        try:
            ap4.clean()
        except ValidationError:
            self.fail("clean() raised ValidationError unexpectedly!")


    def test_property_delete_cascades_to_assigned(self):
        p = Property.objects.get(name="Volunteer run")
        ap1 = AssignedProperty.objects.create(property_definition=p, holder=self.service)
        ap2 = AssignedProperty.objects.create(property_definition=p, holder=self.org)
        ap3 = AssignedProperty.objects.create(property_definition=p, holder=self.location)

        ap3.delete()
        self.assertFalse(AssignedProperty.objects.filter(pk=ap3.pk).exists())
        self.assertTrue(Property.objects.filter(pk=p.pk).exists())

        p.delete()
        self.assertFalse(Property.objects.filter(pk=p.pk).exists())
        self.assertFalse(AssignedProperty.objects.filter(pk=ap1.pk).exists())
        self.assertFalse(AssignedProperty.objects.filter(pk=ap2.pk).exists())


    def test_can_assign_properties(self):
        self.assertEqual(self.service.assigned_properties.count(), 0)
        p = Property.objects.get(name="Volunteer run")
        ap1 = AssignedProperty.objects.create(property_definition=p, holder=self.service)
        ap2 = AssignedProperty.objects.create(property_definition=p, holder=self.org)
        ap3 = AssignedProperty.objects.create(property_definition=p, holder=self.location)
        self.assertTrue(isinstance(AssignedProperty.objects.get(pk=ap1.pk), AssignedProperty))
        self.assertTrue(isinstance(AssignedProperty.objects.get(pk=ap2.pk), AssignedProperty))
        self.assertTrue(isinstance(AssignedProperty.objects.get(pk=ap3.pk), AssignedProperty))


    def tearDown(self):
        Fixtures.general_teardown()
