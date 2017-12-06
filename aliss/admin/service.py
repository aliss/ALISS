from django.contrib import admin

from aliss.models import ServiceArea


@admin.register(ServiceArea)
class ServiceAreaAdmin(admin.ModelAdmin):
    exclude = ('id',)
    prepopulated_fields = {'slug': ('name',)}
    list_display = ('name', 'code', 'type')

