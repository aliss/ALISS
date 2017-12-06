from .base import *

# Email settings
EMAIL_BACKEND = 'django.core.mail.backends.console.EmailBackend'

# Elasticsearch
ELASTICSEARCH_URL = os.environ.get('ELASTICSEARCH_URL')
ELASTICSEARCH_USERNAME = os.environ.get('ELASTICSEARCH_USERNAME')
ELASTICSEARCH_PASSWORD = os.environ.get('ELASTICSEARCH_PASSWORD')
