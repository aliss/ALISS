from django.contrib import admin

from aliss.models import ALISSUser

@admin.register(ALISSUser)
class ALISSUserAdmin(admin.ModelAdmin):
    search_fields = ['email']
    list_display = ['email', 'name', 'is_editor', 'is_staff']