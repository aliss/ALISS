from django.conf import settings

def mixpanel_key(request):
    return {'MIXPANEL_KEY': settings.MIXPANEL_KEY}
