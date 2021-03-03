from .base import *
from dotenv import load_dotenv
load_dotenv()
DEBUG = False

# Elasticsearch
ELASTICSEARCH_URL = os.environ.get('FOUNDELASTICSEARCH_URL')
ELASTICSEARCH_USERNAME = os.environ.get('ELASTICSEARCH_USERNAME', '')
ELASTICSEARCH_PASSWORD = os.environ.get('ELASTICSEARCH_PASSWORD', '')

# Temp session engine until we set up Redis
SESSION_ENGINE = 'django.contrib.sessions.backends.cached_db'

# Email settings
EMAIL_BACKEND = 'django.core.mail.backends.smtp.EmailBackend'

# EMAIL_HOST_USER = EMAIL_HOST_USER = 'apikey'
# EMAIL_HOST= 'smtp.sendgrid.net'
# EMAIL_PORT = 587
# EMAIL_USE_TLS = True
# EMAIL_HOST_PASSWORD = os.environ['SENDGRID_PASSWORD']

# Twilio SendGrid
EMAIL_HOST = 'smtp.sendgrid.net'
EMAIL_PORT = 587
EMAIL_USE_TLS = True
EMAIL_HOST_USER = 'apikey'
EMAIL_HOST_PASSWORD = os.environ.get('SENDGRID_API_KEY')

SECURE_SSL_REDIRECT = True
