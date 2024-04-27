using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.CMS.Models.Collection
{
    public class CollectionListingViewModel
    {
        public IEnumerable<CollectionListPTO> Collections { get; set; }
        public int UserProfileId { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
    }
}