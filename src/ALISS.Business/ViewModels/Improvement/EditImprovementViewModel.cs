using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Improvement
{
    public class EditImprovementViewModel
    {
        public Guid? ImprovementId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid ServiceId { get; set; }
        public string SuggestedImprovement { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}