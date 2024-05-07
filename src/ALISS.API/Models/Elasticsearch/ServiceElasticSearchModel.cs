using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.Elasticsearch
{
    public class ServiceElasticSearchModel
    {
        [Keyword]
        public Guid id { get; set; }
        public ServiceOrganisationModel organisation { get; set; }
        [Date]
        public DateTime created_on { get; set; }
        [Date]
        public DateTime last_edited { get; set; }
        [Date]
        public DateTime last_reviewed { get; set;}
        [Boolean]
        public bool is_claimed { get; set; }
        [Boolean]
        public bool is_deprioritised { get; set;}
        [Keyword]
        public string slug { get; set; }
        [Text(Analyzer = "english")]
        public string name { get; set; }
        [Text(Analyzer = "english")]
        public string description { get; set; }
        [Text(Analyzer = "english")]
        public string summary { get; set; }
        [Keyword]
        public string facebook { get; set; }
        [Keyword]
        public string instagram { get; set; }
        [Keyword]
        public string twitter { get; set; }
        [Keyword]
        public string url { get; set; }
        [Keyword]
        public string referral_url { get; set; }
        [Keyword]
        public string phone { get; set; }
        [Keyword]
        public string email { get; set; }
        [Number(NumberType.Integer)]
        public int location_score { get; set; }
        public IEnumerable<ServiceCategoryModel> categories { get; set; }
        public IEnumerable<ServiceCategoryModel> parent_categories { get; set; }
        public IEnumerable<LocationModel> locations { get; set; }
        public IEnumerable<ServiceAreaElasticSearchModel> service_areas { get; set; }
        public IEnumerable<ServiceAccessibilityFeatureModel> accessibility_features { get; set; }
        public IEnumerable<ServiceCommunityGroupModel> community_groups { get; set; }
        public IEnumerable<ServiceMedia> media_gallery { get; set; }
    }

    public class ServiceOrganisationModel
    {
        [Keyword]
        public Guid id { get; set; }
        [Text]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        [Boolean]
        public bool is_claimed { get; set; }
    }

    public class ServiceCategoryModel
    {
        public int id { get; set; }
        [Text(Analyzer = "english")]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        public bool selected { get; set; }
    }

    public class AggregationResultsModel
    {
        public AggregationModel aggregations { get; set; }

        public AggregationResultsModel()
        {
            aggregations = new AggregationModel();
        }
    }

    public class AggregationModel
    {
        public FilterModel cat_agg { get; set; }
        public FilterModel who_agg { get; set; }
        public FilterModel acc_agg { get; set; }

        public AggregationModel()
        {
            cat_agg = new FilterModel();
            who_agg = new FilterModel();
            acc_agg = new FilterModel();
        }
    }

    public class FilterModel
    {
        public IEnumerable<BucketModel> buckets { get; set; }

        public FilterModel()
        {
            buckets = new List<BucketModel>();
        }
    }

    public class BucketModel
    {
        public int key { get; set; }
        public int doc_count { get; set; }
    }

    public class ServiceAreaElasticSearchModel
    {
        public int id { get; set; }
        [Keyword]
        public string code { get; set; }
        [Keyword]
        public string type { get; set; }
        [Keyword]
        public string name { get; set; }
        public string geojson { get; set; }
    }

    public class ServiceAccessibilityFeatureModel
    {
        public int id { get; set; }
        [Text(Analyzer = "english")]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        [Text(Analyzer = "english")]
        public string additional_info { get; set; }
        public string icon { get; set; }
        public Guid? location_id { get; set; }
    }

    public class ServiceCommunityGroupModel
    {
        public int id { get; set; }
        [Text(Analyzer = "english")]
        public string name { get; set; }
        [Keyword]
        public string slug { get; set; }
        public bool is_range { get; set; }
        public int min_value { get; set; }
        public int max_value { get; set;}
        public bool selected { get; set; }
    }

    public class ServiceMedia
    {
        [Keyword]
        public Guid id { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
        public string alt_text { get; set; }
        public string type { get; set; }
        public string thumbnail { get; set; }
    }
}