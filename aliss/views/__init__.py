from .mixins import OrganisationMixin, ProgressMixin
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
    AccountRecommendationListDeleteView,
    AccountRecommendationListPrintView,
    AccountIsEditor,
    AccountMyDigestView,
    AccountCreateDigestSelection,
    AccountMyDigestDelete
)
from .search import (
    SearchView,
    SearchShareView,
)

from .organisation import *
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
    ServiceCoverageView,
    ServiceEmailView
)
from .claim import (
    ClaimListView,
    ClaimDetailView,
    ClaimCreateView,
    ClaimDeleteView
)
from .reports import (
    ReportsView,
)
