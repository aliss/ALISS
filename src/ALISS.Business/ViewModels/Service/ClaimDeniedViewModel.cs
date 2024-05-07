using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Service
{
    public class ClaimDeniedViewModel
    {
        public Guid? ClaimId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string Logo { get; set; }
    }
}
