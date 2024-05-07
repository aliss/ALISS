using System;

namespace ALISS.Business.PresentationTransferObjects.ServiceClaim
{
    public class ServiceClaimPTO
    {
        public Guid ClaimId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
        public int ClaimedUserId { get; set; }
        public string ClaimedUserName { get; set; }
        public string ServiceName { get; set; }
        public bool ServicePublished { get; set; }
        public int? ExistingClaimedUserId { get; set; }
        public string ExistingClaimedUserName { get; set; }
    }
}

