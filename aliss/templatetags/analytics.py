from django import template
from django.conf import settings
from django.utils.safestring import mark_safe
from google_analytics.templatetags.google_analytics_tags import google_analytics


register = template.Library()

@register.simple_tag(takes_context=True)
def add_analytics_scripts(context):
    analytics_string = """
        <!-- in debug mode, analytics disabled -->
        <script>
            window.disableAnalytics = function(){
                console.log("disableAnalytics (called in debug mode)");
            };
            window.enableAnalytics = function(){
                console.log("enableAnalytics (called in debug mode)");
            };
        </script>
    """
    if not settings.DEBUG:
        analytics_string = """
        <div style="display:none">
            <!-- server side analytics -->
            <img src=\""""+ google_analytics(context) +"""\"width="0" height="0" />
        </div>
        <script async src="https://www.googletagmanager.com/gtag/js?id="""+settings.ANALYTICS_ID+"""\"></script>
        <script>
            var useHotjar = function(h,o,t,j,a,r){
                h.hj=h.hj||function(){(h.hj.q=h.hj.q||[]).push(arguments)};
                h._hjSettings={hjid:1526596,hjsv:6};
                a=o.getElementsByTagName('head')[0];
                r=o.createElement('script');r.async=1;
                r.src=t+h._hjSettings.hjid+j+h._hjSettings.hjsv;
                a.appendChild(r);
            };
            var gtagId = '"""+settings.ANALYTICS_ID+"""';
            window.disableAnalytics = function(){
                window['ga-disable-' + gtagId] = true;
            };
            window.enableAnalytics = function(){
                //window['ga-disable-' + gtagId] = false;
                //gtag('config', gtagId);
                useHotjar(window,document,'https://static.hotjar.com/c/hotjar-','.js?sv=');
            };
            window.disableAnalytics();
            window.dataLayer = window.dataLayer || [];
            //function gtag(){ dataLayer.push(arguments); }
            //gtag('js', new Date());
            //gtag('config', gtagId);
        </script>
        """
    return mark_safe(analytics_string)


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
