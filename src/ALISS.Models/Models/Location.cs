using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class Location
    {
        [Key]
        public Guid LocationId { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string Address { get; set; }
        
        [MaxLength(30)]
        public string City { get; set; }
        
        [MaxLength(10)]
        public string Postcode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Guid OrganisationId { get; set; }

        public int? HealthBoardId { get; set; }

        public int? LocalAuthorityId { get; set; }

        public string Ward { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        [ForeignKey("HealthBoardId")]
        public virtual ServiceArea HealthBoard { get; set; }

        [ForeignKey("LocalAuthorityId")]
        public virtual ServiceArea LocalAuthority { get; set; }
    }
}

