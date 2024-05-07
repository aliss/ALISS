using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class OrganisationAudit
    {
        [Key]
        public Guid OrganisationAuditId { get; set; }
        public Guid? OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int? UserProfileId { get; set; }
        public string UserProfileName { get; set; }
        public DateTime DateOfAction { get; set; }
        public string Action { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}
