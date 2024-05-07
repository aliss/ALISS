using ALISS.Business.PresentationTransferObjects.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.ViewModels.Service
{
    public class MakeLeadServiceClaimantViewModel
    {
        public ServiceClaimantPTO Claimant { get; set; }

        [Required]
        [Display(Name = "Email Message", Description = "The custom message to be sent to the new lead claimant.")]
        public string Message { get; set; }

        [Required]
        [Display(Description = "Confirm that you understand that the lead claimant of this service will change.")]
        public bool Confirmation { get; set; }
    }
}
