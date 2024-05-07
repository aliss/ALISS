using ALISS.API.Models.Elasticsearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ALISS.API.Code.Elasticsearch
{
    public class OrganisationSearch : ElasticSearchBase
    {

        public OrganisationSearch() : base()
        {

        }

        public void AddOrganisation(OrganisationElasticSearchModel organisation)
        {
            var client = GetClient(organisations);
            if (!client.DocumentExists<OrganisationElasticSearchModel>(organisation).Exists)
            {
                _ = client.IndexDocument(organisation);
            }
            else
            {
                _ = client.Index(organisation, default);
            }
        }

        public void AddOrganisations(IEnumerable<OrganisationElasticSearchModel> organisationsToAdd)
        {

            var client = GetClient(organisations);
            BulkResponse response = client.IndexMany(organisationsToAdd);
            if (response.Errors || response.ItemsWithErrors.Any())
            {
                var errors = response.ItemsWithErrors;
            }
        }

        public async Task AddOrganisationAsync(OrganisationElasticSearchModel organisation)
        {
            var client = GetClient(organisations);
            if (!client.DocumentExists<OrganisationElasticSearchModel>(organisation).Exists)
            {
                _ = await client.IndexDocumentAsync(organisation);
            }
            else
            {
                _ = await client.IndexAsync(organisation, default);
            }
        }

        public async Task AddOrganisationsAsync(IEnumerable<OrganisationElasticSearchModel> organisationsToAdd)
        {
            var client = GetClient(organisations);
            foreach (OrganisationElasticSearchModel organisation in organisationsToAdd)
            {
                if (!client.DocumentExists<OrganisationElasticSearchModel>(organisation).Exists)
                {
                    _ = await client.IndexDocumentAsync(organisation);
                }
                else
                {
                    _ = await client.IndexAsync(organisation, default);
                }
            }
        }

        public OrganisationElasticSearchModel GetById(Guid id)
        {
            ElasticClient client = GetClient(organisations);
            DocumentPath<OrganisationElasticSearchModel> organisation = DocumentPath<OrganisationElasticSearchModel>.Id(id);
            IGetResponse<OrganisationElasticSearchModel> response = client.Get(organisation);

            return response?.Source;
        }

        public async Task<OrganisationElasticSearchModel> GetByIdAsync(Guid id)
        {
            ElasticClient client = GetClient(organisations);
            DocumentPath<OrganisationElasticSearchModel> organisation = DocumentPath<OrganisationElasticSearchModel>.Id(id);
            IGetResponse<OrganisationElasticSearchModel> response = await client.GetAsync(organisation);

            return response?.Source;
        }

        public async Task<OrganisationElasticSearchModel> GetBySlugAsync(string slug)
        {
            ElasticClient client = GetClient(organisations);
            var searchRequest = await client.SearchAsync<OrganisationElasticSearchModel>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.slug)
                        .Query(slug)
                        )
                    )
                );

            return searchRequest?.Documents.FirstOrDefault();

        }

        public ISearchResponse<OrganisationElasticSearchModel> GetFilteredOrganisations(string searchTerm, bool published, bool isEditorAdmin, int page = 1)
        {
            ElasticClient client = GetClient(organisations);

            ISearchRequest<OrganisationElasticSearchModel> searchRequest = new SearchRequest<OrganisationElasticSearchModel>();
            searchRequest.Query = isEditorAdmin ? GetFilterBySearchTermQuery(searchTerm) : (GetFilterBySearchTermQuery(searchTerm) & GetFilterByPublishedQuery(published));
            searchRequest.From = 20 * (page - 1);
            searchRequest.Size = 20;
            ISearchResponse<OrganisationElasticSearchModel> searchResults = client.Search<OrganisationElasticSearchModel>(searchRequest);

            return searchResults;
        }

        public BoolQuery GetFilterBySearchTermQuery(string searchTerm)
        {
            BoolQuery boolQuery = new BoolQuery()
            {
                Must = new QueryContainer[] 
                { 
                    new MultiMatchQuery()
                    {
                        Query = searchTerm,
                        Type = TextQueryType.MostFields,
                        Fields = new Field[]
                        {
                            new Field("name", 2),
                            new Field("description")
                        },
                        Fuzziness = Fuzziness.AutoLength(4, 7)
                    } 
                },
                Should = new QueryContainer[] 
                { 
                    new MultiMatchQuery()
                    {
                        Query = searchTerm,
                        Type = TextQueryType.MostFields,
                        Operator = Operator.And,
                        Fields = new Field[]
                        {
                            new Field("name", 2),
                            new Field("description", 1.5)
                        }
                    } 
                }
            };

            return boolQuery;
        }

        public BoolQuery GetFilterByPublishedQuery(bool published)
        {
            string publishedString = published.ToString();

            BoolQuery boolQuery = new BoolQuery()
            {
                Must = new QueryContainer[]
                {
                    new TermQuery()
                    {
                        Field = "published",
                        Value = publishedString
                    }
                }
            };

            return boolQuery;
        }

        public void DeleteOrganisation(Guid organisationId)
        {
            ElasticClient client = GetClient(organisations);
            client.Delete<OrganisationElasticSearchModel>(organisationId);
        }

        public void DeleteAllOrganisations()
        {
            DeleteIndex(organisations);
        }

    }
}