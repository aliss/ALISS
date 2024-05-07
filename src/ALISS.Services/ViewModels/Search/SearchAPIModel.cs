using ALISS.API.Models.Elasticsearch;
using ALISS.ApiServices.ViewModels.Organisation;
using ALISS.ApiServices.ViewModels.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class OrganisationSearchAPIModel
	{
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public IEnumerable<OrganisationModel> data { get; set; }
    }

    public class SearchAPIModel
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public string error { get; set; }
        public IEnumerable<ServiceModel> data { get; set; }
        public AggregationModel aggregations { get; set; }
    }
}
