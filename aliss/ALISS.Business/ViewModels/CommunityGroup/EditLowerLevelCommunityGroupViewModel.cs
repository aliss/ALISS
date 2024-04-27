using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ALISS.Business.ViewModels.CommunityGroup
{
    public class EditLowerLevelCommunityGroupViewModel
	{
        public int CommunityGroupId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Who category name must be no longer than 50 characters.")]
        [Display(Description = "The name of the Who category you wish to add")]
        public string Name { get; set; }

        [Display(Description = "The URL friendly slug for this Who category - this is auto generated")]
        public string Slug { get; set; }

        [Display(Name = "Display Order", Description = "The order this item is displayed in.  If the number is used multiple times, alphabetical order is used.")]
        public int DisplayOrder { get; set; }

        [Required(ErrorMessage = "You must provide a number greater than 0 for the display order")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter a number greater than 0")]
        [DefaultValue(0)]
        [Display(Name = "Parent Who Category", Description = "Please select a parent Who category from the following list")]
        public int ParentCommunityGroupId { get; set; }

        public List<SelectListItem> CommunityGroups { get; set; }
    }
}
