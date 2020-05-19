import csv
from django.contrib import admin
from django.http import HttpResponse
from django.db import models
from aliss.models import Claim, Organisation, ALISSUser


class ExportCsvMixin:
    
    def export_as_csv(self, request, queryset):
      
        meta = self.model._meta
       
        fields = ['organisation', 'user',]
  
        response = HttpResponse(content_type='text/csv')
        response['Content-Disposition'] = 'attachment; filename={}.csv'.format(meta)
    
        writer = csv.writer(response)
     
        for obj in queryset:
    
            row = writer.writerow([getattr(obj, field) for field in fields])        
        
        return response
        
        export_as_csv.short_description = "Export claimed Organisation"


@admin.register(Claim)
class ClaimAdmin(admin.ModelAdmin, ExportCsvMixin):
    
    list_filter = ['status', 'created_on']
    list_display = ['organisation', 'organisation_id', 'status', 'created_on']
    def organisation_id(self, obj):
            return obj.organisation.claimed_by
    ordering = ['-created_on', ]
    search_fields = ['organisation__name']
    actions = ['export_as_csv']
