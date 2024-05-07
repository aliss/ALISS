using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.ViewModels.Category
{
    public class EditTopLevelCategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Category name must be no longer than 50 characters.")]
        [Display(Description = "The name of the category you wish to add")]
        public string Name { get; set; }
        [Display(Description = "The URL friendly slug for this category - this is auto generated")]
        public string Slug { get; set; }
        [Display(Name = "Category Icon", Description = "The icon you wish to display when adding categories to a service")]
        public string Icon { get; set; }
    }
}
