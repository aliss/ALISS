using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.API
{
    public class ServiceModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string description_formatted { get; set; }
        public Uri url { get; set; }
        public Uri referral_url { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string instagram { get; set; }
        public string summary { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<AccessibilityFeature> accessibility_features { get; set; }
        public IEnumerable<CommunityGroup> community_groups { get; set; }
        public IEnumerable<ServiceArea> service_areas { get; set; }
        public IEnumerable<Media> media_gallery { get; set; }
        public ServiceOrganisation organisation { get; set; }
        public string slug { get; set; }
        public Uri aliss_url { get; set; }
        public Uri permalink { get; set; }
        public bool is_claimed { get; set; }
        public bool is_deprioritised { get; set; }
        public DateTime last_updated { get; set; }
        public DateTime last_reviewed { get; set; }
        public IEnumerable<Location> locations { get; set; }
        public int location_score { get; set; }
    }

    public partial class ServiceOrganisation
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Uri aliss_url { get; set; }
        public Uri permalink { get; set; }
        public bool is_claimed { get; set; }
        public string slug { get; set; }
    }
}