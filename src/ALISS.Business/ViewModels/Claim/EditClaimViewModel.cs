using ALISS.Business.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Tactuum.Core.Attributes;

namespace ALISS.Business.ViewModels.Claim
{
    public class EditClaimViewModel
    {
        public Guid ClaimId { get; set; }
        public Guid OrganisationId { get; set; }
        [Display(Name = "Representative's Role", Description = "The role the representative has in the organisation being claimed.")]
        public string RepresentativeRole { get; set; }
        [MaxLength(50)]
        [Display(Name = "Representative's Name", Description = "The name of the person wanting to represent the organisation.")]
        public string RepresentativeName { get; set; }
        [MaxLength(30)]
        [Display(Name = "Representative's Phone Number", Description = "The phone number of the person wanting to represent the organisation.")]
        public string RepresentativePhone { get; set; }
        [Display(Name = "Request to be Lead Contact", Description = "Does this claim include a request to be the Claimed User and lead contact of the organisation?")]
        public bool RequestLeadClaimant { get; set; }
        public DateTime CreatedOn { get; set; }
        public ClaimStatusEnum Status { get; set; }
        public int ClaimedUserId { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedOn { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Organisation Name", Description = "The official name of the organisation")]
        public string OrganisationName { get; set; }
        [Display(Name = "Name", Description = "The name of the claimant of the organisation")]
        public string UserName { get; set; }
        [MaxLength(254)]
        [Display(Description = "The most appropriate email address for someone to contact the organisation with")]
        [ValidEmailAddress]
        public string Email { get; set; }
    }
}