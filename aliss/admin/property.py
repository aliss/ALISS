from django.contrib import admin
from aliss.models import Property, AssignedProperty


@admin.register(Property)
class PropertyAdmin(admin.ModelAdmin):
    class Media:
        css = { "all": ("css/styles.css",) }

    exclude = ('id',)
    list_display = ('name', 'description', 'icon', 'icon_html', 'for_services', 'for_organisations', 'for_locations')
    ordering = ('-name',)
    search_fields = ['name', 'description', 'icon']
