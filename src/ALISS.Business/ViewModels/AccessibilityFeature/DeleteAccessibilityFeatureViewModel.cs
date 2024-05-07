using ALISS.Business.PresentationTransferObjects.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.AccessibilityFeature
{
    public class DeleteAccessibilityFeatureViewModel
    {
        public int AccessibilityFeatureId { get; set; }
        public string AccessibilityFeatureName { get; set; }
        public bool CanDelete { get; set; }
        public List<RelatedServicePTO> RelatedServices { get; set; }
    }
}
