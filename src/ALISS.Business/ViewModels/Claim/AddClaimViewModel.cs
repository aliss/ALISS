using System;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.ViewModels.Claim
{
    public class AddClaimViewModel
    {
        public Guid? Id { get; set; }
        public string Slug { get; set; }
        public string RepresentativeRole { get; set; }
        public string RepresentativeName { get; set; }
        [Display(Name = "Phone Number", Description = "Please enter a valid UK phone number. The most appropriate email address for someone to contact the organisation with")]
        public string RepresentativePhone { get; set; }
        public bool RequestLeadClaimant { get; set; }
        public int ClaimedUserId { get; set; }
    }
}
