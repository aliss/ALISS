using ALISS.Business.PresentationTransferObjects.Service;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Service
{
    public class ServiceListingViewModel
    {
        public List<ServicePTO> Services { get; set; }
        public string ServiceName { get; set; }
        public string OrganisationName { get; set; }
        public string SearchTerm { get; set; }

        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public bool Unpublished { get; set; }

        public ServiceListingViewModel()
        {
            Services = new List<ServicePTO>();
        }
    }
}
