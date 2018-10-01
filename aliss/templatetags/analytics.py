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