from .account import (
    AccountSignupView,
    AccountUpdateView,
    AccountDetailView
)
from .search import SearchView
from .organisation import (
    OrganisationCreateView,
    OrganisationUpdateView,
    OrganisationListView,
    OrganisationDetailView,
    OrganisationDeleteView,
    OrganisationClaimView,
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
    ServiceProblemDetailView,
    ServiceProblemListView,
    ServiceCoverageView
)
from .claim import (
    ClaimListView,
    ClaimDetailView
)
