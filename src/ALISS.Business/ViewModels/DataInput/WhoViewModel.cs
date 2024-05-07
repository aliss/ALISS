using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.CommunityGroup;
using ALISS.Models.Models;

namespace ALISS.Business.ViewModels.DataInput
{
    public class WhoViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public List<ServiceCommunityGroupPTO> ServiceCommunityGroups { get; set; }
        public string SelectedCommunityGroups { get; set; }
        public DataInputSummaryTypeEnum SummaryType { get; set; }

    }
}
