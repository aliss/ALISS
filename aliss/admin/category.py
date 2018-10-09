from django.contrib import admin
from django.db import models
from aliss.models import Category


def remove_categories(modeladmin, request, queryset):
    #makes sure we use overriden delete
    for category in queryset:
        category.delete()
remove_categories.short_description = "Remove selected categories"


@admin.register(Category)
class CategoryAdmin(admin.ModelAdmin):
    prepopulated_fields = {'slug': ('name',)}
    exclude = ('id',)
    search_fields = ['name']
    list_display = ['name', 'slug', 'service_count', 'subcategory_count']
    actions = [remove_categories]

    def get_queryset(self, request):
        qs = super(CategoryAdmin, self).get_queryset(request)
        qs = qs.annotate(service__count=models.Count('services'))
        return qs

    def service_count(self, obj):
        return obj.service__count
    service_count.admin_order_field = 'service__count'

    def subcategory_count(self, obj):
        return Category.objects.filter(parent=obj).count()

    def get_actions(self, request):
        actions = super(CategoryAdmin, self).get_actions(request)
        del actions['delete_selected']
        return actions

