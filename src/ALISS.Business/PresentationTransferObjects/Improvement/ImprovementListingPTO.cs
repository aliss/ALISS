using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Improvement
{
    public class ImprovementListingPTO
    {
        public Guid ImprovementId { get; set; }
        public string OrganisationName { get; set; }
        public Guid? OrganisationId { get; set; }
        public string ServiceName { get; set; }
        public Guid? ServiceId { get; set; }
        public bool Resolved { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool CanEditService { get; set; }
        public bool CanEditOrganisation { get; set; }
    }
}
