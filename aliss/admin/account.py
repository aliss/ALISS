from django.contrib import admin

from aliss.models import ALISSUser

@admin.register(ALISSUser)
class ALISSUserAdmin(admin.ModelAdmin):
    search_fields = ['email']
