using ALISS.Business.PresentationTransferObjects.ServiceClaim;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.ServiceClaim
{
    public class ServiceClaimListingViewModel
    {

        public List<ServiceClaimPTO> Claims { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string ServiceName { get; set; }
        public int TotalResults { get; set; }
    }
}
