from aliss.models import *

class Fixtures():

    @classmethod
    def create(self):
        Postcode.objects.create(
          postcode="G2 4AA", postcode_district="G2",  postcode_sector="4",
          latitude="55.86529182", longitude="-4.2684418",
          council_area_2011_code="S12000046",
          health_board_area_2014_code="S08000021",
          integration_authority_2016_code="S37000015"
        )
        t = ALISSUser.objects.create(name="Mr Test", email="tester@aliss.org")
        u = ALISSUser.objects.create(name="Mr Updater", email="updater@aliss.org", is_editor=True)
        c = ALISSUser.objects.create(name="Mr Claimant", email="claimant@aliss.org")
        o = Organisation.objects.create(
          name="TestOrg",
          description="A test description",
          created_by=t, updated_by=u, claimed_by=c
        )
        l = Location.objects.create(
          name="my location", street_address="my street", locality="a locality", 
          postal_code="G2 4AA", latitude=55.86529182, longitude=-4.2684418,
          organisation=o, created_by=t, updated_by=u
        )
        s = Service.objects.create(
          name="My First Service",
          description="A handy service",
          organisation=o, created_by=t, updated_by=u
        )
        s.locations.add(l)
        s.save()