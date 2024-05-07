using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class OrganisationElasticSearchModel
    {
        [Keyword]
        public Guid id { get; set; }
        [Text(Analyzer = "bigram_combiner")]
        public string name { get; set; }
        [Text(Analyzer = "description_analyzer")]
        public string description { get; set; }
        [Boolean]
        public bool published { get; set; }
        [Date]
        public DateTime created_on { get; set; }
        [Date]
        public DateTime last_edited { get; set; }
        [Boolean]
        public bool is_claimed { get; set; }
        [Keyword]
        public string slug { get; set; }
        [Keyword]
        public string url { get; set; }
        [Keyword]
        public string facebook { get; set; }
        [Keyword]
        public string twitter { get; set; }
        [Keyword]
        public string instagram { get; set; }
        [Keyword]
        public string email { get; set; }
        [Keyword]
        public string phone { get; set; }
        public IEnumerable<OrganisationServiceModel> services { get; set; }
        public IEnumerable<LocationModel> locations { get; set; }
    }
}