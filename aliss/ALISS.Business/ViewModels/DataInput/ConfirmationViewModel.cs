using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.DataInput
{
    public class ConfirmationViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public string ServiceName { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationClaimed { get; set; }
        public string Status { get; set; }
        public bool Suggested { get; set; }
    }
}
