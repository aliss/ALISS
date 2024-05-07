using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Category
{
    public class ServiceCategoryPTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Selected { get; set; }
        public List<ServiceCategoryPTO> NextLevelCategories { get; set; }
    }
}
