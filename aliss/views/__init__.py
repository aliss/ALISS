from .mixins import OrganisationMixin, ProgressMixin
from aliss.views.pdf import printReport
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
    AccountMyReviews,
    AccountMyReviewsApprove,
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
    ServiceDetailEmbeddedMapView,
    ServiceDetailView,
    ServiceDeleteView,
    ServiceReportProblemView,
    ServiceProblemUpdateView,
    ServiceProblemListView,
    ServiceCoverageView,
    ServiceEmailView,
    ServiceAtLocationDelete,
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

from .digest import (
    DigestCreateSelection,
    DigestMyView,
    DigestDelete
)

from .places import (
    PlaceCategoryView,
    PlaceView
)





