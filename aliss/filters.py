from django.db.models import Q

import django_filters

from aliss.models import ALISSUser, Organisation


class OrganisationFilter(django_filters.FilterSet):
    q = django_filters.CharFilter(name='name', lookup_expr='icontains')

    class Meta:
        model = Organisation
        fields = ['q']


class AccountFilter(django_filters.FilterSet):
    q = django_filters.CharFilter(method='email_username_filter')

    class Meta:
        model = ALISSUser
        fields = ['q']

    def email_username_filter(self, queryset, name, value):
        return queryset.filter(Q(username__icontains=value) | Q(email__icontains=value))
