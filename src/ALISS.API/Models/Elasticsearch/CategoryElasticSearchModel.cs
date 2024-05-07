using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class CategoryElasticSearchModel
    {
        public int id { get; set; }
        [Keyword]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        [Keyword]
        public string icon { get; set; }
        public int count { get; set; }
        public IEnumerable<CategoryElasticSearchModel> sub_categories { get; set; }
    }
}