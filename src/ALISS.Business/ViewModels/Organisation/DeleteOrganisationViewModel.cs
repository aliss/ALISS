using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Organisation
{
    public class DeleteOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
    }
}
