using System;

namespace ALISS.Business.PresentationTransferObjects.Location
{
    public class ServiceLocationPTO
    {
        public Guid LocationId { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        public string Name { get; set; }
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address {  get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }
}
