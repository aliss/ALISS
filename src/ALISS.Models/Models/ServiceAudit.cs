using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceAudit
    {
        [Key]
        public Guid ServiceAuditId { get; set; }
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Guid? OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int? UserProfileId { get; set; }
        public string UserProfileName { get; set; }
        public DateTime DateOfAction { get; set; }
        public string Action { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }  
        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}
