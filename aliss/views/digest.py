from django.views.generic import TemplateView, DeleteView



# Import all models
from aliss.models import DigestSelection



class AccountCreateDigestSelection(LoginRequiredMixin, TemplateView):
    # Need to create a new template with a form action points to this view
    template_name = 'account/create_my_digest.html'
    model = DigestSelection

    def get_context_data(self, **kwargs):
        context = super(AccountCreateDigestSelection, self).get_context_data(**kwargs)
        return context

    def post(self,request, *args, **kwargs):
        form = DigestSelectionForm(request.POST)
        if form.is_valid():
            self.object = form.save(commit=False)
            self.object.user = self.request.user
            self.object.save()
            url = reverse('account_my_digest')
            return HttpResponseRedirect(url)
        else:
            # Return a re render of the form with error messages on non-conforming fields.
            return render(request, self.template_name, {'form': form})


class AccountMyDigestView(LoginRequiredMixin, TemplateView):
    template_name = 'account/my_digest.html'

    def get_context_data(self, **kwargs):
        context = super(AccountMyDigestView, self).get_context_data(**kwargs)

        # Setup the date in past to compare results against
        utc = pytz.UTC
        current_date = datetime.now()
        current_date = utc.localize(current_date)

        # Define the number of weeks to include in results
        number_of_weeks = 1

        # Create the historical date to compare against i.e. one week ago
        comparison_date = current_date - timedelta(weeks=number_of_weeks)

        # Create a new key on context updated_services_user_selection use Elastic search to query and filter by postcode and category
        context['selected_updated'] = []
        for digest_object in self.request.user.digest_selections.all():
            r = digest_object.retrieve_new_services(comparison_date)
            context['selected_updated'].append({"values": r[:3], "Postcode": digest_object.postcode, "Category": digest_object.category, "pk":digest_object.pk})
        return context


class AccountMyDigestDelete(LoginRequiredMixin, DeleteView):
    model = DigestSelection
    success_url = reverse_lazy('account_my_digest')


    def delete(self, request, *args, **kwargs):
        self.object = self.get_object()
        success_url = self.get_success_url()
        self.object.delete()
        messages.success(
            self.request,
            'Digest for {postcode} and {category} has been successfully deleted.'.format(
                postcode=self.object.postcode,
                category=self.object.category
            )
        )
        return HttpResponseRedirect(success_url)
