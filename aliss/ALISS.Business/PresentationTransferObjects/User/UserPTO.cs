using System;

namespace ALISS.Business.PresentationTransferObjects.User
{
    public class UserPTO
    {
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime DateJoined { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public int ServiceCount { get; set; }
        public int OrganisationCount { get; set; }
        public int ClaimCount { get; set; }
        public bool IsEditor { get; set; }
        public bool IsAdmin { get; set; }



    }
}
