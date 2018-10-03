from django.contrib import admin

from aliss.models import ServiceArea, Service


@admin.register(ServiceArea)
class ServiceAreaAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'code', 'type')


@admin.register(Service)
class ServiceAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'organisation', 'created_by', 'created_on', 'updated_on')
    ordering = ('-created_on',)
