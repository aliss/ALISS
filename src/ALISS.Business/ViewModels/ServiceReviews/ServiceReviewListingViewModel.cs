using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.PresentationTransferObjects.ServiceReviews;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.ServiceReviews
{
    public class ServiceReviewListingViewModel
    {
        public List<ReviewPTO> Reviews { get; set; }
        public string ServiceName { get; set; }
        public string SearchTerm { get; set; }

        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }

        public ServiceReviewListingViewModel()
        {
            Reviews = new List<ReviewPTO>();
        }
    }
}
