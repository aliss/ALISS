from django.conf.urls import url, include
from django.contrib import admin

urlpatterns = [
    url(r'^admin/', admin.site.urls),
    url(r'^api/', include('aliss.api.urls')),
    url(r'^', include('aliss.urls')),
]
