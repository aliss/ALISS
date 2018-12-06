from django.contrib import admin

from aliss.models import Organisation


@admin.register(Organisation)
class OrganisationAdmin(admin.ModelAdmin):
    exclude = ('id',)
    list_display = ('name', 'created_by', 'claimed_by', 'created_on', 'last_edited', 'published')
    ordering = ('-last_edited',)
    search_fields = ['name']
