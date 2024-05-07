using System;
using System.Collections.Generic;

namespace ALISS.CMS.Models.Collection
{
    public class CollectionServicePTO
    {
        public Guid ServiceId { get; set; }
        public string ServiceSlug { get; set; }
        public string Name { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationSlug { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationIsClaimed { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public DateTime LastUpdated { get; set; }
        public IEnumerable<string> Locations { get; set; }
        public IEnumerable<string> ServiceAreas { get; set; }
        public string Url { get; set; }
        public string Phone { get; set; }
    }
}