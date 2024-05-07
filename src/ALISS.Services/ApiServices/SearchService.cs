using ALISS.API.Models.Elasticsearch;
using ALISS.ApiServices.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace ALISS.ApiServices.ApiServices
{
    public class SearchService
    {
        private const string _orgUrlPart = "organisations/";
        private const string _servSearchPart = "services/";
        private const string _searchAggs = "searchAggregations/";
        private readonly string apiBaseUrl;

        public SearchService()
        {
            apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
        }

        public OrganisationSearchAPIModel GetOrganisationSearch(OrganisationSearchInputModel input)
		{
            UriBuilder ub = new UriBuilder($"{apiBaseUrl}{_orgUrlPart}");
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            if (input != null && !string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                queryString.Add("searchTerm", input.SearchTerm);
            }
            if (input != null && input.Page.HasValue)
            {
                queryString.Add("page", input.Page.HasValue ? input.Page.Value.ToString() : "1");
            }
            ub.Query = queryString.ToString();

            var request = (HttpWebRequest)WebRequest.Create(ub.Uri);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (OrganisationSearchAPIModel)js.Deserialize(objText, typeof(OrganisationSearchAPIModel));

                    return model;
                }
            }
        }

        public SearchAPIModel GetSearch(SearchInputModel input)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append("?postcode=" + input.Postcode);

            if (String.IsNullOrEmpty(input.Query) == false)
            {
                SB.Append("&q=" + input.Query);
            }

            if (input.Page != null)
            {
                SB.Append("&page=" + input.Page.Value);
            }

            if (input.PageSize != null)
			{
                SB.Append("&page_size=" + input.PageSize.Value);
			}

            if (input.LocationType != null)
            {
                SB.Append("&location_type=" + input.LocationType);
            }

            if (input.Categories != null)
            {
                SB.Append("&categories=" + input.Categories.ToLower());
            }

            if (input.Community_Groups != null)
            {
                SB.Append("&community_groups=" + input.Community_Groups.ToLower());
            }

            if (input.Accessibility_Features != null)
            {
                SB.Append("&accessibility_features=" + input.Accessibility_Features.ToLower());
            }

            if (input.Radius != null && input.Radius.Value > 0)
            {
                SB.Append("&radius=" + input.Radius.Value);
            }

            if (input.Radius != null && input.Radius.Value == 0)
            {
                SB.Append("&radius=" + input.CustomRadius.Value);
            }

            if (String.IsNullOrEmpty(input.Sort) == false)
            {
                SB.Append("&sort=" + input.Sort);
            }

            SearchAPIModel model;

            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}{_servSearchPart}{SB}");
            var aggsRequest = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}{_searchAggs}{SB}");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    model = (SearchAPIModel)js.Deserialize(objText, typeof(SearchAPIModel));
                }
            }

            using (var response = (HttpWebResponse)aggsRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    // This is where the deserialization isn't working. If you just look at the string all the values are coming through.
                    var aggregations = (AggregationResultsModel)js.Deserialize(objText, typeof(AggregationResultsModel));
                    model.aggregations = aggregations.aggregations;
                }
            }

            return model;
        }

        public SearchAPIModel GetSiteMap(int page)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}{_servSearchPart}?page={page}");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (SearchAPIModel)js.Deserialize(objText, typeof(SearchAPIModel));

                    return model;
                }
            }
        }

        public OrganisationSearchAPIModel GetOrganisationSiteMap(int page)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}{_orgUrlPart}?page={page}");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (OrganisationSearchAPIModel)js.Deserialize(objText, typeof(OrganisationSearchAPIModel));

                    return model;
                }
            }
        }

        public IEnumerable<CategoryElasticSearchModel> GetCategories(AggregationModel aggregations)
        {
            var apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}categories");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (CategoriesAPIModel)js.Deserialize(objText, typeof(CategoriesAPIModel));

                    var categories = new List<CategoryElasticSearchModel>();
                    foreach (var cat in model.data)
                    {
                        var category = GetCategoryCount(cat, aggregations);
                        if(category != null)
                        {
                            categories.Add(category);
                        } 
                    }
                    model.data = categories;
                    
                    return model.data;
                }
            }
        }

        public IEnumerable<CommunityGroupElasticSearchModel> GetCommunityGroups(AggregationModel aggregations)
        {
            var apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}community-groups");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (CommunityGroupsAPIModel)js.Deserialize(objText, typeof(CommunityGroupsAPIModel));

                    var groups = new List<CommunityGroupElasticSearchModel>();
                    foreach (var group in model.data)
                    {
                        var communityGroup = GetCommunityGroupCount(group, aggregations);
                        if (communityGroup != null)
                        {
                            groups.Add(communityGroup);
                        }
                    }
                    model.data = groups;

                    return model.data;
                }
            }
        }

        public IEnumerable<AccessibilityFeatureElasticSearchModel> GetAccessibilityFeatures(AggregationModel aggregations)
        {
            var apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
            var request = (HttpWebRequest)WebRequest.Create($"{apiBaseUrl}accessibility-features");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    var model = (AccessibilityFeaturesAPIModel)js.Deserialize(objText, typeof(AccessibilityFeaturesAPIModel));

                    var features = new List<AccessibilityFeatureElasticSearchModel>();
                    foreach (var feature in model.data)
                    {
                        var accessibilityFeature = GetAccessibilityFeatureCount(feature, aggregations);
                        if (accessibilityFeature != null)
                        {
                            features.Add(accessibilityFeature);
                        }
                    }
                    model.data = features;

                    return model.data;
                }
            }
        }

        private CategoryElasticSearchModel GetCategoryCount(CategoryElasticSearchModel category, AggregationModel aggregations)
        {
            category.count = aggregations.cat_agg?.buckets.FirstOrDefault(k => k.key == category.id)?.doc_count ?? 0;

            if (category.sub_categories?.Any() ?? false)
            {
                var newSubCategories = new List<CategoryElasticSearchModel>();
                foreach (var cat in category.sub_categories)
                {
                    var subCategory = GetCategoryCount(cat, aggregations);
                    if (subCategory != null)
                    {
                        newSubCategories.Add(subCategory);
                    }
                }
                category.sub_categories = newSubCategories;
            }
            return category;
        }

        private CommunityGroupElasticSearchModel GetCommunityGroupCount(CommunityGroupElasticSearchModel communityGroup, AggregationModel aggregations)
        {
            communityGroup.count = aggregations.who_agg?.buckets.FirstOrDefault(k => k.key == communityGroup.id)?.doc_count ?? 0;

            if (communityGroup.sub_communitygroups?.Any() ?? false)
            {
                var newSubGroups = new List<CommunityGroupElasticSearchModel>();
                foreach (var group in  communityGroup.sub_communitygroups)
                {
                    var subGroup = GetCommunityGroupCount(group, aggregations);
                    if(subGroup != null)
                    {
                        newSubGroups.Add(subGroup);
                    }
                }
                communityGroup.sub_communitygroups = newSubGroups;
            }
            return communityGroup;
        }

        private AccessibilityFeatureElasticSearchModel GetAccessibilityFeatureCount(AccessibilityFeatureElasticSearchModel feature, AggregationModel aggregations)
        {
            feature.count = aggregations.acc_agg?.buckets.FirstOrDefault(k => k.key == feature.id)?.doc_count ?? 0;

            return feature;
        }
    }
}
