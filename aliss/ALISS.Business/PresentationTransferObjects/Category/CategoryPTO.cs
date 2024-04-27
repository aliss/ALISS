using System.Collections.Generic;

namespace ALISS.Business.PresentationTransferObjects.Category
{
    public class CategoryPTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int ServiceCount { get; set; }
        public List<CategoryPTO> NextLevelCategories { get; set; }
    }
}
