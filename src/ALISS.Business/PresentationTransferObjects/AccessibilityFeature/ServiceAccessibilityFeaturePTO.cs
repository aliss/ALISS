using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.AccessibilityFeature
{
    public class ServiceAccessibilityFeaturePTO
    {
        public int AccessibilityFeatureId { get; set; }
        public string Name { get; set; }
        public string PromptQuestions { get; set; }
        public string Icon { get; set; }
        public bool Selected { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
