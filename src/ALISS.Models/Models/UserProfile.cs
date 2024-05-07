using System;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Models.Models
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }
        [MaxLength(254)]
        public string Username { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }
        public bool Active { get; set; }
        public DateTime DateJoined { get; set; }
        public bool AcceptPrivacyPolicy { get; set; }
        public bool AcceptTermsAndConditions { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(10)]
        public string Postcode { get; set; }
        public bool PrepopulatePostcode { get; set; }
    }
}
