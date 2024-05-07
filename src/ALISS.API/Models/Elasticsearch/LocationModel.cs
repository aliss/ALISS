using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class LocationModel
    {
        [Keyword]
        public Guid id { get; set; }
        [Keyword]
        public string formatted_address { get; set; }
        [Text]
        public string name { get; set; }
        [Text(Analyzer = "description_analyzer")]
        public string description { get; set; }
        [Keyword]
        public string street_address { get; set; }
        [Keyword]
        public string locality { get; set; }
        [Keyword]
        public string region { get; set; }
        [Keyword]
        public string state { get; set; }
        [Keyword]
        public string postal_code { get; set; }
        [Keyword]
        public string country { get; set; }
        [GeoPoint]
        public GeoLocation point { get; set; }
    }
}