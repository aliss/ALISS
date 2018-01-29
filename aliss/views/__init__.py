from .mixins import OrganisationMixin
from .account import (
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
    AccountRecommendationListDeleteView
)
from .search import SearchView, SearchShareView
from .organisation import (
    OrganisationCreateView,
    OrganisationUpdateView,
    OrganisationListView,
    OrganisationDetailView,
    OrganisationDeleteView,
    OrganisationSearchView,
    OrganisationUnPublishedView
)
from .location import (
    LocationCreateView,
    LocationUpdateView,
    LocationDetailView,
    LocationDeleteView
)
from .service import (
    ServiceCreateView,
    ServiceUpdateView,
    ServiceDetailView,
    ServiceDeleteView,
    ServiceReportProblemView,
    ServiceProblemUpdateView,
    ServiceProblemListView,
    ServiceCoverageView
)
from .claim import (
    ClaimListView,
    ClaimDetailView,
    ClaimCreateView
)
