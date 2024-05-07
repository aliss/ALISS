using ALISS.Business.PresentationTransferObjects.Organisation;
using System;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Organisation
{
    public class OrganisationListingViewModel
    {
        public List<OrganisationPTO> Organisations { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public bool Unpublished { get; set; }

        public OrganisationListingViewModel()
        {
            Organisations = new List<OrganisationPTO>();
        }
    }
}
