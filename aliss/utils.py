from django.conf import settings

from rest_framework import status
from rest_framework.views import exception_handler
from rest_framework.exceptions import ValidationError


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
