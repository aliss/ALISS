using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.CMS.Models.Collection
{
    public class CollectionViewModel
    {
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CollectionServicePTO> Services { get; set; }
        public int TotalCount { get; set; }
    }
}