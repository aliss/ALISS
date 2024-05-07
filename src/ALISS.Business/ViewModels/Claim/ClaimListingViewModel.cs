using ALISS.Business.PresentationTransferObjects.Claim;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Claim
{
    public class ClaimListingViewModel
    {

        public List<ClaimPTO> Claims { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string OrganisationName { get; set; }
        public int TotalResults { get; set; }
    }
}
