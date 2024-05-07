using ALISS.Business.PresentationTransferObjects.Location;
using System;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Organisation
{
    public class ServiceClaimantListingViewModel
    {
        public List<ServiceClaimantPTO> Claimants { get; set; }
        public OrganisationClaimantPTO OrganisationLeadClaimant { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string ServiceName { get; set; }
        public int TotalResults { get; set; }
    }
}
