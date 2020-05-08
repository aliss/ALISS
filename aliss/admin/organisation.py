import csv
from django.http import HttpResponse
from django.contrib import admin
from aliss.models import Organisation, Claim
from django.core.validators import MinLengthValidator
from django.db import models


class ExportCsvMixin:
    def export_as_csv(self, request, queryset):
        meta = self.model._meta
        field_names = ['name',  'claimed_by']
        response = HttpResponse(content_type='text/csv')
        response['Content-Disposition'] = 'attachment; filename={}.csv'.format(meta)
        writer = csv.writer(response)
        writer.writerow(field_names)      
        description = Organisation.TextField(max_length=255,blank=False, validators=[MinLengthValidator(1)])
        for obj in queryset:
            row = writer.writerow([getattr(obj, field) for field in field_names])        
        return response
    export_as_csv.short_description = "Export claimed Organisation"
    
@admin.register(Organisation)
class OrganisationAdmin(admin.ModelAdmin, ExportCsvMixin):
    def custom_titled_filter(title):

        class Wrapper(admin.FieldListFilter):
            def __new__(cls, *args, **kwargs):
                instance = admin.FieldListFilter.create(*args, **kwargs)
                instance.title = title
                return instance
        return Wrapper


    exclude = ('id', 'created_by',)
    list_display = ('name', 'claimed_by', 'last_edited', 'created_on',  'published')
    ordering = ('claimed_by','created_on')
    search_fields = ['name']
    actions = ['export_as_csv']
    list_filter = ('created_on', custom_titled_filter('date')),

  
    

