using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class CommunityGroupElasticSearchModel
    {
        public int id { get; set; }
        [Keyword]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        public int displayorder { get; set; }
        public bool isrange { get; set; }
        public int count { get; set; }
        public IEnumerable<CommunityGroupElasticSearchModel> sub_communitygroups { get; set; }
    }
}