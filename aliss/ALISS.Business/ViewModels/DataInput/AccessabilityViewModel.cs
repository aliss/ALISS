using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.AccessibilityFeature;
using ALISS.Business.PresentationTransferObjects.CommunityGroup;

namespace ALISS.Business.ViewModels.DataInput
{
    public class AccessibilityViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public List<ServiceLocationAccessibilityFeaturePTO> ServiceLocations { get; set; }
        public string SelectedAccessibilityFeatures { get; set; }
        public DataInputSummaryTypeEnum SummaryType { get; set; }

        public AccessibilityViewModel()
        {
            ServiceLocations = new List<ServiceLocationAccessibilityFeaturePTO>();
        }
    }
}
