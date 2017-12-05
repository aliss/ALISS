from django.contrib import admin

from aliss.models import Category

@admin.register(Category)
class CategoryAdmin(admin.ModelAdmin):
    prepopulated_fields = {'slug': ('name',)}
    exclude = ('id',)
