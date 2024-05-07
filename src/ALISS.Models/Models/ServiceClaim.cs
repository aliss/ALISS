using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceClaim
    {
        [Key]
        public Guid ClaimId { get; set; }
        public Guid? ServiceId { get; set; }
        public string RepresentativeRole { get; set; }
        [MaxLength(50)]
        public string RepresentativeName { get; set; }
        [MaxLength(30)]
        public string RepresentativePhone { get; set; }
        public bool RequestLeadClaimant { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Status { get; set; }
        public int? ClaimedUserId { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedOn { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("ClaimedUserId")]
        public virtual UserProfile ClaimedUser { get; set; }
        [ForeignKey("ReviewedByUserId")]
        public virtual UserProfile ReviewedUser { get; set; }
    }
}
