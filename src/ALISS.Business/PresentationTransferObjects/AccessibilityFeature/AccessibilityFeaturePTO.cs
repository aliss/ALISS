using System.Collections.Generic;

namespace ALISS.Business.PresentationTransferObjects.AccessibilityFeature
{
    public class AccessibilityFeaturePTO
    {
        public int AccessibilityFeatureId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public int ServiceCount { get; set; }
    }
}
