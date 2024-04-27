using ALISS.Business.PresentationTransferObjects.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.CommunityGroup
{
    public class DeleteCommunityGroupViewModel
    {
        public int CommunityGroupId { get; set; }
        public string CommunityGroupName { get; set; }
        public bool CanDelete { get; set; }
        public List<RelatedServicePTO> RelatedServices { get; set; }
    }
}
