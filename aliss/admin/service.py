from django.contrib import admin

from aliss.models import ServiceArea


@admin.register(ServiceArea)
class ServiceAreaAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'code', 'type')

