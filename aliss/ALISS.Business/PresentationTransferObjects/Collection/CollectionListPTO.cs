using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Collection
{
    public class CollectionListPTO
    {
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public bool CanDelete { get; set; }
        public int ServiceCount { get; set; }
    }
}
