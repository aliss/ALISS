using ALISS.API.Models.Elasticsearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ALISS.API.Code.Elasticsearch
{
    public class ElasticSearchBase
    {
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        internal const string services = "services";
        internal const string organisations = "organisations";
        internal const string categories = "categories";
        internal const string serviceAreas = "serviceareas";
        internal const string communityGroups = "communitygroups";
        internal const string accessibilityFeatures = "accessibilityfeatures";

        public ElasticSearchBase()
        {
            _baseUrl = ConfigurationManager.AppSettings["ElasticSearch:Host"].ToString();
            _username = ConfigurationManager.AppSettings["ElasticSearch:Username"].ToString();
            _password = ConfigurationManager.AppSettings["ElasticSearch:Password"].ToString();
        }

        public ElasticClient GetClient()
        {
            Uri node = new Uri(_baseUrl);
            ConnectionSettings settings = new ConnectionSettings(node);
            if (_baseUrl != "http://localhost:9200")
            {
                settings.BasicAuthentication(_username, _password);
            }
            settings.ThrowExceptions(alwaysThrow: true);
            settings.PrettyJson();
            settings.DisableDirectStreaming(true);
            return new ElasticClient(settings);
        }

        public ElasticClient GetClient(string index)
        {
            string indexToCheck = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + index;
            ElasticClient client = GetClient();
            if (!client.Indices.Exists(indexToCheck).Exists)
            {
                switch (index)
                {
                    case categories:
                        CreateCategoryIndex(client);
                        break;
                    case organisations:
                        CreateOrganisationIndex(client);
                        break;
                    case serviceAreas:
                        CreateServiceAreaIndex(client);
                        break;
                    case communityGroups:
                        CreateCommunityGroupIndex(client);
                        break;
                    case accessibilityFeatures:
                        CreateAccessibilityFeaturesIndex(client);
                        break;
                    default:
                        CreateServiceIndex(client);
                        break;
                }
            }
            Uri node = new Uri(_baseUrl);
            ConnectionSettings settings = new ConnectionSettings(node);
            if (_baseUrl != "http://localhost:9200")
            {
                settings.BasicAuthentication(_username, _password);
            }
            settings.ThrowExceptions(alwaysThrow: true);
            settings.PrettyJson();
            settings.DisableDirectStreaming(true);
            settings.DefaultIndex(indexToCheck);
            client = new ElasticClient(settings);

            return client;
        }

        public void CreateServiceIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + services;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            .Stop("my_stop", stf => stf
                                .StopWords(new string[] { "_english_" }))
                            .Shingle("custom_shingle", sh => sh
                                .MinShingleSize(2)
                                .MaxShingleSize(3)
                                .OutputUnigrams(true))
                            )
                        .Analyzers(aa => aa
                            .Custom("description_analyzer", da => da
                                .Tokenizer("standard")
                                .CharFilters(new string[] { "html_strip" })
                                .Filters(new string[] { "lowercase", "my_stop" }))
                            .Custom("bigram_combiner", bc => bc
                                .Tokenizer("standard")
                                .Filters(new string[] { "lowercase", "custom_shingle", "my_stop" }))
                            )
                        )
                    )
                .Map<ServiceElasticSearchModel>(m => m
                    .AutoMap<ServiceElasticSearchModel>()
                )
            );
        }

        public void CreateOrganisationIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + organisations;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            .Stop("my_stop", stf => stf
                                .StopWords(new string[] { "_english_" }))
                            .Shingle("custom_shingle", sh => sh
                                .MinShingleSize(2)
                                .MaxShingleSize(3)
                                .OutputUnigrams(true))
                            )
                        .Analyzers(aa => aa
                            .Custom("description_analyzer", da => da
                                .Tokenizer("standard")
                                .CharFilters(new string[] { "html_strip" })
                                .Filters(new string[] { "lowercase", "my_stop" }))
                            .Custom("bigram_combiner", bc => bc
                                .Tokenizer("standard")
                                .Filters(new string[] { "lowercase", "custom_shingle", "my_stop" }))
                            )
                        )
                    )
                .Map<OrganisationElasticSearchModel>(m => m
                    .AutoMap<OrganisationElasticSearchModel>()
                )
            );
        }

        public void CreateCategoryIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + categories;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Map<CategoryElasticSearchModel>(m => m
                    .AutoMap<CategoryElasticSearchModel>()
                )
            );
        }

        public void CreateCommunityGroupIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + communityGroups;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Map<CommunityGroupElasticSearchModel>(m => m
                    .AutoMap<CommunityGroupElasticSearchModel>()
                )
            );
        }

        public void CreateAccessibilityFeaturesIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + accessibilityFeatures;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Map<AccessibilityFeatureElasticSearchModel>(m => m
                    .AutoMap<AccessibilityFeatureElasticSearchModel>()
                )
            );
        }

        public void CreateServiceAreaIndex(ElasticClient client)
        {
            string indexToCreate = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + serviceAreas;
            var createIndexResponse = client.Indices.Create(indexToCreate, i => i
                .Map<ServiceAreaElasticSearchModel>(m => m
                    .AutoMap<ServiceAreaElasticSearchModel>()
                )
            );
        }

        public void DeleteIndex(string index)
        {
            string indexToDelete = ConfigurationManager.AppSettings["ElasticSearch:IndexPrefix"].ToString().ToLower() + index;
            ElasticClient client = GetClient();

            var deleteIndexResponse = client.Indices.Delete(indexToDelete);
        }
    }
}