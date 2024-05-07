using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;

namespace ALISS.Business.ViewModels.Organisation
{
    public class EditOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Description = "The official name of the organisation you would like to add")]
        public string Name { get; set; }
        [Required]
        [Display(Description = "A clear and concise description of the organisation you would like to add. The description will be used for the search feature so keep in mind key words or phrases")]
        public string Description { get; set; }
        [Display(Name = "Phone Number", Description = "Please enter a valid UK phone number.")]
        [PhoneNumberValidator]
		[RequiredContactDetails]
        public string PhoneNumber { get; set; }
        [RequiredContactDetails]
        [MaxLength(254)]
        [Display(Description = "The most appropriate email address for someone to contact the organisation with")]
        [ValidEmailAddress]
        public string Email { get; set; }
        [MaxLength(200)]
        [Display(Name = "Web Address", Description = "The URL (website address) of the organisation\'s offical webpage e.g. https://www.organisation.com")]
        [RequiredContactDetails]
        [UrlValidator]
        public string Url { get; set; }
        [RequiredContactDetails]
        [MaxLength(200)]
        [UrlValidator]
        [Display(Description = "A link to the official Facebook page for the organisation you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        public string Facebook { get; set; }
        [RequiredContactDetails]
        [MaxLength(200)]
        [UrlValidator]
        [Display(Description = "A link to the official Twitter account of the organisation you would like to add e.g. https://twitter.com/rnibuk")]
        public string Twitter { get; set; }
        [MaxLength(120)]
        [Display(Description = "A link to the official Twitter account of the organisation you would like to add e.g. https://instagram.com/rnibuk")]
        [RequiredContactDetails]
        [UrlValidator]
        public string Instagram { get; set; }
        [MaxLength(120)]
        public string Slug { get; set; }
        [Display(Description = "An image file for the logo of the organisation you would like to add.")]
        public string Logo { get; set; }
        public bool Published { get; set; }
        public string ReturnUrl { get; set; }
   

        public class RequiredContactDetailsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var instance = validationContext.ObjectInstance as EditOrganisationViewModel;

                if (string.IsNullOrEmpty(instance.PhoneNumber) &&
                    string.IsNullOrEmpty(instance.Email) &&
                    string.IsNullOrEmpty(instance.Url) &&
                    string.IsNullOrEmpty(instance.Twitter) &&
                    string.IsNullOrEmpty(instance.Facebook) &&
                    string.IsNullOrEmpty(instance.Instagram))
                {
                    return new ValidationResult("At least one contact field must be filled in before saving.");
                }
                return ValidationResult.Success;
            }
        }
    }
}
