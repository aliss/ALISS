using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.DataInput
{
    public class ContinueAddingPTO
    {
        public Guid OrganisationId { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string OrganisationName { get; set; }
        public int CurrentStep { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
