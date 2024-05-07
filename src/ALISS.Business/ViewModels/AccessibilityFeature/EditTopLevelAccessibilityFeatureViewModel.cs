using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.ViewModels.AccessibilityFeature
{
    public class EditTopLevelAccessibilityFeatureViewModel
    {
        public int AccessibilityFeatureId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Accessibility Feature name must be no longer than 50 characters.")]
        [Display(Description = "The name of the Accessibility Feature you wish to add")]
        public string Name { get; set; }

        [Display(Description = "The URL friendly slug for this Accessibility Feature - this is auto generated")]
        public string Slug { get; set; }

        [Display(Description = "An optional list of prompt questions/points to be considered.  Each line (question/point) will be shown as a bullet point")]
        public string PromptQuestions { get; set; }

        [Display(Name = "Accessibility Feature Icon", Description = "The icon you wish to display when adding Accessibility Features to a service")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "You must provide a number greater than 0 for the display order")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter a number greater than 0")]
        [DefaultValue(0)]
        [Display(Name = "Display Order", Description = "The order this item is displayed in.  If the number is used multiple times, alphabetical order is used.")]
        public int DisplayOrder { get; set; }
        [Required(ErrorMessage = "You must specify the type(s) of services this accessibility feature is available for.")]
        public string AvailableFor { get; set; }
    }
}
