using ALISS.Business.PresentationTransferObjects.CommunityGroup;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.CommunityGroup
{
    public class CommunityGroupListingViewModel
    {
        public List<CommunityGroupPTO> CommunityGroups { get; set; }
        public string SearchTerm { get; set; }
        public int TotalResults { get; set; }
    }
}
