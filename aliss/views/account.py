from django.views.generic import View, CreateView, UpdateView, DetailView, ListView, DeleteView, TemplateView
from django.contrib.auth import authenticate, login, views as auth_views
from django.contrib import messages
from django.http import HttpResponseRedirect
from django.urls import reverse_lazy, reverse
from django.shortcuts import get_object_or_404
from django.contrib.messages.views import SuccessMessageMixin

from django_filters.views import FilterView
from braces.views import LoginRequiredMixin, StaffuserRequiredMixin

from aliss.models import ALISSUser, Service, ServiceArea, Organisation, RecommendedServiceList, ServiceProblem, Claim
from aliss.forms import SignupForm, AccountUpdateForm, RecommendationServiceListForm
from aliss.filters import AccountFilter


def login_view(request, *args, **kwargs):
    if request.method == 'POST':
        if not request.POST.get('remember_me', None):
            request.session.set_expiry(0)
    return auth_views.login(request, *args, **kwargs)


class AccountSignupView(CreateView):
    model = ALISSUser
    form_class = SignupForm
    template_name = 'account/signup.html'
    success_url = reverse_lazy('signup_success')

    def form_valid(self, form):
        self.object = ALISSUser.objects.create_user(
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


class AccountListView(StaffuserRequiredMixin, FilterView):
    template_name = 'account/list.html'
    paginate_by = 10
    filterset_class = AccountFilter


class AccountDetailView(StaffuserRequiredMixin, DetailView):
    model = ALISSUser
    template_name = 'account/detail.html'


class AccountSaveServiceView(LoginRequiredMixin, View):
    def post(self, request, *args, **kwargs):
        service = get_object_or_404(Service, pk=self.kwargs['pk'])

        user = self.request.user

        user.saved_services.add(service)

        next = self.request.POST.get('next', '')
        if next:
            url = next
        else:
            url = reverse('account_saved_services')

        messages.success(self.request, 'Service Added to My Saved Services')

        return HttpResponseRedirect(url)


class AccountRemoveSavedServiceView(View):
    def post(self, request, *args, **kwargs):
        service = get_object_or_404(Service, pk=self.kwargs['pk'])

        user = self.request.user

        user.saved_services.remove(service)

        next = self.request.POST.get('next', '')
        if next:
            url = next
        else:
            url = reverse('account_saved_services')

        messages.success(self.request, 'Service Removed from My Saved Services')

        return HttpResponseRedirect(url)


class AccountServiceHelpfulView(LoginRequiredMixin, View):
    def post(self, request, *args, **kwargs):
        service = get_object_or_404(Service, pk=self.kwargs['pk'])

        user = self.request.user

        if service in user.helpful_services.all():
            messages.success(
                self.request,
                'Service removed from helpful services'
            )
            user.helpful_services.remove(service)
        else:
            messages.success(
                self.request,
                'Service marked as helpful'
            )
            user.helpful_services.add(service)

        next = self.request.POST.get('next', '')
        if next:
            url = next
        else:
            url = reverse('service_detail', kwargs={'pk': service.pk})

        return HttpResponseRedirect(url)


class AccountSavedServicesView(LoginRequiredMixin, TemplateView):
    template_name = 'account/saved_services.html'


class AccountMyRecommendationsView(LoginRequiredMixin, CreateView):
    form_class = RecommendationServiceListForm
    template_name = 'account/my_recommendations.html'
    success_url = reverse_lazy('account_my_recommendations')

    def form_valid(self, form):
        self.object = form.save(commit=False)
        self.object.user = self.request.user
        self.object.save()
        form.save_m2m()

        return HttpResponseRedirect(self.get_success_url())

    def get_context_data(self, **kwargs):
        context = super(AccountMyRecommendationsView, self).get_context_data(**kwargs)
        context['recommended_service_lists'] = RecommendedServiceList.objects.filter(user=self.request.user)
        return context


class AccountRecommendationListDetailView(LoginRequiredMixin, DetailView):
    model = RecommendedServiceList
    template_name = 'account/recommendation_list.html'


class AccountRecommendationListDeleteView(LoginRequiredMixin,SuccessMessageMixin, DeleteView):
    model = RecommendedServiceList
    success_url = reverse_lazy('account_my_recommendations')
    success_message = "%(name)s recommendation list was deleted successfully"

    def get_queryset(self):
        return RecommendedServiceList.objects.filter(user=self.request.user)


class AccountRecommendationListAddServiceView(LoginRequiredMixin, View):
    def post(self, request, *args, **kwargs):
        recommendation_list = get_object_or_404(
            RecommendedServiceList,
            user=self.request.user,
            pk=self.request.POST.get('recommendation_list')
        )
        service = get_object_or_404(
            Service,
            pk=self.request.POST.get('service')
        )

        recommendation_list.services.add(service)

        next = self.request.POST.get('next', '')
        if next:
            url = next
        else:
            url = reverse(
                'account_my_recommendations_detail',
                kwargs={'pk': recommendation_list.pk}
            )

        messages.success(self.request, 'Service added to {name} list'.format(name=recommendation_list.name))

        return HttpResponseRedirect(url)


class AccountRecommendationListRemoveServiceView(View):
    def post(self, request, *args, **kwargs):
        recommendation_list = get_object_or_404(
            RecommendedServiceList,
            user=self.request.user,
            pk=self.request.POST.get('recommendation_list')
        )
        service = get_object_or_404(
            Service,
            pk=self.request.POST.get('service')
        )

        recommendation_list.services.remove(service)

        next = self.request.POST.get('next', '')
        if next:
            url = next
        else:
            url = reverse(
                'account_my_recommendations_detail',
                kwargs={'pk': recommendation_list.pk}
            )

        messages.success(self.request, 'Service removed from {name} list'.format(name=recommendation_list.name))

        return HttpResponseRedirect(url)


class AccountMyOrganisationsView(LoginRequiredMixin, ListView):
    template_name = 'account/my_organisations.html'

    def get_queryset(self):
        return Organisation.objects.filter(claimed_by=self.request.user)

class AccountMySearchesView(LoginRequiredMixin, TemplateView):
    template_name = 'account/my_searches.html'

class AccountAdminDashboard(StaffuserRequiredMixin, TemplateView):
    template_name = 'account/dashboard.html'

    def get_context_data(self, **kwargs):
        context = super(AccountAdminDashboard, self).get_context_data(**kwargs)

        from datetime import datetime
        current_month = datetime.now().month

        context['service_month_count'] = Service.objects.filter(created_on__month=current_month).count()
        context['orgs_month_count'] = Organisation.objects.filter(created_on__month=current_month).count()
        context['user_month_count'] = ALISSUser.objects.filter(date_joined__month=current_month).count()
        context['problem_month_count'] = ServiceProblem.objects.filter(created_on__month=current_month).count()
        context['claim_request_count'] = Claim.objects.filter(created_on__month=current_month).count()

        context['services'] = Service.objects\
            .prefetch_related('locations', 'service_areas').all()
        context['service_areas'] = ServiceArea.objects\
            .prefetch_related('services')\
            .order_by('type', 'code')
        context['recently_added'] = Service.objects.all().order_by('-created_on')[:10]

        return context
