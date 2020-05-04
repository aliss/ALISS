import csv
from django.http import HttpResponse
from django.contrib import admin
from aliss.models import Organisation


class ExportCsvMixin:
    def export_as_csv(self, request, queryset):
        meta = self.model._meta
        field_names = ['name',  'claimed_by']
        response = HttpResponse(content_type='text/csv')
        response['Content-Disposition'] = 'attachment; filename={}.csv'.format(meta)
        writer = csv.writer(response)

        writer.writerow(field_names)
        
        for obj in queryset:
            row = writer.writerow([getattr(obj, field) for field in field_names])
        
  
        return response

    export_as_csv.short_description = "Export claimed Organisation"


@admin.register(Organisation)
class OrganisationAdmin(admin.ModelAdmin, ExportCsvMixin):
    exclude = ('id', 'created_by', 'created_on', )
    list_display = ('name',  'claimed_by',  'last_edited', 'published')
    ordering = ('claimed_by',)
    search_fields = ['name']
    actions = ['export_as_csv']
    list_filter = ('claimed_by'),
    
