using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class OrganisationSearchInputModel
	{
        public string SearchTerm { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class SearchInputModel
    {
        public string Postcode { get; set; }
        public string Categories { get; set; }
        public string Community_Groups { get; set; }
        public string Accessibility_Features { get; set; }
        public string Placename { get; set; }
        public string Query { get; set; }
        public string LocationType { get; set; }
        public int? Radius { get; set; }
        public int? CustomRadius { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string Sort { get; set; }
        public string View { get; set; }
    }
}
