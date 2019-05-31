from django import template
from datetime import datetime
import pytz
import logging

from aliss.models import Category, Organisation

register = template.Library()


@register.assignment_tag #Becomes simple_tag in django 2.0
def can_edit(user, object):
    return object.is_edited_by(user)


@register.assignment_tag
def can_add_logo(user, object):
    if object.pk is None:
        return False# return user.is_staff or user.is_editor
    else:
        return object.can_add_logo(user)


@register.simple_tag
def query_transform(request, **kwargs):
    updated = request.GET.copy()
    for k, v in kwargs.items():
        if v is not None:
            updated[k] = v
        else:
            updated.pop(k, 0)  # Remove or return 0 - aka, delete safely this key

    return updated.urlencode()

@register.simple_tag
def process_locations(collection, **kwargs):
    postcode = kwargs['postcode'].upper().strip()
    length = len(postcode)
    specificity = length
    matching_districts = []
    locations = list(collection)
    while len(collection) > len(matching_districts) and specificity >= 0:
        comparison_code = postcode[:specificity]
        for location in locations:
            if comparison_code in str(location).strip():
                index = locations.index(location)
                matching_districts.append(locations.pop(index))
        if specificity == 0:
            return matching_districts + locations
        specificity -= 1
    return matching_districts

@register.simple_tag
def get_root_categories():
    return Category.objects.prefetch_related('children').filter(parent__isnull=True)


@register.simple_tag
def get_categories():
    return Category.objects.select_related('parent').prefetch_related('children').all()


@register.simple_tag
def get_category_tree(category):
    tree = []
    while True:
        tree.insert(0, category)
        if category.parent:
            category = category.parent
        else:
            break
    return tree


@register.filter
def get_icon(category):
    icons = {
        'housing-and-homelessness': 'fa-home',
        'money': 'fa-pound-sign',
        'food-nutrition': 'fa-utensil-fork',
        'conditions': 'fa-medkit',
        'transport-mobility': 'fa-bus',
        'rights-representation': 'fa-gavel',
        'health-social-care-services': 'fa-hand-paper',
        'goods': 'fa-cube',
        'activity': 'fa-bolt',
        'education-employability': 'fa-university',
        'children-families': 'fa-child',
        'sexual-health': 'fa-heart'
    }
    return icons.get(category.slug)


@register.filter
def get_item(dictionary, key):
    return dictionary.get(key)

@register.filter
def format_time_string(value):
    utc = pytz.UTC
    value = value.split('+')
    value = value[0]
    d = datetime.strptime(value, '%Y-%m-%dT%H:%M:%S.%f')
    utc.localize(d)
    return d

@register.simple_tag(takes_context=True)
def absolute(context, path):
    request = context["request"]
    return request.scheme + "://" + request.get_host() + path

@register.simple_tag()
def meta_description(service):
    m_location = meta_location(service)
    categories = []
    for c in service.categories.all():
        categories.append(c.name)
    description = service.name
    if m_location:
        description += " " + m_location + " "
    else:
        description += " - "
    description += ", ".join(categories)
    remaining = 297 - len(description)
    if remaining > 15:
        description += " - " + service.description[:remaining] + '...' * (len(service.description) > remaining)
    return description


@register.simple_tag()
def meta_location(service, brackets=True):
    txt = ''
    if service.locations.count() == 1:
        location = service.locations.first()
        if location.name:
            txt = location.name + ", " + location.locality
        else:
            txt = location.locality
    else:
        areas = []
        for area in service.service_areas.filter(type=2).all():
            areas.append(area.name)
        txt = ", ".join(areas)
    if brackets and txt:
        txt = "(" + txt + ")"
    return txt
