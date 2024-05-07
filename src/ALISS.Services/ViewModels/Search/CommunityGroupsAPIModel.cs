using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.API.Models.Elasticsearch;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class CommunityGroupsAPIModel
    {
        public IEnumerable<CommunityGroupElasticSearchModel> data { get; set; }
    }
}
