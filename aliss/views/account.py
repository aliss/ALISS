from django.views.generic import CreateView, UpdateView, DetailView
from django.contrib.auth import authenticate, login
from django.http import HttpResponseRedirect
from django.urls import reverse_lazy

from django_filters.views import FilterView
from braces.views import LoginRequiredMixin

from aliss.models import ALISSUser
from aliss.forms import SignupForm, AccountUpdateForm


class AccountSignupView(CreateView):
    model = ALISSUser
    form_class = SignupForm
    template_name = 'account/signup.html'
    success_url = reverse_lazy('signup_success')

    def form_valid(self, form):
        self.object = ALISSUser.objects.create_user(
            username=form.cleaned_data['username'],
            email=form.cleaned_data['email'],
            password=form.cleaned_data['password1']
        )

        # Authenticate the newly created user
        user = authenticate(
            request=self.request,
            username=form.cleaned_data['email'],
            password=form.cleaned_data['password1']
        )
        login(self.request, user)

        return HttpResponseRedirect(self.get_success_url())


class AccountUpdateView(UpdateView):
    model = ALISSUser
    form_class = AccountUpdateForm
    template_name = 'account/update.html'
    success_url = reverse_lazy('account_update')

    def get_object(self):
        return self.request.user


class AccountDetailView(LoginRequiredMixin, DetailView):
    model = ALISSUser
    template_name = 'account/detail.html'
