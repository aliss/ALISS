from django.contrib import admin
from aliss.models import ServiceArea, Service




@admin.register(ServiceArea)
class ServiceAreaAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'code', 'type')


@admin.register(Service)
class ServiceAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'organisation', 'created_by', 'last_edited', 'created_on')
    ordering = ('-last_edited',)
    search_fields = ['name']
    
