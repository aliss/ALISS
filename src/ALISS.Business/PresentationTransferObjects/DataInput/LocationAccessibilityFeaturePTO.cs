using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Models.Models;

namespace ALISS.Business.PresentationTransferObjects.DataInput
{
    public class LocationAccessibilityFeaturePTO
    {
        public string Title { get; set; }
        public List<ServiceAccessibilityFeature> accessibilityFeatures { get; set; }
    }
}
