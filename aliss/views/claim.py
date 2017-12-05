from django.views.generic import UpdateView, ListView
from django.http import HttpResponseRedirect
from django.urls import reverse_lazy
from django.core.mail import send_mail
from django.conf import settings

from braces.views import StaffuserRequiredMixin

from aliss.forms import ClaimUpdateForm
from aliss.models import Claim


class ClaimListView(StaffuserRequiredMixin, ListView):
    model = Claim
    template_name = 'claim/list.html'
    paginate_by = 10

    def get_queryset(self):
        queryset = super(ClaimListView, self).get_queryset()

        if self.request.GET.get('status', None) and int(self.request.GET.get('status', None)) in [Claim.UNREVIEWED, Claim.CONFIRMED, Claim.DENIED]:
            queryset = queryset.filter(status=self.request.GET['status'])

        return queryset


class ClaimDetailView(StaffuserRequiredMixin, UpdateView):
    model = Claim
    template_name = 'claim/detail.html'
    form_class = ClaimUpdateForm
    success_url = reverse_lazy('claim_list')

    def send_user_email(self, claim):
        if claim.status == Claim.CONFIRMED:
            message = 'Congratulations, your claim of ownership for {organisation} on ALISS has been confirmed.'.format(organisation=claim.organisation
            )
        else:
            message = 'We are sorry, but your claim of ownership for {organisation} on ALISS has been denied.'.format(
                organisation=claim.organisation
            )

        send_mail(
            'Your ownership claim of {organisation} on ALISS'.format(
                organisation=claim.organisation
            ),
            message,
            settings.DEFAULT_FROM_EMAIL,
            [claim.user.email],
            fail_silently=True,
        )

    def form_valid(self, form):
        self.object = form.save(commit=False)

        if self.object.status == Claim.CONFIRMED:
            self.object.organisation.claimed_by = self.object.user
            self.object.organisation.save()

        self.object.save()

        self.send_user_email(self.object)

        return HttpResponseRedirect(self.get_success_url())
