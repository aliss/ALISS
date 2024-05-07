using ALISS.Business.PresentationTransferObjects.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.ServiceArea
{
    public class DeleteServiceAreaModel
    {
        public int ServiceAreaId { get; set; }
        public string ServiceAreaName { get; set; }
        public bool CanDelete { get; set; }
        public List<RelatedServicePTO> RelatedServices { get; set; }
    }
}
