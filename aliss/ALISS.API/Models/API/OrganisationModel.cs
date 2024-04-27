using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.API
{
    public class OrganisationModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Uri alissUrl { get; set; }
        public Uri permalink { get; set; }
        public bool is_claimed { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public string description_formatted { get; set; }
        public Uri facebook { get; set; }
        public Uri twitter { get; set; }
        public Uri instagram { get; set; }
        public Uri url { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime last_edited { get; set; }
        public IEnumerable<Service> services { get; set; }
        public IEnumerable<Location> locations { get; set; }
    }

    public partial class Location
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
    public partial class Service
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public Uri url { get; set; }
        public Uri referral_url { get; set; }
        public string phone { get; set; }
        public Uri facebook { get; set; }
        public Uri twitter { get; set; }
        public Uri instagram { get; set; }
        public string email { get; set; }
        public bool is_claimed { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<AccessibilityFeature> accessibility_features { get; set; }
        public IEnumerable<CommunityGroup> community_groups { get; set; }
        public IEnumerable<Media> media_galery { get; set; }
        public IEnumerable<ServiceArea> service_areas { get; set; }
    }

    public partial class Category
    {
        public string name { get; set; }
        public string slug { get; set; }
        public bool selected { get; set; }
    }
    public partial class ServiceArea
    {
        public string code { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }
    public partial class AccessibilityFeature
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string additional_info { get; set; }
        public string icon { get; set; }
        public Guid? location_id { get; set; }
    }
    public partial class CommunityGroup
    {
        public string name { get; set; }
        public string slug { get; set; }
        public bool is_range { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool selected { get; set; }

    }
    public partial class Media
    {
        public Guid id { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
        public string alt_text { get; set; }
        public string type { get; set; }
        public string thumbnail { get; set; }

    }
}