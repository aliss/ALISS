using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Collection
{
    public class CollectionServicePTO
    {
        public Guid ServiceId { get; set; }
        public string ServiceSlug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsClaimed { get; set; }
        public bool CanEdit { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationSlug { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationIsClaimed { get; set; }
        public bool CanEditOrganisation { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public DateTime LastUpdated { get; set; }
        public IEnumerable<Tuple<string, string>> Locations { get; set; }
        public IEnumerable<string> ServiceAreas { get; set; }
        public string Url { get; set; }
        public string Phone { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Email { get; set; }
    }
}
