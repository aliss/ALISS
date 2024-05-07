using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.AccessibilityFeature
{
    public class ServiceLocationAccessibilityFeaturePTO
    {
        public string Title { get; set; }
        public Guid? LocationId { get; set; }
        public List<ServiceAccessibilityFeaturePTO> AccessibilityFeatures { get; set; }
    }
}
