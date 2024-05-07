using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class OrganisationClaimUser
    {
        [Key]
        public Guid OrganisationClaimUserId { get; set; }
        public Guid? OrganisationId { get; set; }
        public int? ClaimedUserId { get; set; }
        public Guid? ClaimId { get; set; }
        public bool IsLeadClaimant { get; set; }
        public DateTime ApprovedOn { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
        [ForeignKey("ClaimedUserId")]
        public virtual UserProfile ClaimedUser { get; set; }
        [ForeignKey("ClaimId")]
        public virtual Claim Claim { get; set; }
    }
}
