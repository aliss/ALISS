using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.ApiServices.ViewModels.Search
{
    public class SearchViewModel
    {
        public SearchViewModel(SearchInputModel input)
        {
            Query = input.Query;    
            Postcode = input.Postcode;
            Categories = input.Categories;
            CommunityGroups = input.Community_Groups;
            AccessibilityFeatures = input.Accessibility_Features;
            Placename = input.Placename;
            LocationType = input.LocationType;
            Radius = input.Radius ?? 10000;
            CustomRadius = input.CustomRadius ?? 10000;
            Page = input.Page ?? 1;
            PageSize = input.PageSize ?? 10;
            Sort = input.Sort;
            View = input.View;
        }
        public SearchAPIModel SearchModel { get; set; }
        public string Postcode { get; set; }
        public string Categories { get; set; }
        public string CommunityGroups { get; set; }
        public string AccessibilityFeatures { get; set; }
        public string Placename { get; set; }
        public string Query { get; set; }
        public string LocationType { get; set; }
        public int Radius { get; set; }
        public int CustomRadius { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public string View { get; set; }
    }
}
