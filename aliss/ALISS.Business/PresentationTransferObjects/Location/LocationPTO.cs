using System;

namespace ALISS.Business.PresentationTransferObjects.Location
{
    public class LocationPTO
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public int LocationCount { get; set; }
    }
}
