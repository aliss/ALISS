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
    public class AccessibilityFeatureSearch : ElasticSearchBase
    {
        public AccessibilityFeatureSearch() : base()
        {
            
        }

        public async Task AddAccessibilityFeatures(List<AccessibilityFeatureElasticSearchModel> accessibilityFeaturesToAdd)
        {
            var client = GetClient(accessibilityFeatures);

            foreach (AccessibilityFeatureElasticSearchModel accessibilityFeature in accessibilityFeaturesToAdd)
            {
                if (!client.DocumentExists<AccessibilityFeatureElasticSearchModel>(accessibilityFeature).Exists)
                {
                    await client.IndexDocumentAsync(accessibilityFeature);
                }
                else
                {
                    await UpdateAccessibilityFeature(accessibilityFeature);
                }
            }
        }

        public async Task UpdateAccessibilityFeature(AccessibilityFeatureElasticSearchModel accessibilityFeatureToUpdate)
        {
            ElasticClient client = GetClient(accessibilityFeatures);

            _ = await client.IndexAsync(accessibilityFeatureToUpdate, default);
        }

        public async Task<IEnumerable<AccessibilityFeatureElasticSearchModel>> GetAccessibilityFeatures()
        {
            ElasticClient client = GetClient(accessibilityFeatures);

            ISearchResponse<AccessibilityFeatureElasticSearchModel> response = await client.SearchAsync<AccessibilityFeatureElasticSearchModel>(s =>
                s.MatchAll()
                .Size(2000)
            );

            return response?.Documents ?? Enumerable.Empty<AccessibilityFeatureElasticSearchModel>();
        }

        public void DeleteAllAccessibilityFeatures()
        {
            DeleteIndex(accessibilityFeatures);
        }

        public void DeleteAccessibilityFeature(int accessibilityFeatureToDelete)
        {
            ElasticClient client = GetClient(accessibilityFeatures);
            client.Delete<AccessibilityFeatureElasticSearchModel>(accessibilityFeatureToDelete);
        }
    }
}