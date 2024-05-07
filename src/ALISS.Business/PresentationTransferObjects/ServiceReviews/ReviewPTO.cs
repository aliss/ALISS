using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.PresentationTransferObjects.Service;

namespace ALISS.Business.PresentationTransferObjects.ServiceReviews
{
    public class ReviewPTO
    {
        public DateTime LastReviewedDate { get; set; }
        public int ReviewStatus { get; set; }
        public ServicePTO Service { get; set; }
    }
}
