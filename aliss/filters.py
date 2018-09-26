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
        stopwords = ["the", "The", "THE", "a", "A"]
        value_arr = value.split()
        if (len(value_arr) > 1) and (value_arr[0] in stopwords):
            del value_arr[0]
        value = " ".join(value_arr).replace("'s", '')

        puncstripper = str.maketrans('', '', string.punctuation.replace('-', ''))
        stripped = value.translate(puncstripper)

        return queryset.filter(
            Q(name__icontains = value.replace("and", "&")) |
            Q(name__icontains = value.replace("&", "and")) |
            Q(name__icontains = stripped.replace("&", "and")) |
            Q(name__icontains = stripped.replace("and", "&"))
        )


class AccountFilter(django_filters.FilterSet):
    q = django_filters.CharFilter(method='email_name_filter')

    class Meta:
        model = ALISSUser
        fields = ['q']

    def email_name_filter(self, queryset, name, value):
        return queryset.filter(Q(name__icontains=value) | Q(email__icontains=value))