using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.ViewModels.CommunityGroup
{
    public class EditTopLevelCommunityGroupViewModel
    {
        public int CommunityGroupId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Community Group name must be no longer than 50 characters.")]
        [Display(Description = "The name of the Community Group you wish to add")]
        public string Name { get; set; }

        [Display(Description = "The URL friendly slug for this Community Group - this is auto generated")]
        public string Slug { get; set; }

        [Display(Name = "Community Group Icon", Description = "The icon you wish to display when adding Community Groups to a service")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "You must provide a number greater than 0 for the display order")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter a number greater than 0")]
        [DefaultValue(0)]
        [Display(Name = "Display Order", Description = "The order this item is displayed in.  If the number is used multiple times, alphabetical order is used.")]
        public int DisplayOrder { get; set; }
        [Display(Name = "Use a Min / Max Range")]
        public bool IsMinMax { get; set; }
    }
}
