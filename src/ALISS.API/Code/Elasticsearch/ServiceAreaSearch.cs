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
    public class ServiceAreaSearch : ElasticSearchBase
    {
        public ServiceAreaSearch() : base()
        {
            
        }

        public async Task AddServiceAreas(List<ServiceAreaElasticSearchModel> serviceAreasToAdd)
        {
            var client = GetClient(serviceAreas);
            foreach (ServiceAreaElasticSearchModel serviceArea in serviceAreasToAdd)
            {
                await client.IndexAsync(serviceArea, default);
            }
        }

        public async Task<IEnumerable<ServiceAreaElasticSearchModel>> GetServiceAreas()
        {
            ElasticClient client = GetClient(serviceAreas);

            ISearchResponse<ServiceAreaElasticSearchModel> response = await client.SearchAsync<ServiceAreaElasticSearchModel>(s =>
                s.MatchAll()
                .Size(2000)
            );

            return response?.Documents ?? Enumerable.Empty<ServiceAreaElasticSearchModel>();
        }

        public async Task<IEnumerable<ServiceAreaElasticSearchModel>> GetServiceAreas(IEnumerable<int> serviceAreaIds)
        {
            ElasticClient client = GetClient(serviceAreas);

            ISearchResponse<ServiceAreaElasticSearchModel> response = await client.SearchAsync<ServiceAreaElasticSearchModel>(s => s
                .Query(q => q
                    .Terms(t => t
                        .Field(f => f.id)
                        .Terms(serviceAreaIds)
                    )
                )
                .Size(2000)
            );

            return response?.Documents ?? Enumerable.Empty<ServiceAreaElasticSearchModel>();
        }

        public async Task<IEnumerable<ServiceAreaElasticSearchModel>> GetServiceAreas(IEnumerable<string> serviceAreaCodes)
        {
            ElasticClient client = GetClient(serviceAreas);

            ISearchResponse<ServiceAreaElasticSearchModel> response = await client.SearchAsync<ServiceAreaElasticSearchModel>(s => s
                .Query(q => q
                    .Terms(t => t
                        .Field(f => f.code)
                        .Terms(serviceAreaCodes)
                    )
                )
                .Size(2000)
            );

            return response?.Documents ?? Enumerable.Empty<ServiceAreaElasticSearchModel>();
        }

        public void DeleteAllServiceAreas()
        {
            DeleteIndex(serviceAreas);
        }

    }
}