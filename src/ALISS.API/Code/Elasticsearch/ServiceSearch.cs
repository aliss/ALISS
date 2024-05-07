using ALISS.API.Models.Elasticsearch;
using ALISS.API.Models.External;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.WebPages;

namespace ALISS.API.Code.Elasticsearch
{
    public class ServiceSearch : ElasticSearchBase
    {
        public ServiceSearch() : base()
        {

        }

        public void AddService(ServiceElasticSearchModel service)
        {
            var client = GetClient(services);
            if (!client.DocumentExists<ServiceElasticSearchModel>(service).Exists)
            {
                _ = client.IndexDocument(service);
            }
            else
            {
                _ = client.Index(service, default);
            }
        }

        public void AddServices(IEnumerable<ServiceElasticSearchModel> servicesToAdd)
        {
            var client = GetClient(services);
            BulkResponse response = client.IndexMany(servicesToAdd);
            if (response.Errors || response.ItemsWithErrors.Any())
            {
                var errors = response.ItemsWithErrors;
            }
        }

        public async Task AddServiceAsync(ServiceElasticSearchModel service)
        {
            var client = GetClient(services);
            if (!client.DocumentExists<ServiceElasticSearchModel>(service).Exists)
            {
                _ = await client.IndexDocumentAsync(service);
            }
            else
            {
                _ = await client.IndexAsync(service, default);
            }
        }

        public async Task AddServicesAsync(IEnumerable<ServiceElasticSearchModel> servicesToAdd)
        {
            var client = GetClient(services);
            foreach (ServiceElasticSearchModel service in servicesToAdd)
            {
                if (!client.DocumentExists<ServiceElasticSearchModel>(service).Exists)
                {
                    _ = await client.IndexDocumentAsync(service);
                }
                else
                {
                    _ = await client.IndexAsync(service, default);
                }
            }
        }

        public ServiceElasticSearchModel GetById(Guid id)
        {
            ElasticClient client = GetClient(services);

            DocumentPath<ServiceElasticSearchModel> service = DocumentPath<ServiceElasticSearchModel>.Id(id);
            IGetResponse<ServiceElasticSearchModel> response = client.Get(service);

            return response?.Source;
        }

        public async Task<ServiceElasticSearchModel> GetByIdAsync(Guid id)
        {
            ElasticClient client = GetClient(services);

            DocumentPath<ServiceElasticSearchModel> service = DocumentPath<ServiceElasticSearchModel>.Id(id);
            IGetResponse<ServiceElasticSearchModel> response = await client.GetAsync(service);

            return response?.Source;
        }

