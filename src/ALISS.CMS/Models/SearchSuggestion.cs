using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.CMS.Models
{
    public class SearchSuggestion
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string ImageAltText { get; set; }
        public string Summary { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
    }
}