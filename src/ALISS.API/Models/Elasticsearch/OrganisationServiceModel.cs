using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class OrganisationServiceModel
    {
        [Keyword]
        public Guid id { get; set; }
        [Keyword]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        [Keyword]
        public string description { get; set; }
        [Keyword]
        public string summary { get; set; }
        [Keyword]
        public string url { get; set; }
        [Keyword]
        public string referral_url { get; set; }
        [Keyword]
        public string phone { get; set; }
        [Keyword]
        public string email { get; set; }
        public string instagram { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        [Boolean]
        public bool is_claimed { get; set; }
        public IEnumerable<ServiceCategoryModel> categories { get; set; }
        public IEnumerable<ServiceAreaElasticSearchModel> service_areas { get; set; }
    }
}