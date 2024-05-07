using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceClaimUser
    {
        [Key]
        public Guid ServiceClaimUserId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? ClaimedUserId { get; set; }
        public Guid? ServiceClaimId { get; set; }
        public bool IsLeadClaimant { get; set; }
        public DateTime ApprovedOn { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("ClaimedUserId")]
        public virtual UserProfile ClaimedUser { get; set; }
        [ForeignKey("ServiceClaimId")]
        public virtual ServiceClaim Claim { get; set; }
    }
}
