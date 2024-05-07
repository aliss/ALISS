using System;

namespace ALISS.Business.PresentationTransferObjects.Service
{
    public class ServicePTO
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationPublished { get; set; }

        public bool ServiceEditable {  get; set; }
        public bool OrganisationEditable {  get; set; }
        public int CreatedUserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedUserName { get; set; }
        public string CreatedUserEmail { get; set; }
        public int? LastUpdatedUserId { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedUserName { get; set; }
        public int LinkedLocations { get; set; }
        public int LinkedAreas { get; set; }
        public bool Published { get; set; }
        public bool Deprioritised { get; set; }
        public bool CanEdit { get; set; }
        public bool CanEditOrganisation { get; set; }
    }
}