        public async Task<ServiceElasticSearchModel> GetBySlugAsync(string slug)
        {
            ElasticClient client = GetClient(services);

            var searchRequest = await client.SearchAsync<ServiceElasticSearchModel>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.slug)
                        .Query(slug)
                        )
                    )
                );

            return searchRequest?.Documents.FirstOrDefault();
        }

        public async Task<ISearchResponse<ServiceElasticSearchModel>> GetAllServicesAsync(int page = 1)
        {
            ElasticClient client = GetClient(services);

            ISearchResponse<ServiceElasticSearchModel> response = await client.SearchAsync<ServiceElasticSearchModel>(s => s
                .MatchAll()
                .Sort(ss => ss.Ascending(i => i.id))
                .Skip((page - 1) * 20)
                .Take(20)
            );

            return response;
        }

        private QueryContainer GetSearchQuery(string searchTerm, string categories, string community_groups, string accessibility_features, string location_type, Postcode postcode, int radius)
        {
            QueryContainer query = new QueryContainer();
            categories = string.IsNullOrEmpty(categories) ? "" : categories.Replace(" ", "");
            community_groups = string.IsNullOrEmpty(community_groups) ? "" : community_groups.Replace(" ", "");
            accessibility_features = string.IsNullOrEmpty(accessibility_features) ? "" : accessibility_features.Replace(" ", "");

            query = String.IsNullOrEmpty(searchTerm) ? query : query & FilterServicesBySearchTerm(searchTerm);

            if (!String.IsNullOrEmpty(categories))
            {
                List<string> categoryList = categories.Split(';').ToList();
                foreach (var category in categoryList)
                {
                    query &= FilterByCategory(category);
                }
            }

            if (!String.IsNullOrEmpty(community_groups))
            {
                List<string> communityGroupsList = community_groups.Split(';').ToList();
                foreach (var communityGroup in communityGroupsList)
                {
                    string[] communityGroupArray = communityGroup.Split('|');
                    if (communityGroupArray.Length == 2)
                    {
                        query &= FilterByCommunityGroup(communityGroupArray[0]);
                        query &= FilterByCommunityGroupRange(Int32.Parse(communityGroupArray[1]));
                    }
                    else
                    {
                        query &= FilterByCommunityGroup(communityGroup);
                    }
                }
            }

            if (!String.IsNullOrEmpty(accessibility_features))
            {
                List<string> accessibilityFeaturesList = accessibility_features.Split(';').ToList();
                foreach (var accessiblityFeature in accessibilityFeaturesList)
                {
                    query &= FilterByAccessibilityFeature(accessiblityFeature);
                }
            }

            query = location_type == null ? query : query & FilterByLocationType(location_type);
            if (postcode != null && (String.IsNullOrEmpty(location_type) || location_type == "local"))
            {
                query &= FilterByPostcode(postcode, radius);
            }

            return query;
        }

        public async Task<AggregationResultsModel> GetSearchAggregationsAsync(Postcode postcode, string searchTerm, string categories = "", string locationType = null, int radius = 5000, int page = 1, int pageSize = 10, string sort = "", string community_groups = "", string accessibility_features = "")
        {
            ElasticClient client = GetClient(services);

            ISearchRequest<ServiceElasticSearchModel> searchRequest = new SearchRequest<ServiceElasticSearchModel>()
            {
                Query = GetSearchQuery(searchTerm, categories, community_groups, accessibility_features, locationType, postcode, radius),
                Size = 0,
                Aggregations = new AggregationDictionary
                {
                    {
                    "cat_agg",
                    new AggregationContainer
                    {
                            Terms = new TermsAggregation("cat_agg")
                            {
                                Field = new Field("categories.id"),
                                Size = 2000
                            }
                        }
                    },
                    {
                        "who_agg",
                        new AggregationContainer
                        {
                            Terms = new TermsAggregation("who_agg")
                            {
                                Field = new Field("community_groups.id"),
                                Size = 2000
                            }
                        }
                    },
                    {
                        "acc_agg",
                        new AggregationContainer
                        {
                            Terms = new TermsAggregation("acc_agg")
                            {
                                Field = new Field("accessibility_features.id"),
                                Size = 2000
                            }
                        }
                    }
                },
            };
            
            var searchResults = await client.LowLevel.SearchAsync<StringResponse>(
                ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + services,
                PostData.Serializable(searchRequest)
            );

            var results = Json.Decode<AggregationResultsModel>(searchResults.Body);

            return results;
        }

        public async Task<List<ServiceElasticSearchModel>> SearchServicesByCategoryIdAsync(int categoryId)
        {
            ElasticClient client = GetClient(services);

            BoolQuery categoryQuery = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.categories.Select(c => c.id)),
                        Value = categoryId
                    }
                }
            };

            ISearchRequest<ServiceElasticSearchModel> searchRequest = new SearchRequest<ServiceElasticSearchModel>()
            {
                Query = categoryQuery,
                From = 0,
                Size = 10000
            };

            ISearchResponse<ServiceElasticSearchModel> searchResults = await client.SearchAsync<ServiceElasticSearchModel>(searchRequest);

            return searchResults.Documents.ToList();
        }

        public async Task<List<ServiceElasticSearchModel>> SearchServicesByCommunityGroupIdAsync(int communityGroupId)
        {
            ElasticClient client = GetClient(services);

            BoolQuery communityGroupQuery = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(c => c.id)),
                        Value = communityGroupId
                    }
                }
            };

            ISearchRequest<ServiceElasticSearchModel> searchRequest = new SearchRequest<ServiceElasticSearchModel>()
            {
                Query = communityGroupQuery,
                From = 0,
                Size = 10000
            };

            ISearchResponse<ServiceElasticSearchModel> searchResults = await client.SearchAsync<ServiceElasticSearchModel>(searchRequest);

            return searchResults.Documents.ToList();
        }

        public async Task<List<ServiceElasticSearchModel>> SearchServicesByAccessibilityFeatureIdAsync(int accessibilityFeatureId)
        {
            ElasticClient client = GetClient(services);

            BoolQuery accessibilityFeatureQuery = new BoolQuery
            {
                Must = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.accessibility_features.Select(c => c.id)),
                        Value = accessibilityFeatureId
                    }
                }
            };

            ISearchRequest<ServiceElasticSearchModel> searchRequest = new SearchRequest<ServiceElasticSearchModel>()
            {
                Query = accessibilityFeatureQuery,
                From = 0,
                Size = 10000
            };

            ISearchResponse<ServiceElasticSearchModel> searchResults = await client.SearchAsync<ServiceElasticSearchModel>(searchRequest);

            return searchResults.Documents.ToList();
        }

        public async Task<ISearchResponse<ServiceElasticSearchModel>> SearchServicesAsync(Postcode postcode, string searchTerm, string categories = "", string locationType = null, int radius = 5000, int page = 1, int pageSize = 10, string sort = "", string community_groups = "", string accessibility_features = "")
        {
            ElasticClient client = GetClient(services);

            ISearchRequest<ServiceElasticSearchModel> searchRequest = new SearchRequest<ServiceElasticSearchModel>()
            {
                Query = GetSearchQuery(searchTerm, categories, community_groups, accessibility_features, locationType, postcode, radius),
                From = pageSize * (page - 1),
                Size = pageSize,
                Sort = new List<ISort>
                {
                    SortByDeprioritised()
                },
                Aggregations = new AggregationDictionary
                {
                    {
                        "cat_agg",
                        new AggregationContainer
                        {
                            Terms = new TermsAggregation("cat_agg")
                            {
                                Field = new Field("categories.id"),
                                Size = 2000
                            }
                        }
                    }
                },
            };

            switch (sort)
            {
                case "sort-a-z":
                    searchRequest.Sort.Add(SortByAlphabeticalOrder());
                    break;
                case "sort-recently-added":
                    searchRequest.Sort.Add(SortByDateAdded());
                    break;
                case "sort-last-reviewed":
                    searchRequest.Sort.Add(SortByLastReviewed());
                    break;
                case "sort-distance":
                    searchRequest.Sort.Add(SortByPostcode(postcode));
                    break;
                case "sort-relevance":
                    searchRequest.Sort.Add(SortByScore()); 
                    break;
                default:
                    searchRequest.Sort.Add(!String.IsNullOrEmpty(postcode?.postcode) 
                        ? SortByLocationScore() 
                        : !String.IsNullOrEmpty(searchTerm) 
                            ? SortByScore() 
                            : SortByLastReviewed());
                    break;
            }

            if (!String.IsNullOrEmpty(postcode?.postcode) && !string.IsNullOrEmpty(sort) && !sort.Equals("sort-distance"))
            {
                searchRequest.Sort.Add(SortByPostcode(postcode));
            }

            if (!String.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(sort) && !sort.Equals("sort-relevance"))
            {
                searchRequest.Sort.Add(SortByScore());
            }

            if (!string.IsNullOrEmpty(sort) && !sort.Equals("sort-a-z"))
            {
                searchRequest.Sort.Add(SortByAlphabeticalOrder());
            }

            if (!string.IsNullOrEmpty(sort) && sort.Equals("sort-relevance"))
            {
                searchRequest.Sort.Add(SortByLastReviewed());
            }

            ISearchResponse<ServiceElasticSearchModel> searchResults = await client.SearchAsync<ServiceElasticSearchModel>(searchRequest);

            return searchResults;
        }

        public QueryContainer FilterByPostcode(Postcode postcode, int radius)
        {
            ExistsQuery hasSerivceAreaQuery = new ExistsQuery()
            {
                Field = "service_areas"
            };

            TermQuery matchesServiceAreaQuery = new TermQuery()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.code)),
                Value = postcode.codes.admin_district
            };

            TermQuery matchesWardQuery = new TermQuery()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.code)),
                Value = postcode.codes.admin_ward
            };

            TermQuery matchesHealthBoardQuery = new TermQuery()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.code)),
                Value = GetHealthBoardForLocalAuthority(postcode.codes.admin_district)
            };

            TermQuery hasNationalCoverageQuery = new TermQuery()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.type)),
                Value = "Country"
            };

            GeoDistanceQuery geoDistanceQuery = new GeoDistanceQuery()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.locations.Select(s => s.point)),
                Distance = String.Format("{0}", radius),
                Location = new GeoLocation(postcode.latitude, postcode.longitude),
            };

            FunctionScoreQuery scoreQuery = new FunctionScoreQuery()
            {
                Functions = new List<IScoreFunction>
                {
                    new GaussGeoDecayFunction()
                    {
                        Origin = new GeoLocation(postcode.latitude, postcode.longitude),
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.locations.Select(s => s.point)),
                        Scale = String.Format("{0}m", (radius/3)),
                        Decay = 0.3
                    }
                }
            };

            return scoreQuery & ((matchesServiceAreaQuery | matchesWardQuery | matchesHealthBoardQuery | hasNationalCoverageQuery) | ((!hasSerivceAreaQuery | (matchesServiceAreaQuery | matchesHealthBoardQuery | hasNationalCoverageQuery | geoDistanceQuery)) & geoDistanceQuery));
        }

        public BoolQuery FilterServicesBySearchTerm(string searchTerm)
        {
            double fuzzyName = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Name"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Name"]) : 0;
            double exactName = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Name"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Name"]) : 0;
            double fuzzyDescription = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Description"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Description"]) : 0;
            double exactDescription = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Description"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Description"]) : 0;
            double fuzzySummary = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Summary"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Summary"]) : 0;
            double exactSummary = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Summary"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Summary"]) : 0;
            double fuzzyCategories = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Categories"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:Categories"]) : 0;
            double exactCategories = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Categories"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:Categories"]) : 0;
            double fuzzyAccessibility = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:AccessibilityFeatures"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:AccessibilityFeatures"]) : 0;
            double exactAccessibility = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:AccessibilityFeatures"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:AccessibilityFeatures"]) : 0;
            double fuzzyCommunity = ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:CommunityGroups"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Fuzzy:CommunityGroups"]) : 0;
            double exactCommunity = ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:CommunityGroups"].IsDecimal() ? double.Parse(ConfigurationManager.AppSettings["ElasticSearch:Boost:Exact:CommunityGroups"]) : 0;

            BoolQuery boolQuery = new BoolQuery()
            {
                Must = new QueryContainer[]
                {
                    new MultiMatchQuery()
                    {
                        Query = searchTerm.ToLower(),
                        Operator = Operator.And,
                        Type = TextQueryType.MostFields,
                        Fields = new Field[]
                        {
                            new Field("name", fuzzyName),
                            new Field("description", fuzzyDescription),
                            new Field("summary", fuzzySummary),
                            Infer.Field<ServiceElasticSearchModel>(f => f.categories.Select(n => n.name.ToLower()), fuzzyCategories),
                            Infer.Field<ServiceElasticSearchModel>(f => f.accessibility_features.Select(s => s.name.ToLower()), fuzzyAccessibility),
                            Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(n => n.name.ToLower()), fuzzyCommunity),
                        },
                        Fuzziness = Fuzziness.AutoLength(4, 7)
                    },
                },
                Should = new QueryContainer[]
                {
                    new MultiMatchQuery()
                    {
                        Query = searchTerm.ToLower(),
                        Operator = Operator.Or,
                        Type = TextQueryType.MostFields,
                        Fields = new Field[]
                        {
                            new Field("name", exactName),
                            new Field("description", exactDescription),
                            new Field("summary", exactSummary),
                            Infer.Field<ServiceElasticSearchModel>(f => f.categories.Select(n => n.name.ToLower()), exactCategories),
                            Infer.Field<ServiceElasticSearchModel>(f => f.accessibility_features.Select(s => s.name.ToLower()), exactAccessibility),
                            Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(n => n.name.ToLower()), exactCommunity),
                        }
                    }
                }
            };
            return boolQuery;
        }

        public FunctionScoreQuery FilterByCategory(string category)
        {
            FunctionScoreQuery scoreQuery = new FunctionScoreQuery()
            {
                Query = new TermQuery()
                {
                    Field = Infer.Field<ServiceElasticSearchModel>(f => f.categories.Select(i => i.slug)),
                    Value = category
                },
                Functions = new List<IScoreFunction>()
                {
                    new WeightFunction()
                    {
                        Weight = 2,
                        Filter = new TermQuery()
                        {
                            Field = Infer.Field<ServiceElasticSearchModel>(f => f.categories.Select(i => i.slug)),
                            Value = category
                        }
                    }
                },
                ScoreMode = FunctionScoreMode.Sum
            };

            return scoreQuery;
        }

        public FunctionScoreQuery FilterByCommunityGroup(string communityGroup)
        {
            FunctionScoreQuery scoreQuery = new FunctionScoreQuery()
            {
                Query = new TermQuery()
                {
                    Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.slug)),
                    Value = communityGroup
                },
                Functions = new List<IScoreFunction>()
                {
                    new WeightFunction()
                    {
                        Weight = 2,
                        Filter = new TermQuery()
                        {
                            Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.slug)),
                            Value = communityGroup
                        }
                    }
                },
                ScoreMode = FunctionScoreMode.Sum
            };
            
            return scoreQuery;
        }

        public QueryBase FilterByCommunityGroupRange(int value)
        {
            FunctionScoreQuery maxQuery = new FunctionScoreQuery()
            {
                Query = new NumericRangeQuery()
                {
                    Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.max_value)),
                    GreaterThanOrEqualTo = value
                },
                Functions = new List<IScoreFunction>()
                {
                    new WeightFunction()
                    {
                        Weight = 2,
                        Filter = new NumericRangeQuery()
                        {
                            Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.max_value)),
                            GreaterThanOrEqualTo = value
                        }
                    }
                }
            };

            FunctionScoreQuery minQuery = new FunctionScoreQuery()
            {
                Query = new NumericRangeQuery()
                {
                    Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.min_value)),
                    LessThanOrEqualTo = value
                },
                Functions = new List<IScoreFunction>()
                {
                    new WeightFunction()
                    {
                        Weight = 2,
                        Filter = new NumericRangeQuery()
                        {
                            Field = Infer.Field<ServiceElasticSearchModel>(f => f.community_groups.Select(i => i.min_value)),
                            LessThanOrEqualTo = value
                        }
                    }
                }
            };

            return maxQuery & minQuery;
        }

        public FunctionScoreQuery FilterByAccessibilityFeature(string accessibilityFeature)
        {
            FunctionScoreQuery scoreQuery = new FunctionScoreQuery()
            {
                Query = new TermQuery()
                {
                    Field = Infer.Field<ServiceElasticSearchModel>(f => f.accessibility_features.Select(i => i.slug)),
                    Value = accessibilityFeature
                },
                Functions = new List<IScoreFunction>()
                {
                    new WeightFunction()
                    {
                        Weight = 2,
                        Filter = new TermQuery()
                        {
                            Field = Infer.Field<ServiceElasticSearchModel>(f => f.accessibility_features.Select(i => i.slug)),
                            Value = accessibilityFeature
                        }
                    }
                },
                ScoreMode = FunctionScoreMode.Sum
            };

            return scoreQuery;
        }

        public BoolQuery FilterByLocationType(string type)
        {
            BoolQuery query = new BoolQuery();
            if (type == "local")
            {
                query.MustNot = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.type)),
                        Value = "Country"
                    }
                };
            }
            else
            {
                query.Must = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = Infer.Field<ServiceElasticSearchModel>(f => f.service_areas.Select(c => c.type)),
                        Value = "Country"
                    }
                };
            }

            return query;
        }

        public GeoDistanceSort SortByPostcode(Postcode postcode)
        {
            GeoDistanceSort geoSort = new GeoDistanceSort()
            {
                Field = Infer.Field<ServiceElasticSearchModel>(f => f.locations.Select(s => s.point)),
                Order = SortOrder.Ascending,
                Unit = DistanceUnit.Meters,
                Points = new List<GeoLocation>()
                {
                    new GeoLocation(postcode.latitude, postcode.longitude)
                }
            };

            return geoSort;
        }

        public FieldSort SortByDeprioritised()
        {
            return new FieldSort()
            {
                Field = "is_deprioritised",
                Order = SortOrder.Ascending
            };
        }

        public FieldSort SortByLocationScore()
        {
            FieldSort locationScoreSort = new FieldSort()
            {
                Field = "location_score",
                Order = SortOrder.Ascending
            };

            return locationScoreSort;
        }

        public FieldSort SortByScore()
        {
            FieldSort scoreSort = new FieldSort()
            {
                Field = "_score",
                Order = SortOrder.Descending
            };

            return scoreSort;
        }

        public FieldSort SortByLastReviewed() 
        {
            return new FieldSort()
            {
                Field = "last_reviewed",
                Order = SortOrder.Descending
            };
        }

        public FieldSort SortByDateAdded()
        {
            return new FieldSort()
            {
                Field = "created_on",
                Order = SortOrder.Descending
            };
        }

        public FieldSort SortByAlphabeticalOrder()
        {
            return new FieldSort()
            {
                Field = "slug",
                Order = SortOrder.Ascending
            };
        }

        public void DeleteService(Guid serviceId)
        {
            ElasticClient client = GetClient(services);
            client.Delete<ServiceElasticSearchModel>(serviceId);
        }

        public void DeleteAllServices()
        {
            DeleteIndex(services);
        }

        public string GetHealthBoardForLocalAuthority(string localAuthority)
        {
            if (localAuthority == "S12000008" || localAuthority == "S12000021" || localAuthority == "S12000028")
            {
                return "S08000015";
            }
            else if (localAuthority == "S12000026")
            {
                return "S08000016";
            }
            else if (localAuthority == "S12000006")
            {
                return "S08000017";
            }
            else if (localAuthority == "S12000047")
            {
                return "S08000029";
            }
            else if (localAuthority == "S12000005" || localAuthority == "S12000030" || localAuthority == "S12000014")
            {
                return "S08000019";
            }
            else if (localAuthority == "S12000034" || localAuthority == "S12000033" || localAuthority == "S12000020")
            {
                return "S08000020";
            }
            else if (localAuthority == "S12000009" || localAuthority == "S12000045" || localAuthority == "S12000011" || localAuthority == "S12000046" || localAuthority == "S12000049" || localAuthority == "S12000018" || localAuthority == "S12000038" || localAuthority == "S12000039")
            {
                return "S08000031";
            }
            else if (localAuthority == "S12000017" || localAuthority == "S12000035")
            {
                return "S08000022";
            }
            else if (localAuthority == "S12000044" || localAuthority == "S12000050" || localAuthority == "S12000029")
            {
                return "S08000032";
            }
            else if (localAuthority == "S12000036" || localAuthority == "S12000010" || localAuthority == "S12000019" || localAuthority == "S12000040")
            {
                return "S08000024";
            }
            else if (localAuthority == "S12000023")
            {
                return "S08000025";
            }
            else if (localAuthority == "S12000027")
            {
                return "S08000026";
            }
            else if (localAuthority == "S12000048" || localAuthority == "S12000042" || localAuthority == "S12000041")
            {
                return "S08000030";
            }
            else if (localAuthority == "S12000013")
            {
                return "S08000028";
            }
            else
            {
                return "";
            }
        }
    }
}