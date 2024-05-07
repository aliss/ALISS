using ALISS.Business.PresentationTransferObjects.Improvement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Improvement
{
    public class ImprovementListingViewModel
    {
        public IEnumerable<ImprovementListingPTO> Improvements { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
