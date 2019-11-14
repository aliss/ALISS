from django.contrib import admin
from django.db import models
from aliss.models import Claim

@admin.register(Claim)
class CategoryAdmin(admin.ModelAdmin):
    list_filter = ['status']
    list_display = ['user', 'organisation', 'status']
    search_fields = ['user']
