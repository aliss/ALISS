using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.API.Models.Elasticsearch;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class CategoriesAPIModel
    {
        public IEnumerable<CategoryElasticSearchModel> data { get; set; }
    }
}
