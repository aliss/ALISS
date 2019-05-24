from django.http import HttpResponseRedirect
from aliss.models import Postcode, Category
from django.views.generic import View
from django.urls import reverse
import logging


class PlacesView(View):
    def get(self, request, *args, **kwargs):
        logger = logging.getLogger(__name__)
        category_slug = request.GET.get('category')
        place_name_slug = request.GET.get('place-name')
        query="&category=" + str(category_slug)
        postcode = Postcode.objects.get(slug=place_name_slug).postcode
        return HttpResponseRedirect(
            "{url}?postcode={postcode}{query}".format(
                url=reverse('search'),
                postcode=postcode,
                query="&q={query}".format(query=query) if query else ''
            )
        )
