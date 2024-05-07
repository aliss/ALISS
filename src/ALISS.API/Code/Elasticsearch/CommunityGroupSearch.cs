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
    public class CommunityGroupSearch : ElasticSearchBase
    {
        public CommunityGroupSearch() : base()
        {
            
        }

        public async Task AddCommunityGroups(List<CommunityGroupElasticSearchModel> communityGroupsToAdd)
        {
            var client = GetClient(communityGroups);

            foreach (CommunityGroupElasticSearchModel communityGroup in communityGroupsToAdd)
            {
                if (!client.DocumentExists<CommunityGroupElasticSearchModel>(communityGroup).Exists)
                {
                    await client.IndexDocumentAsync(communityGroup);
                }
                else
                {
                    await UpdateCommunityGroup(communityGroup);
                }
            }
        }

        public async Task UpdateCommunityGroup(CommunityGroupElasticSearchModel communityGroupToUpdate)
        {
            ElasticClient client = GetClient(communityGroups);

            _ = await client.IndexAsync(communityGroupToUpdate, default);
        }

        public async Task<IEnumerable<CommunityGroupElasticSearchModel>> GetCommunityGroups(bool flat = false)
        {
            ElasticClient client = GetClient(communityGroups);

            ISearchResponse<CommunityGroupElasticSearchModel> response = await client.SearchAsync<CommunityGroupElasticSearchModel>(s =>
                s.MatchAll()
                .Size(2000)
            );

            IEnumerable<CommunityGroupElasticSearchModel> communityGroupsList = response?.Documents ?? Enumerable.Empty<CommunityGroupElasticSearchModel>();

            if (flat)
            {
                List<CommunityGroupElasticSearchModel> flatCommunityGroups = new List<CommunityGroupElasticSearchModel>();

                foreach (CommunityGroupElasticSearchModel communityGroup in communityGroupsList)
                {
                    if (communityGroup.sub_communitygroups != null && communityGroup.sub_communitygroups.Any())
                    {
                        flatCommunityGroups.AddRange(ExtractSubCommunityGroups(communityGroup));
                    }

                    flatCommunityGroups.Add(new CommunityGroupElasticSearchModel { id = communityGroup.id, name = communityGroup.name, slug = communityGroup.slug });
                }

                return flatCommunityGroups.OrderBy(c => c.name);
            }
            else
            {
                return communityGroupsList;
            }
        }

        public void DeleteAllCommunityGroups()
        {
            DeleteIndex(communityGroups);
        }

        public void DeleteCommunityGroup(int communityGroupToDelete)
        {
            ElasticClient client = GetClient(communityGroups);
            client.Delete<CommunityGroupElasticSearchModel>(communityGroupToDelete);
        }

        private IEnumerable<CommunityGroupElasticSearchModel> ExtractSubCommunityGroups(CommunityGroupElasticSearchModel communityGroup)
        {
            List<CommunityGroupElasticSearchModel> communityGroups = new List<CommunityGroupElasticSearchModel>();

            foreach (CommunityGroupElasticSearchModel subCommunityGroup in communityGroup.sub_communitygroups)
            {
                if (subCommunityGroup.sub_communitygroups != null && subCommunityGroup.sub_communitygroups.Any())
                {
                    communityGroups.AddRange(ExtractSubCommunityGroups(subCommunityGroup));
                }

                communityGroups.Add(new CommunityGroupElasticSearchModel { id = subCommunityGroup.id, name = subCommunityGroup.name, slug = subCommunityGroup.slug });
            }

            return communityGroups;
        }
    }
}