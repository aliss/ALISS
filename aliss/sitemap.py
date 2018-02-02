from django.contrib.sitemaps import Sitemap
from django.core.urlresolvers import reverse

from aliss.models import Service, Organisation


class ServiceSitemap(Sitemap):
    changefreq = 'monthly'
    priority = 0.5
    protocol = 'https'

    def items(self):
        return Service.objects.all()

    def lastmod(self, obj):
        return obj.updated_on

    def location(self, obj):
        return reverse('service_detail', kwargs={'pk': obj.pk})


class OrganisationSitemap(Sitemap):
    changefreq = 'monthly'
    priority = 0.5
    protocol = 'https'

    def items(self):
        return Organisation.objects.all()

    def lastmod(self, obj):
        return obj.updated_on

    def location(self, obj):
        return reverse('organisation_detail', kwargs={'pk': obj.pk})


class StaticViewSitemap(Sitemap):
    priority = 0.5
    changefreq = 'monthly'
    protocol = 'https'

    def items(self):
        return ['homepage', 'terms_and_conditions', 'privacy_policy', 'about']

    def location(self, item):
        return reverse(item)
