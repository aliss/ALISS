import os
import cloudinary
import cloudinary.uploader
import cloudinary.api

# Build paths inside the project like this: os.path.join(BASE_DIR, ...)
BASE_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# Quick-start development settings - unsuitable for production
# See https://docs.djangoproject.com/en/1.11/howto/deployment/checklist/

# SECURITY WARNING: keep the secret key used in production secret!
SECRET_KEY = os.environ.get('SECRET_KEY')

# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = os.environ.get('ALLOWED_HOSTS').split(',')


# Application definition

INSTALLED_APPS = [
    'django.contrib.admin',
    'django.contrib.auth',
    'django.contrib.contenttypes',
    'django.contrib.sessions',
    'django.contrib.messages',
    'django.contrib.staticfiles',
    'django.contrib.humanize',
    'django.contrib.sitemaps',

    'django_filters',
    'rest_framework',
    'corsheaders',

    'aliss',
    'cloudinary',
    'google_analytics'
]

MIDDLEWARE = [
    'corsheaders.middleware.CorsMiddleware',
    'django.middleware.security.SecurityMiddleware',
    'whitenoise.middleware.WhiteNoiseMiddleware',
    'django.contrib.sessions.middleware.SessionMiddleware',
    'django.middleware.common.CommonMiddleware',
    'django.middleware.csrf.CsrfViewMiddleware',
    'django.contrib.auth.middleware.AuthenticationMiddleware',
    'django.contrib.messages.middleware.MessageMiddleware',
    'django.middleware.clickjacking.XFrameOptionsMiddleware',
    'corsheaders.middleware.CorsMiddleware',
    'django.middleware.common.CommonMiddleware',
]

# CORS Config
CORS_ORIGIN_ALLOW_ALL = False
CORS_ORIGIN_WHITELIST = [
    "https://testing-aliss.herokuapp.com",
    "https://sub.testing-aliss.herokuapp.com",
    "http://localhost:8080",
    "http://127.0.0.1:9000"
]

CORS_URLS_REGEX = r'^/api/.*$'
X_FRAME_OPTIONS = 'DENY'
SECURE_BROWSER_XSS_FILTER = True 
CORS_ALLOW_CREDENTIALS = True
SECURE_FRAME_DENY = True
SECURE_CONTENT_TYPE_NOSNIFF = True
SECURE_BROWSER_XSS_FILTER = True
SESSION_COOKIE_SECURE = True
SESSION_COOKIE_HTTPONLY = True
SECURE_HSTS_SECONDS = 2592000
SECURE_HSTS_INCLUDE_SUBDOMAINS = True
CORS_PREFLIGHT_MAX_AGE = 86400
SECURE_HSTS_PRELOAD = True
SECURE_REFERRER_POLICY = "no-referrer"
CSP_REPORT_ONLY = True

ROOT_URLCONF = 'config.urls'

CORS_ALLOW_HEADERS = [
    'accept',
    'accept-encoding',
    'authorization',
    'content-type',
    'dnt',
    'origin',
    'user-agent',
    'x-csrftoken',
    'x-requested-with',
]

CORS_ALLOW_METHODS = [
    'DELETE',
    'GET',
    'OPTIONS',
    'PATCH',
    'POST',
    'PUT',
]

TEMPLATES = [
    {
        'BACKEND': 'django.template.backends.django.DjangoTemplates',
        'DIRS': [],
        'APP_DIRS': True,
        'OPTIONS': {
            'context_processors': [
                'django.template.context_processors.debug',
                'django.template.context_processors.request',
                'django.contrib.auth.context_processors.auth',
                'django.contrib.messages.context_processors.messages',
            ],
        },
    },
]

WSGI_APPLICATION = 'config.wsgi.application'


# Database
# https://docs.djangoproject.com/en/1.11/ref/settings/#databases
import dj_database_url
DATABASES = {'default': dj_database_url.config()}


# Password validation
# https://docs.djangoproject.com/en/1.11/ref/settings/#auth-password-validators

AUTH_PASSWORD_VALIDATORS = [
    {
        'NAME': 'django.contrib.auth.password_validation.UserAttributeSimilarityValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.MinimumLengthValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.CommonPasswordValidator',
    },
    {
        'NAME': 'django.contrib.auth.password_validation.NumericPasswordValidator',
    },
]


# Internationalization
# https://docs.djangoproject.com/en/1.11/topics/i18n/

LANGUAGE_CODE = 'en-gb'
TIME_ZONE = 'UTC'
USE_I18N = True
USE_L10N = True
USE_TZ = True


# Static files (CSS, JavaScript, Images)
# https://docs.djangoproject.com/en/1.11/howto/static-files/
STATIC_URL = '/static/'
STATIC_ROOT = os.path.join(BASE_DIR, 'staticfiles')
STATICFILES_STORAGE = 'whitenoise.django.GzipManifestStaticFilesStorage'

# Custom User model
AUTH_USER_MODEL = 'aliss.ALISSUser'
LOGIN_URL = '/account/login/'
LOGIN_REDIRECT_URL = '/'

# Email Settings
SERVER_EMAIL = 'info@aliss.org'
DEFAULT_FROM_EMAIL = 'info@aliss.org'

# Error Reporting
ADMINS = (
    ('Vinh Tran', 'vtran@tactuum.com'),
    ('Stefano Labia', 's.labia@tactuum.com'),
)

SESSION_ENGINE = "django.contrib.sessions.backends.cache"
se = os.environ.get('SESSION_ENGINE')
if se != None:
    SESSION_ENGINE = se

SESSION_CACHE_ALIAS = "default"

GOOGLE_API_KEY = os.environ.get('GOOGLE_API_KEY')
ANALYTICS_ID = os.environ.get('ANALYTICS_ID')
GOOGLE_ANALYTICS = {  'google_analytics_id': ANALYTICS_ID } #required for server-side analytics pack

if ANALYTICS_ID == None:
    ANALYTICS_ID = '';

cloudinary.config(
  cloud_name = os.environ.get('CLOUDINARY_CLOUD_NAME'),
  api_key = os.environ.get('CLOUDINARY_API_KEY'),
  api_secret = os.environ.get('CLOUDINARY_API_SECRET')
)

testing_environment = os.environ.get('TESTING_ENV')
if testing_environment == None:
   testing_environment = 'standard'
TESTING_ENV = testing_environment
