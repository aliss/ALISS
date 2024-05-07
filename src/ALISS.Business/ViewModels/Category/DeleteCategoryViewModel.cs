using ALISS.Business.PresentationTransferObjects.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Category
{
    public class DeleteCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool CanDelete { get; set; }
        public List<RelatedServicePTO> RelatedServices { get; set; }
    }
}
