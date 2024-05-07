using ALISS.Business.PresentationTransferObjects.Location;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Organisation
{
    public class OrganisationClaimantListingViewModel
    {
        public List<OrganisationClaimantPTO> Claimants { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string OrganisationName { get; set; }
        public int TotalResults { get; set; }
    }
}
