from django.contrib import admin
from django.db import models
from aliss.models import Category
from aliss.search import filter_by_category, check_boundaries, find_boundary_matches, setup_data_set_doubles
from django.db.models import Count, Case, When, IntegerField, CharField, F
import json


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

    def service_area_by_region_top_category_count(self, services_in_service_area_by_region, region_name, limit):
           reaction = ('#### ' + region_name + ':')
           region_queryset = services_in_service_area_by_region[region_name]
           for category in Category.objects.all().annotate(
                service_count=Count(Case(
                    When(services__in=region_queryset, then=1),
                    output_field=IntegerField(),
                ))
            ).order_by('-service_count')[:limit]:
               action = (" - " + category.name + ": " + str(category.service_count))
           return service_area_by_region_top_category_count