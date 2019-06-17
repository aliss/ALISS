from django.contrib import admin
from aliss.models import Postcode


@admin.register(Postcode)
class PostcodeAdmin(admin.ModelAdmin):
    list_display = ('postcode', 'latitude', 'longitude', 'council_area_2011_code', 'place_name')
    search_fields = ['place_name', 'postcode']
