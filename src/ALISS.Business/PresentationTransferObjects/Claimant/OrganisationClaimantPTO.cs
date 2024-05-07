using System;

namespace ALISS.Business.PresentationTransferObjects.Location
{
    public class OrganisationClaimantPTO
    {
        public Guid OrganisationClaimUserId { get; set; }
        public Guid OrganisationId { get; set; }
        public int ClaimantUserId { get; set; }
        public string ClaimantName { get; set; }
        public string ClaimantEmail { get; set; }
        public bool IsLeadClaimant { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}
