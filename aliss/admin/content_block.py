from django.contrib import admin

from aliss.models import ContentBlock

@admin.register(ContentBlock)
class ContentBlockAdmin(admin.ModelAdmin):
    list_display = ('slug', 'body')
