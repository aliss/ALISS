using System;

namespace ALISS.Business.PresentationTransferObjects.Organisation
{
    public class OrganisationPTO
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedUserId { get; set; }
        public string UpdatedUserName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? ClaimedUserId { get; set; }
        public string ClaimedUserName { get; set; }
        public DateTime? ClaimedOn { get; set; }
        public bool Published { get; set; }
        public bool CanEdit { get; set; }
    }
}
