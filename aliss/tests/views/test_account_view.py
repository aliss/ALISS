from django.test import TestCase
from django.test import Client
from django.urls import reverse
from aliss.models import Organisation, ALISSUser, Service, Location
from aliss.tests.fixtures import Fixtures
from datetime import datetime, timedelta
import pytz

# class AccountViewTestCase(TestCase):
#     def setUp(self):
#         self.user = ALISSUser.objects.create_user("random@random.org", "passwurd")
#         self.client.login(username='random@random.org', password='passwurd')

#         t,u,c,s = Fixtures.create_users()
#         self.user = c
#         self.user.is_editor = True
#         self.user.save()

#          # Create an Organisation for 2 services
#         self.org = Fixtures.create_organisation(t, u, self.user)
#         self.new_org = Fixtures.create_organisation(t,u,u)

#          # Create a service which has been reviewed recently and is represented by user.
#         self.recently_reviewed_service = Fixtures.create_service(self.org)
#         utc = pytz.UTC
#         current_date = datetime.now()
#         current_date = utc.localize(current_date)
#         in_range_date = (current_date - timedelta(weeks=5))
#         self.recently_reviewed_service.last_edited = in_range_date
#         self.recently_reviewed_service.save()

#          # Create a service which is outwith review range but is represented by user.
#         self.unreviewed_service = Service.objects.create(name="Old Service", description="An old service which needs to be reviewed.", organisation=self.org, created_by=self.org.created_by, updated_by=self.org.updated_by)
#         utc = pytz.UTC
#         current_date = datetime.now()
#         current_date = utc.localize(current_date)
#         outwith_range_date = (current_date - timedelta(weeks=8))
#         self.unreviewed_service.last_edited = outwith_range_date
#         self.unreviewed_service.save()

#          # Create a service which is not represented by the user.
#         self.different_user_editor = Service.objects.create(name="Different Editor Service", description="A service which isn't edited by the other user.", organisation=self.new_org, created_by=self.new_org.created_by)


#     def test_saved_services(self):
#         response = self.client.get(reverse('account_saved_services'))
#         self.assertEqual(response.status_code, 200)


#     def test_logout_saved_services(self):
#         self.client.logout()
#         response = self.client.get(reverse('account_saved_services'))
#         self.assertEqual(response.status_code, 302)


#     # def test_account_valid_creation(self):
#     #     response = self.client.post(reverse('signup'),
#     #         { 'name': 'Chef', 'accept_terms_and_conditions': True,
#     #         'email': 'bork@bork.com', 'email_two': 'bork@bork.com', 'password1': 'passwurd', 'password2': 'passwurd' })
#     #     u = ALISSUser.objects.latest('date_joined')
#     #     self.assertEqual(u.email, 'bork@bork.com')
#     #     self.assertEqual(response.status_code, 302)

#     def test_my_reviews(self):
#         response = self.client.get(reverse('account_my_reviews'))
#         self.assertEqual(response.status_code, 200)


#     # Test to check that a user with no claimed services on log in is redirected to the search page.
#     def test_user_no_services_to_review(self):
#         self.client.logout()
#         response = self.client.post('/account/login/', { 'username': "random@random.org", 'password': "passwurd" })
#         self.assertRedirects(response, reverse('homepage'), status_code=302, target_status_code=200)


#     # Test to check that a user with services to review is redirected to the 'My reviews' page on log in.
#     def test_user_services_to_review_redirect(self):
#         self.client.logout()
#         response = self.client.post('/account/login/', { 'username': "claimant@user.org", 'password': "passwurd" })
#         self.assertRedirects(response, reverse('account_my_reviews'), status_code=302, target_status_code=200)


#     def test_user_services_to_review_content(self):
#         self.client.logout()
#         self.client.login(username="claimant@user.org", password="passwurd")
#         response = self.client.get('/account/my-reviews/')
#         self.assertContains(response, "Old Service")


#     def tearDown(self):
#         Fixtures.organisation_teardown()
