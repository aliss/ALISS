from django import template

from aliss.models import Category

register = template.Library()


@register.assignment_tag #Becomes simple_tag in django 2.0
def can_edit(user, object):
    return object.is_edited_by(user)


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
        2: 'fa-home',
        4: 'fa-pound-sign',
        1: 'fa-utensil-fork',
        134: 'fa-medkit',
        135: 'fa-bus',
        140: 'fa-gavel',
        159: 'fa-hand-paper',
        197: 'fa-cube',
        203: 'fa-bolt',
        204: 'fa-university',
    }
    return icons.get(category.id)

@register.filter
def get_item(dictionary, key):
    return dictionary.get(key)
