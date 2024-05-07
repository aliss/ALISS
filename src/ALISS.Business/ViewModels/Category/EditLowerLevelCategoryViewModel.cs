using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ALISS.Business.ViewModels.Category
{
    public class EditLowerLevelCategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Category name must be no longer than 50 characters.")]
        [Display(Description = "The name of the category you wish to add")]
        public string Name { get; set; }
        [Display(Description = "The URL friendly slug for this category - this is auto generated")]
        public string Slug { get; set; }
        [Required(ErrorMessage = "You must select a level one or level 2 category for this category to sit under.")]
        [Display(Name = "Parent Category", Description = "Please select a parent category from the following list")]
        public int ParentCategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
