using System.Collections.Generic;

namespace ALISS.Business.PresentationTransferObjects.CommunityGroup
{
    public class CommunityGroupPTO
	{
        public int CommunityGroupId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public int ServiceCount { get; set; }
        public bool IsMinMax { get; set; }
        public List<CommunityGroupPTO> NextLevelCommunityGroups { get; set; }
    }
}
