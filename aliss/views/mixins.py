from django.shortcuts import get_object_or_404

from aliss.models import Organisation

class OrganisationMixin(object):
    def get_organisation(self):
        return get_object_or_404(
            Organisation,
            pk=self.kwargs.get('pk')
        )

    def get(self, request, *args, **kwargs):
        self.organisation = self.get_organisation()
        return super(OrganisationMixin, self).get(request, *args, **kwargs)

    def post(self, request, *args, **kwargs):
        self.organisation = self.get_organisation()
        return super(OrganisationMixin, self).post(request, *args, **kwargs)

    def get_context_data(self, **kwargs):
        context = super(OrganisationMixin, self).get_context_data(**kwargs)
        context['organisation'] = self.organisation
        return context


class ProgressMixin(object):
    def get_context_data(self, **kwargs):
        if isinstance(self.object, Organisation):
            organisation = self.object
        else:
            organisation = self.organisation

        context = super(ProgressMixin, self).get_context_data(**kwargs)
        context['progress'] = 2
        if (organisation.locations.count() > 0):
            context['progress'] = 3
        if (organisation.services.count() > 0):
            context['progress'] = 4
        return context