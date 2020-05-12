import csv
from django.http import HttpResponse
from django.contrib import admin
from aliss.models import ALISSUser, Organisation


admin.site.site_header = "Aliss Admin"
admin.site.site_title = "Aliss Admin Portal"
admin.site.index_title = "Welcome to Aliss admin Portal"

class ExportCsvMixin:
    def export_as_csv(self, request, queryset):
        meta = self.model._meta
        fields = ['name',  'email']
        response = HttpResponse(content_type='text/csv')
        response['Content-Disposition'] = 'attachment; filename={}.csv'.format(meta)
        writer = csv.writer(response)
       
        for obj in queryset:
            row = writer.writerow([getattr(obj, field) for field in fields])        
        return response
        
    export_as_csv.short_description = "Export claimed user"

@admin.register(ALISSUser)
class ALISSUserAdmin(admin.ModelAdmin, ExportCsvMixin):
    def custom_titled_filter(title):
        class Wrapper(admin.FieldListFilter):
         def __new__(cls, *args, **kwargs):
          search_fields = ['email']
    list_display = ['email', 'name', 'is_editor', 'is_staff', 'date_joined']
    ordering = ['-date_joined', ]
    list_filter = ('is_editor'),
    actions = ['export_as_csv']
    

