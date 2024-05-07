using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class Organisation
    {
        [Key]
        public Guid OrganisationId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }
        [MaxLength(200)]
        public string Facebook { get; set; }
        [MaxLength(200)]
        public string Instagram { get; set; }
        [MaxLength(200)]
        public string Twitter { get; set; }
        public int? ClaimedUserId { get; set; }
        public DateTime? ClaimedOn { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool Published { get; set; }
        [MaxLength(120)]
        public string Slug { get; set; }
        public string Logo { get; set; }
        public string LogoAltText { get; set; }
        public bool Submitted { get; set; }

        [ForeignKey("ClaimedUserId")]
        public virtual UserProfile ClaimedUser { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual UserProfile CreatedUser { get; set; }
        [ForeignKey("UpdatedUserId")]
        public virtual UserProfile UpdatedUser { get; set; }
    }
}
