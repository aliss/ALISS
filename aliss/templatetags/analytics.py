from django import template
from django.conf import settings
from django.utils.safestring import mark_safe

register = template.Library()


@register.simple_tag()
def google_analytics_script():
    ga_string = '<!-- in debug mode, analytics disabled -->'
    if not settings.DEBUG:
        ga_string = '<script async src="https://www.googletagmanager.com/gtag/js?id=UA-106504389-2"></script>\
        <script>\
            window.dataLayer = window.dataLayer || [];\
            function gtag(){dataLayer.push(arguments);}\
            gtag(\'js\', new Date());\
            gtag(\'config\', \'UA-106504389-2\');\
        </script>'
    return mark_safe(ga_string)


@register.simple_tag(takes_context=True)
def ga_search_events(context, invalid_area, errors):
    request = context["request"]
    ga_string = "//Analytics Disabled "
    if not settings.DEBUG:
        ga_string = ""
    if invalid_area:
        ga_string = ga_string + "gtag('event', 'search-error-unrecognised', { details: 'user entered unrecognised postcode: "+request.GET['postcode']+"' });"
    elif errors and 'postcode' in errors.keys():
        ga_string = ga_string + "gtag('event', 'search-error-invalid', { details: 'user entered invalid postcode: "+request.GET['postcode']+"' });"
    return mark_safe(ga_string)
