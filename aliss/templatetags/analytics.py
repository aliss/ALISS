from django import template
from django.conf import settings
from django.utils.safestring import mark_safe

register = template.Library()

@register.simple_tag()
def google_analytics_script():
    ga_string = '<!-- in debug mode, analytics disabled -->'
    if not settings.DEBUG:
        ga_string = '<script async src="https://www.googletagmanager.com/gtag/js?id='+settings.ANALYTICS_ID+'"></script>\
        <script>\
            window.dataLayer = window.dataLayer || [];\
            function gtag(){dataLayer.push(arguments);}\
            gtag(\'js\', new Date());\
            gtag(\'config\', \''+settings.ANALYTICS_ID+'\');\
        </script>'
    return mark_safe(ga_string)


@register.simple_tag(takes_context=True)
def ga_search_events(context, invalid_area, errors):
    request = context["request"]
    ga_string = "//Analytics Disabled "
    postcode_qs = request.GET.get('postcode')
    if not settings.DEBUG:
        ga_string = ""
    if invalid_area:
        if postcode_qs == None:
            ga_string = ga_string + "gtag('event', 'search-error-unrecognised', { 'event_label': 'no postcode provided: "+request.GET.urlencode()+"' });"
        else:
            ga_string = ga_string + "gtag('event', 'search-error-unrecognised', { 'event_label': 'user entered unrecognised postcode: "+postcode_qs+"' });"
    elif errors and 'postcode' in errors.keys():
        ga_string = ga_string + "gtag('event', 'search-error-invalid', { 'event_label': 'user entered invalid postcode: "+postcode_qs+"' });"

    return mark_safe(ga_string)


@register.simple_tag(takes_context=True)
def ga_form_event(context, form_selector, event_action, event_label):
    ga_string = "//Analytics Disabled "
    request = context["request"]
    if not settings.DEBUG:
        ga_string = ""
    if event_label == "":
        event_label = request.GET.urlencode()
    ga_string = ga_string + "$('"+form_selector+"').submit(function(e){ "
    ga_string = ga_string + "gtag('event', '"+event_action+"', { 'event_label': '"+event_label+"' });"
    ga_string = ga_string + " });"

    return mark_safe(ga_string)


@register.simple_tag(takes_context=True)
def ga_click_event(context, element_selector, event_action, event_label):
    ga_string = "//Analytics Disabled "
    request = context["request"]
    if not settings.DEBUG:
        ga_string = ""
    if event_label == "":
        event_label = request.GET.urlencode()
    ga_string = ga_string + "$('"+element_selector+"').click(function(e){ "
    ga_string = ga_string + "gtag('event', '"+event_action+"', { 'event_label': '"+ event_label +"' });"
    ga_string = ga_string + " });"

    return mark_safe(ga_string)