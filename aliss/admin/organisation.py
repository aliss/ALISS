from django.contrib import admin

from aliss.models import Organisation


@admin.register(Organisation)
class OrganisationAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'created_by', 'created_on', 'updated_on', 'published')
    ordering = ('-created_on',)
    search_fields = ['name']
