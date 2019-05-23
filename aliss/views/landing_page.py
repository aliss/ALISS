from django.http import HttpResponseRedirect
from aliss.models import Postcode, Category

class LandingPageView(View):
    def get(self, request, *args, **kwargs):
        category_slug = self.kwargs.get('category')
        place_name_slug = self.kwargs.get('place-name')
        postcode = Postcode.objects.get(slug=place_name_slug).postcode
        query="&category=" + category_slug
        return HttpResponseRedirect(
            "{url}?postcode={postcode}{query}".format(
                url=reverse('search'),
                postcode=postcode,
                query="&q={query}".format(query=query) if query else ''
            )
        )
