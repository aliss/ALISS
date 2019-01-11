from django.contrib.sitemaps import Sitemap
from django.core.urlresolvers import reverse

from aliss.models import Service, Organisation, Postcode


class ServiceSitemap(Sitemap):
    changefreq = 'daily'
    priority = 0.5
    protocol = 'https'
    limit = 2000

    def items(self):
        return Service.objects.order_by('last_edited').all()

    def lastmod(self, obj):
        return obj.last_edited

    def location(self, obj):
        if obj.slug:
            return reverse('service_detail_slug', kwargs={'slug': obj.slug})
        else:
            return reverse('service_detail', kwargs={'pk': obj.pk})


class OrganisationSitemap(Sitemap):
    changefreq = 'monthly'
    priority = 0.5
    protocol = 'https'
    limit = 2000
    section = "static"

    def items(self):
        return Organisation.objects.order_by('last_edited').all()

    def lastmod(self, obj):
        return obj.last_edited

    def location(self, obj):
        if obj.slug:
            return reverse('organisation_detail_slug', kwargs={'slug': obj.slug})
        else:
            return reverse('organisation_detail', kwargs={'pk': obj.pk})


class StaticViewSitemap(Sitemap):
    priority = 0.5
    changefreq = 'monthly'
    protocol = 'https'

    def items(self):
        return ['homepage', 'terms_and_conditions', 'privacy_policy', 'about']

    def location(self, item):
        return reverse(item)


class SearchSitemap(Sitemap):
    changefreq = 'daily'
    priority = 0.5
    protocol = 'https'
    limit = 2000

    def items(self):
        return Postcode.objects.order_by('postcode').all()

    def location(self, obj):
        return reverse('search_share', kwargs={'postcode': obj.postcode})