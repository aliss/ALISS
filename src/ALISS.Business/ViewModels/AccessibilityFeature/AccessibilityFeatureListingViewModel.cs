using ALISS.Business.PresentationTransferObjects.AccessibilityFeature;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.AccessibilityFeature
{
    public class AccessibilityFeatureListingViewModel
    {
        public List<AccessibilityFeaturePTO> AccessibilityFeatures { get; set; }
        public string SearchTerm { get; set; }
        public int TotalResults { get; set; }
    }
}
