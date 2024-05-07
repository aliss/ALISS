using System;

namespace ALISS.Business.PresentationTransferObjects.Location
{
    public class ServiceClaimantPTO
    {
        public Guid ServiceClaimUserId { get; set; }
        public Guid ServiceId { get; set; }
        public int ClaimantUserId { get; set; }
        public string ClaimantName { get; set; }
        public string ClaimantEmail { get; set; }
        public bool IsLeadClaimant { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}
