using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.CommunityGroup
{
    public class ServiceCommunityGroupPTO
    {
        public int CommunityGroupId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Selected { get; set; }
        public bool IsMinMax { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public int DisplayOrder { get; set; }
        public List<ServiceCommunityGroupPTO> NextLevelCommunityGroups { get; set; }
    }
}
