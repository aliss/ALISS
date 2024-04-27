using System;

namespace ALISS.Business.PresentationTransferObjects.Claim
{
    public class ClaimPTO
    {
        public Guid ClaimId { get; set; }
        public Guid OrganisationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
        public int ClaimedUserId { get; set; }
        public string ClaimedUserName { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationPublished { get; set; }
        public int? ExistingClaimedUserId { get; set; }
        public string ExistingClaimedUserName { get; set; }
    }
}

