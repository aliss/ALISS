from django.conf import settings
from django.utils.text import slugify

from rest_framework import status
from rest_framework.views import exception_handler
from rest_framework.exceptions import ValidationError

import random
import string


def custom_exception_handler(exc, context):
    # Call REST framework's default exception handler first,
    # to get the standard error response.
    response = exception_handler(exc, context)

    if type(exc) == ValidationError:
        response.status_code = status.HTTP_422_UNPROCESSABLE_ENTITY

        if response is not None:
            errors = []
            for field, value in response.data.items():
                if field != 'non_field_errors':
                    errors.append({
                        'field': field,
                        'messages': value
                    })
                else:
                    errors.append({
                        'non_field_errors': value
                    })

            response.data = {
                'message':'Validation Failed',
            }
            response.data['errors'] = errors

    return response


def random_string_generator(size=10, chars=string.ascii_lowercase + string.digits):
    return ''.join(random.choice(chars) for _ in range(size))


def unique_slug_generator(instance, slugifyfield, new_slug=None):
    if new_slug is not None:
        slug = new_slug
    else:
        slug = slugify(getattr(instance, slugifyfield))

    Klass = instance.__class__
    qs_exists = Klass.objects.filter(slug=slug).exists()
    if qs_exists:
        new_slug = "{slug}-{randstr}".format(slug=slug, randstr=random_string_generator(size=3))
        return unique_slug_generator(instance, slugifyfield, new_slug=new_slug)

    return slug