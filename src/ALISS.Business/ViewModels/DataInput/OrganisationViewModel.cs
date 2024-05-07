using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.ViewModels.Service;
using Nest;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;
using ALISS.Business.ViewModels.Organisation;

namespace ALISS.Business.ViewModels.DataInput
{
    public class OrganisationViewModel
    {
        public Guid OrganisationId { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Organisation Name", Description = "Please add the name of the organisation that provides the service or resource.")]
        public string OrganisationName { get; set; }
        [Display(Name = "Do you want to claim this organisation?", Description = "By checking this box you claim editorial control of the organisation. This ensures only you can make changes without approval of the ALISS team.")]
        public bool OrganisationRepresentative { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "My name:")]
        public string OrganisationRepresentativeName { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "My role in the organisation:")]
        public string OrganisationRepresentativeRole { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "My phone number:")]
        [PhoneNumberValidator]
        public string OrganisationRepresentativePhone { get; set; }
        [ConditionallyRequiredBoolTrueValidator("OrganisationRepresentative")]
        [Display(Name = "I understand and accept the terms above")]
        public bool OrganisationAcceptDataStandards { get; set; }
        [Required]
        [Display(Name = "Organisation Description", Description = "Please add a description of the organisation that provides the service. This should include any details that someone might wish to know about your organisation, including an explanation of what to expect from it, availability times, and any requirements. (Excluding any personal information)")]
        public string OrganisationDescription { get; set; }
        [Display(Name = "Organisation Phone Number", Description = "Please enter a valid UK phone number.")]
        [PhoneNumberValidator]
        [RequiredContactDetails]
        public string PhoneNumber { get; set; }
        [MaxLength(254)]
        [Display(Name = "Organisation Email", Description = "The most appropriate email address for someone to access or find out more about this organisation.")]
        [ValidEmailAddress]
        [RequiredContactDetails]
        public string Email { get; set; }
        [MaxLength(200)]
        [Display(Name = "Organisation Web Address", Description = "The URL (web address) for the webpage of the organisation you would like to add. Copy and paste the link e.g. https://www.example.com/a-organisation")]
        [RequiredContactDetails]
        [UrlValidator]
        public string Url { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Name = "Organisation Facebook", Description = "A link to the official Facebook page for the organisation you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        [RequiredContactDetails]
        public string Facebook { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Name = "Organisation X/Twitter", Description = "A link to the official X/Twitter account of the organisation you would like to add e.g. https://twitter.com/rnibuk")]
        [RequiredContactDetails]
        public string Twitter { get; set; }
        [MaxLength(120)]
        [Display(Name = "Organisation Instagram", Description = "A link to the official Instagram page for the organisation you would like to add e.g. https://www.instagram.com/rnibuk")]
        [UrlValidator]
        [RequiredContactDetails]
        public string Instagram { get; set; }
        [MaxLength(200)]
        public string Slug { get; set; }

        public class RequiredContactDetailsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                OrganisationViewModel instance = validationContext.ObjectInstance as OrganisationViewModel;

                return string.IsNullOrEmpty(instance.PhoneNumber) &&
                    string.IsNullOrEmpty(instance.Email) &&
                    string.IsNullOrEmpty(instance.Url) &&
                    string.IsNullOrEmpty(instance.Twitter) &&
                    string.IsNullOrEmpty(instance.Facebook) &&
                    string.IsNullOrEmpty(instance.Instagram)
                    ? new ValidationResult("At least one contact field must be filled in before saving.")
                    : ValidationResult.Success;
            }
        }
    }
}
