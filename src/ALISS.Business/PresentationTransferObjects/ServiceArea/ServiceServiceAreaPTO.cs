using ALISS.Business.PresentationTransferObjects.Category;
using System.Collections.Generic;

namespace ALISS.Business.PresentationTransferObjects.ServiceArea
{
    public class ServiceServiceAreaPTO
    {
        public int ServiceAreaId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public bool Selected { get; set; }
        public List<ServiceServiceAreaPTO> NextLevelServiceAreas { get; set; }
    }
}
