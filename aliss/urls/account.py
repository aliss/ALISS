from django.conf.urls import url
from django.contrib.auth import views as auth_views
from django.views.generic import TemplateView

from aliss.views import (
    login_view,
    AccountSignupView,
    AccountUpdateView,
    AccountSavedServicesView,
    AccountMyRecommendationsView,
    AccountMyOrganisationsView,
    AccountMySearchesView,
    AccountSaveServiceView,
    AccountRemoveSavedServiceView,
    AccountAdminDashboard,
    AccountListView,
    AccountDetailView,
    AccountServiceHelpfulView,
    AccountRecommendationListDetailView,
    AccountRecommendationListAddServiceView,
    AccountRecommendationListRemoveServiceView,
    AccountRecommendationListDeleteView,
    AccountRecommendationListPrintView,
    AccountIsEditor,
    AccountMyReviews,
    AccountMyReviewsApprove,
)

from aliss.views import (
    DigestCreateSelection,
    DigestMyView,
    DigestDelete,
)


urlpatterns = [
    url(r'^signup/$',
        AccountSignupView.as_view(),
        name='signup'
    ),
    url(r'^notifications/$',
        DigestMyView.as_view(),
        name='account_my_digest'
    ),
    url(r'^digests/create$',
        DigestCreateSelection.as_view(),
        name='account_create_my_digest'
    ),
    url(r'^digests/(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        DigestDelete.as_view(),
        name='account_my_digest_delete'
    ),
    url(r'^signup/success/$',
        TemplateView.as_view(template_name="account/signup_success.html"),
          auth_views.password_reset,
        {
            'template_name': 'account/welcome.html',
            'html_email_template_name': 'account/emails/welcome.html',
        },
        name='signup_success'
    ),
      
    url(r'^login/$',
        login_view,
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
        {
            'template_name': 'account/password_reset.html',
            'html_email_template_name': 'account/emails/password_reset_email.html'
        },
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
    url(r'^my-reviews/(?P<pk>[0-9A-Za-z\-]+)/$',
        AccountMyReviewsApprove.as_view(),
        name='account_my_reviews_approve'
    ),
    url(r'^my-reviews/$',
        AccountMyReviews.as_view(),
        name='account_my_reviews'
    ),
    url(r'^my-recommendations/email/$',
        AccountRecommendationListPrintView.as_view(),
        name='account_my_recommendations_email'
    ),
    url(r'^my-recommendations/(?P<pk>[0-9A-Za-z\-]+)/$',
        AccountRecommendationListDetailView.as_view(),
        name='account_my_recommendations_detail'
    ),
    url(r'^my-recommendations/(?P<pk>[0-9A-Za-z\-]+)/delete/$',
        AccountRecommendationListDeleteView.as_view(),
        name='account_my_recommendations_delete'
    ),
    url(r'^my-recommendations/service/add/$',
        AccountRecommendationListAddServiceView.as_view(),
        name='account_my_recommendations_add_service'
    ),
    url(r'^my-recommendations/service/remove/$',
        AccountRecommendationListRemoveServiceView.as_view(),
        name='account_my_recommendations_remove_service'
    ),
    url(r'^my-organisations/$',
        AccountMyOrganisationsView.as_view(),
        name='account_my_organisations'
    ),
    url(r'^my-searches/$',
        AccountMySearchesView.as_view(),
        name='account_my_searches'
    ),
    url(r'^saved-services/add/(?P<pk>[0-9A-Za-z\-]+)/$',
        AccountSaveServiceView.as_view(),
        name='account_save_service'
    ),
    url(r'^saved-services/remove/(?P<pk>[0-9A-Za-z\-]+)/$',
        AccountRemoveSavedServiceView.as_view(),
        name='account_remove_saved_service'
    ),
    url(r'^service/(?P<pk>[0-9A-Za-z\-]+)/helpful/$',
        AccountServiceHelpfulView.as_view(),
        name='account_service_helpful'
    ),
    url(r'^dashboard/$',
        AccountAdminDashboard.as_view(),
        name='account_dashboard'
    ),
    url(r'^list/$',
        AccountListView.as_view(),
        name='account_list'
    ),
    url(r'^(?P<pk>[0-9A-Za-z\-]+)/$',
        AccountDetailView.as_view(),
        name='account_detail'
    ),
    url(r'^user/(?P<pk>.+)/editor/$',
        AccountIsEditor.as_view(),
        name='account_is_editor'
    )
]
