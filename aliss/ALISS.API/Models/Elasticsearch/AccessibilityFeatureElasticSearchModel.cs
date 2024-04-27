using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class AccessibilityFeatureElasticSearchModel
    {
        public int id { get; set; }
        [Keyword]
        public string name { get; set; }
        public string promptquestions { get; set; }
        public string icon { get; set; }
        [Keyword]
        public string slug { get; set; }
        public int displayorder { get; set; }
        public bool is_virtual {get;set;}
        public bool is_physical { get; set; }
        public int count { get; set; }
    }
}