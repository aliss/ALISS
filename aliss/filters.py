from django.db.models import Q

import django_filters
import string

from aliss.models import ALISSUser, Organisation


class OrganisationFilter(django_filters.FilterSet):
    q = django_filters.CharFilter(name='name', lookup_expr='icontains', method='name_filter')

    class Meta:
        model = Organisation
        fields = ['q']

    def name_filter(self, queryset, name, value):
        puncstripper = str.maketrans('', '', string.punctuation.replace('-', ''))
        value = value.translate(puncstripper)
        stopwords = ['the', 'a', 'and', '&']
        stopped_arr = filter(lambda stopword : stopword not in stopwords, value.split())
        return queryset.filter(Q(name__icontains=" ".join(stopped_arr)))


class AccountFilter(django_filters.FilterSet):
    q = django_filters.CharFilter(method='email_name_filter')

    class Meta:
        model = ALISSUser
        fields = ['q']

    def email_name_filter(self, queryset, name, value):
        return queryset.filter(Q(name__icontains=value) | Q(email__icontains=value))
