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
    public class CategorySearch : ElasticSearchBase
    {
        public CategorySearch() : base()
        {
            
        }

        public async Task AddCategories(List<CategoryElasticSearchModel> categoriesToAdd)
        {
            var client = GetClient(categories);
            foreach (CategoryElasticSearchModel category in categoriesToAdd)
            {
                if (!client.DocumentExists<CategoryElasticSearchModel>(category).Exists)
                {
                    await client.IndexDocumentAsync(category);
                }
                else
                {
                    await UpdateCategory(category);
                }
            }
        }

        public async Task UpdateCategory(CategoryElasticSearchModel categoryToUpdate)
        {
            ElasticClient client = GetClient(categories);

            _ = await client.IndexAsync(categoryToUpdate, default);
        }

        public void DeleteCategory(int categoryToDelete)
        {
            ElasticClient client = GetClient(categories);
            client.Delete<ServiceElasticSearchModel>(categoryToDelete);
        }

        public async Task<IEnumerable<CategoryElasticSearchModel>> GetCategories(bool flat = false)
        {
            ElasticClient client = GetClient(categories);

            ISearchResponse<CategoryElasticSearchModel> response = await client.SearchAsync<CategoryElasticSearchModel>(s =>
                s.MatchAll()
                .Size(2000)
            );

            IEnumerable<CategoryElasticSearchModel> categoriesList = response?.Documents ?? Enumerable.Empty<CategoryElasticSearchModel>();

            if (flat)
            {
                List<CategoryElasticSearchModel> flatCategories = new List<CategoryElasticSearchModel>();

                foreach (CategoryElasticSearchModel category in categoriesList)
                {
                    if (category.sub_categories != null && category.sub_categories.Any())
                    {
                        flatCategories.AddRange(ExtractSubCategories(category));
                    }

                    flatCategories.Add(new CategoryElasticSearchModel { id = category.id, name = category.name, slug = category.slug });
                }

                return flatCategories.OrderBy(c => c.name);
            }
            else
            {
                return categoriesList;
            }
        }

        public void DeleteAllCategories()
        {
            DeleteIndex(categories);
        }

        private IEnumerable<CategoryElasticSearchModel> ExtractSubCategories(CategoryElasticSearchModel category)
        {
            List<CategoryElasticSearchModel> categories = new List<CategoryElasticSearchModel>();

            foreach (CategoryElasticSearchModel subCategory in category.sub_categories)
            {
                if (subCategory.sub_categories != null && subCategory.sub_categories.Any())
                {
                    categories.AddRange(ExtractSubCategories(subCategory));
                }

                categories.Add(new CategoryElasticSearchModel { id = subCategory.id, name = subCategory.name, slug = subCategory.slug });
            }

            return categories;
        }
    }
}