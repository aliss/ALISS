using ALISS.Business.PresentationTransferObjects.Category;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Category
{
    public class CategoryListingViewModel
    {
        public List<CategoryPTO> Categories { get; set; }
        public string SearchTerm { get; set; }
        public int TotalResults { get; set; }
    }
}
