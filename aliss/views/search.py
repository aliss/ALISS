import string
from django.views.generic import View, TemplateView
from django.views.generic.list import MultipleObjectMixin
from django.conf import settings
from django.urls import reverse
from django.http import HttpResponseRedirect

from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

from aliss.paginators import ESPaginator
from aliss.forms import SearchForm
from aliss.models import Postcode, Service, Category
from aliss.search import (
    filter_by_query,
    filter_by_postcode,
    filter_by_location_type,
    filter_by_category,
    postcode_order,
    keyword_order,
    combined_order
)

class SearchView(MultipleObjectMixin, TemplateView):
    template_name = 'search/results.html'
    #paginator_class = ESPaginator
    paginate_by = 10

    def get_context_data(self, **kwargs):
        context = super(SearchView, self).get_context_data(**kwargs)
        context['postcode'] = self.postcode
        service_area = self.postcode.get_local_authority()
        if service_area:
            context['service_area'] = service_area.name
        context['category'] = self.category
        context['expanded_radius'] = self.radius * 2
        context['distance_scores'] = self.distance_scores
        return context

    def get(self, request, *args, **kwargs):
        legacy_locations_dict = {
            "Brechin": "DD9 6AD",
            "Dundee": "DD3 8EA",
            "Erskine": "PA8 7WZ",
        }

        location = self.request.GET.get("location")
        result = self.return_match_for_legacy_location(location, legacy_locations_dict)
        search_form = SearchForm(data=self.request.GET)

        if result["match"] == True:
            self.prepare_common_params(self.request.GET)
            return self.assign_legacy_postcode(result["name"], legacy_locations_dict)

        elif search_form.is_valid():
            self.prepare_common_params(search_form.cleaned_data)
            return self.process_user_submitted_postcode(search_form.cleaned_data)

        else:
            searched_term = search_form.data.get('postcode')
            valid_searched_term = None
            if searched_term:
                valid_searched_term = searched_term.strip().isalpha()
            if valid_searched_term:
                try:
                    processed_search_term = searched_term.capitalize().strip()
                    lower_searched = searched_term.lower().strip()
                    matched_postcode = Postcode.objects.get(slug=lower_searched).postcode
                    processed_postcode = matched_postcode.upper().strip().replace(' ', '+')
                    return HttpResponseRedirect(
                        "{url}?postcode={postcode}&place_name={searched_place}".format(
                            url=reverse('search'),
                            postcode=processed_postcode,
                            searched_place=processed_search_term,
                        ))
                except Postcode.DoesNotExist:
                    invalid_placename = True
                    return self.render_to_response(context={
                        'form': search_form,
                        'errors': search_form.errors,
                        'invalid_placename': invalid_placename,
                    })
            else:
                invalid_area = search_form.cleaned_data.get('postcode', None) == None
                return self.render_to_response(context={
                    'form': search_form,
                    'errors': search_form.errors,
                    'invalid_area': invalid_area,
                })


    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')
        if self.request.user.is_staff:
            queryset = queryset.extra(explain=True)
        return queryset

    def filter_queryset(self, queryset):
        if self.category:
            queryset = filter_by_category(queryset, self.category)
        if self.location_type:
            queryset = filter_by_location_type(queryset, self.location_type)
        if self.postcode:
            queryset = filter_by_postcode(queryset, self.postcode, self.radius)
        if self.q:
            queryset = filter_by_query(queryset, self.q)

        if self.q and self.sort == 'keyword':
            results = keyword_order(queryset)
        elif self.q and self.sort in ['best_match', None, '']:
            results = combined_order(queryset, self.postcode)
        else:
            results = postcode_order(queryset, self.postcode)
        self.distance_scores = self.check_distance_within_radius(results["distance_scores"], self.radius)
        return Service.objects.filter(id__in=results["ids"]).order_by(results["order"])

    def assign_legacy_postcode(self, location, legacy_locations_dict):
        postcode = Postcode.objects.get(postcode = legacy_locations_dict.get(str(location)))
        self.postcode = Postcode.objects.get(postcode=postcode)
        return self.define_object_list_return_response()

    def process_user_submitted_postcode(self, data):
        postcode = data.get('postcode', None)
        try:
            if postcode and len(postcode) > 4:
                self.postcode = Postcode.objects.get(postcode=postcode)
            else:
                self.postcode = Postcode.get_by_district(postcode)
        except Postcode.DoesNotExist:
            return self.render_to_response(context={'invalid_area': True})
        return self.define_object_list_return_response()

    def define_object_list_return_response(self):
        self.object_list = self.filter_queryset(self.get_queryset())
        return self.render_to_response(self.get_context_data())

    def prepare_common_params(self, data):
        self.q = data.get('q', None)
        puncstripper = str.maketrans('', '', string.punctuation.replace('-', '')) #keep -
        self.q = self.q.translate(puncstripper)
        self.location_type = data.get('location_type',None)
        self.sort = data.get('sort', None)
        self.category = data.get('category', None)
        self.radius = data.get('radius', None)
        if self.radius == None:
            self.radius = 10000

    def return_match_for_legacy_location(self, location, legacy_locations_dict):
        result = { "match": False, "name": "" }
        for legacy_location_name in legacy_locations_dict:
            if str(legacy_location_name).lower() in str(location).lower():
                result["match"] = True
                result["name"] = str(legacy_location_name)
        return result

    def check_distance_within_radius(self, distance_scores, radius):
        for key in distance_scores.keys():
            if distance_scores[key] != None and distance_scores[key] > radius:
                distance_scores[key] = None
        return distance_scores

class SearchShareView(View):
    def get(self, request, *args, **kwargs):
        postcode = self.kwargs.get('postcode')
        query = self.kwargs.get('query')
        return HttpResponseRedirect(
            "{url}?postcode={postcode}{query}".format(
                url=reverse('search'),
                postcode=postcode,
                query="&q={query}".format(query=query) if query else ''
            )
        )
