using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Improvement
{
    public class ViewImprovementViewModel
    {
        public Guid ImprovementId { get; set; }
        public Guid? ServiceId { get; set; }
        [Display(Name="Service Name")]
        public string ServiceName { get; set; }
        public Guid? OrganisationId { get; set; }
        [Display(Name = "Organisation Name")]
        public string OrganisationName { get; set; }
        [Display(Name = "Suggested Improvement")]
        public string SuggestedImprovement { get; set; }
        [Display(Name = "Submitted By Name")]
        public string Name { get; set; }
        [Display(Name = "Submitted By Email")]
        public string Email { get; set; }
        [Display(Name = "Has This Improvement Been Implemented?")]
        public string Resolved { get; set; }
    }
}
