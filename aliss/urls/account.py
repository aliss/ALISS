from django.conf.urls import url
from django.contrib.auth import views as auth_views
from django.views.generic import TemplateView

from aliss.views import (
    AccountSignupView,
    AccountUpdateView,
    AccountSavedServicesView,
    AccountMyRecommendationsView,
    AccountMyOrganisationsView,
    AccountMySearchesView
)


urlpatterns = [
    url(r'^signup/$',
        AccountSignupView.as_view(),
        name='signup'
    ),
    url(r'^signup/success/$',
        TemplateView.as_view(template_name="account/signup_success.html"),
        name='signup_success'
    ),
    url(r'^login/$',
        auth_views.login,
        {'template_name': 'account/login.html'},
        name='login'
    ),
    url(r'^logout/$',
        auth_views.logout,
        {'template_name': 'account/logout.html'},
        name='logout'
    ),
    url(r'^password/reset/$',
        auth_views.password_reset,
        {'template_name': 'account/password_reset.html'},
        name='password_reset'
    ),
    url(r'^password/reset/done/$',
        auth_views.password_reset_done,
        {'template_name': 'account/password_reset_done.html'},
        name='password_reset_done'
    ),
    url(r'^password-reset-confirm/(?P<uidb64>[0-9A-Za-z]{1,13})-(?P<token>[0-9A-Za-z]{1,13}-[0-9A-Za-z]{1,20})/$',
        auth_views.password_reset_confirm,
        {'template_name': 'account/password_reset_confirm.html'},
        name='password_reset_confirm'
    ),
    url(r'^password-reset-complete/$',
        auth_views.password_reset_complete,
        {'template_name': 'account/password_reset_complete.html'},
        name='password_reset_complete'),

    url(r'^change-password/$',
        auth_views.password_change,
        {'template_name': 'account/password_change.html'},
        name='password_change'
    ),
    url(r'^change-password-done/$',
        auth_views.password_change_done,
        {'template_name': 'account/password_change_done.html'},
        name='password_change_done'
    ),
    url(r'^update/$',
        AccountUpdateView.as_view(),
        name='account_update'
    ),
    url(r'^saved-services/$',
        AccountSavedServicesView.as_view(),
        name='account_saved_services'
    ),
    url(r'^my-recommendations/$',
        AccountMyRecommendationsView.as_view(),
        name='account_my_recommendations'
    ),
    url(r'^my-organisations/$',
        AccountMyOrganisationsView.as_view(),
        name='account_my_organisations'
    ),
    url(r'^my-searches/$',
        AccountMySearchesView.as_view(),
        name='account_my_searches'
    ),
]
