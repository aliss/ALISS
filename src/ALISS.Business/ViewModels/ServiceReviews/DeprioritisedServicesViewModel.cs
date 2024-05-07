using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.PresentationTransferObjects.ServiceReviews;

namespace ALISS.Business.ViewModels.ServiceReviews
{
    public class DeprioritisedServicesViewModel
    {
        public List<DeprioritisedServicePTO> Services { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }

        public DeprioritisedServicesViewModel() 
        {
            Services = new List<DeprioritisedServicePTO>();
        }
    }
}
