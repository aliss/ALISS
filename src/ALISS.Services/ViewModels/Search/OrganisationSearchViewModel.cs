using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.ApiServices.ViewModels.Search;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class OrganisationSearchViewModel
    {
        public OrganisationSearchViewModel(OrganisationSearchInputModel input)
        {
            SearchTerm = input.SearchTerm;
            Page = input.Page ?? 1;
            PageSize = input.PageSize ?? 20;
        }
        public OrganisationSearchAPIModel SearchModel { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
