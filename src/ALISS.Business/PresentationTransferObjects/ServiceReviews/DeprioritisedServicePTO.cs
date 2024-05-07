using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.ServiceReviews
{
    public class DeprioritisedServicePTO
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public DateTime DeprioritisedDate { get; set; }
        public int CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public int? ClaimedUserId { get; set; }
        public string ClaimedUserName { get; set; }
        public int Managers { get; set; }
        public bool ServicePublished { get; set; }
        public bool OrganisationPublished { get; set; }
    }
}
