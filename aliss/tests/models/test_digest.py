from django.conf import settings
from django.test import TestCase
from elasticsearch_dsl import Search
from aliss.models import *
from aliss.search import *
from django.utils import timezone
from datetime import datetime
from datetime import timedelta
import pytz

class DigestTestCase(TestCase):
    fixtures = ['service_areas.json', 'g2_postcodes.json', 'categories.json']

    def setUp(self):
        # Setup for basic DigestSelection creation and deletion
        self.user = ALISSUser.objects.create_user("random@random.org", "passwurd", name="Mike")
        self.postcode = Postcode.objects.get(pk="G2 4AA")
        self.category = Category.objects.get(slug="conditions")

        category2 = Category.objects.get(slug="housing-and-homelessness")

        # Setup for checking the retieve method in the model works
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=t,
          updated_by=u,
          claimed_by=c
        )

        l = Location.objects.create(
          name="My location", street_address="my street", locality="a locality",
          postal_code="G2 4AA", latitude=55.865192, longitude=-4.266868,
          organisation=o, created_by=t, updated_by=u
        )

        # Setup of Service area to match postcode
        service1 = Service.objects.create(name="My First Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        service1.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        service1.categories.add(self.category)
        service1.locations.add(l)
        service1.save()

        service2 = Service.objects.create(name="My Second Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        service2.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        service2.categories.add(self.category)
        service2.locations.add(l)
        service2.save()

        service3 = Service.objects.create(name="My Third Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        service3.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        service3.categories.add(self.category)
        service3.locations.add(l)
        service3.save()

        service4 = Service.objects.create(name="My Fourth Service", description="A handy service", organisation=o, created_by=t, updated_by=u)
        service4.service_areas.add(ServiceArea.objects.get(name="Glasgow City", type=2))
        service4.categories.add(category2)
        service4.locations.add(l)
        service4.save()

        # Need to create string comparison datetime
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)
        self.comparison_date = (current_date - timedelta(weeks=1))

    def test_can_create_digest(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))

    def test_can_retrieve_digest(self):
        # Create a service which checks for category of conditions
        d = DigestSelection.objects.create(postcode=self.postcode, category=self.category, user=self.user)
        self.assertTrue(isinstance(d,DigestSelection))
        # Get a digest object
        test_digest = DigestSelection.objects.get(user=self.user)
        self.assertTrue(isinstance(test_digest, DigestSelection))
        # retrieve the services which match the digest selection
        retrieved = test_digest.retrieve_updated_services(self.comparison_date)
        number_retrieved = len(retrieved[:3])
        # Check the correect number of services are returned
        self.assertTrue(number_retrieved == 3)
        # Create a map of service ids
        service_pks = map(lambda s: s.id, retrieved)
        # Create a list of services which do not have "conditions" as one of their categories
        service_without_category_conditions = Category.objects.filter(services__in = service_pks).exclude(slug="conditions")
        # Check that the list of services which do not have "conditions" has a length of zero
        self.assertTrue(service_without_category_conditions.count() == 0)

    def test_can_retrieve_new_services(self):
        # Create a service which checks for category of conditions
        d = DigestSelection.objects.create(postcode=self.postcode, category=self.category, user=self.user)
        self.assertTrue(isinstance(d,DigestSelection))
        # Get a digest object
        test_digest = DigestSelection.objects.get(user=self.user)
        self.assertTrue(isinstance(test_digest, DigestSelection))
        # retrieve the services which match the digest selection
        retrieved_new = test_digest.retrieve_new_services(self.comparison_date)
        number_retrieved = len(retrieved_new[:3])
        # Create a map of service ids
        service_pks = map(lambda s: s.id, retrieved_new)
        # Create a list of services which do not have "conditions" as one of their categories
        service_without_category_conditions = Category.objects.filter(services__in = service_pks).exclude(slug="conditions")
        # Check that the list of services which do not have "conditions" has a length of zero
        self.assertTrue(service_without_category_conditions.count() == 0)

    def test_can_retrieve_new_services_update_no_impact(self):
        # Create a service which checks for category of conditions
        d = DigestSelection.objects.create(postcode=self.postcode, category=self.category, user=self.user)
        self.assertTrue(isinstance(d,DigestSelection))
        # Get a digest object
        test_digest = DigestSelection.objects.get(user=self.user)
        self.assertTrue(isinstance(test_digest, DigestSelection))
        # #Update a service last_edited field
        service2 = Service.objects.get(name="My Second Service")
        service2.update_service_last_edited()
        service2.save()
        # retrieve the services which match the digest selection
        retrieved_new = test_digest.retrieve_new_services(self.comparison_date)
        retrieve_updated = test_digest.retrieve_updated_services(self.comparison_date)
        # Check retrieve_new_service and retrieve_updated_services don't have the same result.
        self.assertFalse(retrieved_new == retrieve_updated)
        # Get the most recently edited service name form elastic search result.
        most_recently_edited_name = retrieve_updated[0].name
        # Using name retrieve db record of most recently valid edited service.
        most_recently_edited_service = Service.objects.get(name=most_recently_edited_name)
        # Check most recently updated is the one we manually updated.
        self.assertTrue(most_recently_edited_service == service2)
        # Get the most recently created service name form elastic search result.
        newest_service_name = retrieved_new[0].name
        # Using name retrieve db record of most recently created service.
        newest_service = Service.objects.get(name=newest_service_name)
        self.assertFalse(newest_service == service2)
        # If services are created in order then service3 should be the most recently created
        service3 = Service.objects.get(name="My Third Service")
        self.assertTrue(newest_service == service3)



    def test_can_create_digest_without_category(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode)
        self.assertTrue(isinstance(d,DigestSelection))

    def test_user_delete_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        ALISSUser.objects.get(email="random@random.org").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)

    def test_category_delete_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        Category.objects.get(slug="conditions").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)

    def test_postcode_delete_cascades(self):
        d = DigestSelection.objects.create(user=self.user, postcode=self.postcode, category=self.category)
        self.assertTrue(isinstance(d,DigestSelection))
        Postcode.objects.get(pk="G2 4AA").delete()
        self.assertTrue(DigestSelection.objects.all().count() < 1)

    def tearDown(self):
        Service.objects.get(name="My First Service").delete()
        Service.objects.get(name="My Second Service").delete()
        Service.objects.get(name="My Third Service").delete()
        Service.objects.get(name="My Fourth Service").delete()
