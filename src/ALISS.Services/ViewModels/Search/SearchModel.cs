using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class SearchModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public IEnumerable<AlissServiceCategory> categories { get; set; }
        public IEnumerable<AlissServiceServiceArea> service_areas { get; set; }
        public AlissServiceOrganisation organisation { get; set; }
        public string slug { get; set; }
        public string aliss_url { get; set; }
        public string permalink { get; set; }
        public DateTime last_reviewed { get; set; }
        public DateTime last_updated { get; set; }
        public IEnumerable<AlissServiceLocation> locations { get; set; }
    }

    public class AlissServiceCategory
    {
        public string name { get; set; }
        public string slug { get; set; }
    }

    public class AlissServiceLocation
    {
        public Guid id { get; set; }
        public string formatted_address { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string street_address { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class AlissServiceOrganisation
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string aliss_url { get; set; }
        public string permalink { get; set; }
        public bool is_claimed { get; set; }
        public string slug { get; set; }
    }

    public class AlissServiceServiceArea
    {
        public string code { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }
}
