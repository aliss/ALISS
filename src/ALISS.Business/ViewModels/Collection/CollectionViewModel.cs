using ALISS.Business.PresentationTransferObjects.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Collection
{
    public class CollectionViewModel
    {
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CollectionServicePTO> Services { get; set; }
        public int TotalCount { get; set; }
    }
}
