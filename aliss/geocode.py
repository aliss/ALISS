from django.core.cache import cache
from django.conf import settings

from geopy.geocoders import GoogleV3


def geocode_location(location):
    cache_result = cache.get("GEOCODE_SEARCH_LOCATION:{0}".format(location))

    if cache_result:
        return cache_result

    geolocator = GoogleV3(api_key=settings.GOOGLE_API_KEY)
    geocode_result = geolocator.geocode(
        location, components={'country': 'gb'}, exactly_one=True, timeout=5
    )

    if geocode_result:
        formatted_result = format_geocode_result(geocode_result.raw)

        cache.set(
            "GEOCODE_SEARCH_LOCATION:{0}".format(location),
            formatted_result,
            2592000 #30 days
        )

        return formatted_result
    else:
        return None


def format_geocode_result(result):
    address = {}

    for component in result['address_components']:
        if 'street_number' in component['types']:
            address['street_number'] = component['long_name']
        if 'route' in component['types']:
            address['route'] = component['long_name']
        if 'locality' in component['types']:
            address['locality'] = component['long_name']
        if 'administrative_area_level_3' in component['types']:
            address['administrative_area_level_3'] = component['long_name']
        if 'administrative_area_level_2' in component['types']:
            address['administrative_area_level_2'] = component['long_name']
        if 'administrative_area_level_1' in component['types']:
            address['administrative_area_level_1'] = component['long_name']
        if 'country' in component['types']:
            address['country'] = component['long_name']
        if 'postal_code' in component['types']:
            address['postal_code'] = component['long_name']

    result['address'] = address
    return result
