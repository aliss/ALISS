from django.http import HttpResponseRedirect, HttpResponse
from aliss.models import Postcode, Category
from django.views.generic import View
from django.urls import reverse
import logging


class PlacesView(View):
    def get(self, request, place_slug, category_slug):
        postcode = Postcode.objects.get(slug=place_slug).postcode
        query = "&category=" + category_slug
        return HttpResponseRedirect(
            "{url}?postcode={postcode}{query}".format(
                url=reverse('search'),
                postcode=postcode,
                query="&q={query}".format(query=query) if query else ''
            )
        )
