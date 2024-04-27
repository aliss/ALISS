using ALISS.ApiServices.ViewModels.Organisation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.ApiServices.ViewModels.Service
{
    public class ServiceModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string description_formatted { get; set; }
        public string url { get; set; }
        public string referral_url { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string twitter { get; set; }
        public IEnumerable<AlissServiceCategory> categories { get; set; }
        public IEnumerable<AlissServiceServiceArea> service_areas { get; set; }
        public AlissServiceOrganisation organisation { get; set; }
        public string slug { get; set; }
        public string aliss_url { get; set; }
        public string permalink { get; set; }
        public bool is_claimed { get; set; }
        public bool is_deprioritised { get; set; }
        public DateTime last_updated { get; set; }
        public DateTime last_reviewed { get; set; }
        public DateTime created_on { get; set; }
        public IEnumerable<AlissServiceLocation> locations { get; set; }
        public IEnumerable<AlissServiceAccessibilityFeature> accessibility_features { get; set; }
        public IEnumerable<AlissServiceCommunityGroup> community_groups { get; set; }
        public IEnumerable<AlissServiceMedia> media_gallery { get; set; }
    }

    public class AlissServiceCategory
    {
        public string name { get; set; }
        public string slug { get; set; }
        public bool selected { get; set; }
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
    public class AlissServiceAccessibilityFeature
    {
        public string name { get; set; }
        public string additional_info { get; set; }
        public string icon { get; set; }
        public Guid? location_id { get; set; }
    }
    public class AlissServiceCommunityGroup
    {
        public string name { get; set; }
        public bool is_range { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool selected { get; set; }
    }

    public class AlissServiceMedia
    {
        public Guid id { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
        public string alt_text { get; set; }
        public string type { get; set; }
        public string thumbnail { get; set; }
    }
}
