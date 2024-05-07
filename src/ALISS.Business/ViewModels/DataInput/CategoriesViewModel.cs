using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Category;

namespace ALISS.Business.ViewModels.DataInput
{
    public class CategoriesViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public List<ServiceCategoryPTO> ServiceCategories { get; set; }
        [Required(ErrorMessage = "You must select at least one category for this service")]
        public string SelectedCategories { get; set; }
        public DataInputSummaryTypeEnum SummaryType { get; set; }
    }
}
