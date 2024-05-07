using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Claim
{
    class ClaimExportPTO
    {
        public DateTime CreatedOn { get; set; }
        public int ClaimedUserId { get; set; }
        public string RepresentativeName { get; set; }
        public string OrganisationName { get; set; }
        public string Email { get; set; }
    }
}
