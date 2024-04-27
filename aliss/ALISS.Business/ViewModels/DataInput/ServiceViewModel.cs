using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.ViewModels.Service;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;
using ALISS.Business.Enums;

namespace ALISS.Business.ViewModels.DataInput
{
    public class ServiceViewModel
    {
        public Guid OrganisationId { get; set; }
        public Guid ServiceId { get; set; }
        public OrganisationViewModelNoValidation organisationModel { get; set; }

        [ConditionallyRequired("UseOrganisationName", false)]
        [MaxLength(100)]
        [Display(Name = "Service Name", Description = "You must provide a service name.")]
        public string ServiceName { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation name")]
        public bool UseOrganisationName { get; set; }
        [Display(Name = "Do you want to claim this service?", Description = "By checking this box you claim editorial control of the service. This ensures only you can make changes without approval of the ALISS team.")]
        public bool ServiceRepresentative { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "My name:")]
        public string ServiceRepresentativeName { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "My role in the service:")]
        public string ServiceRepresentativeRole { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "My phone number:")]
        [PhoneNumberValidator]
        public string ServiceRepresentativePhone { get; set; }
        [ConditionallyRequiredBoolTrueValidator("ServiceRepresentative")]
        [Display(Name = "I understand and accept the terms above")]
        public bool ServiceAcceptDataStandards { get; set; }
        [Required]
        [MinLength(50, ErrorMessage = "The Service Summary must have a minimum of 50 and a maximum of 200 characters."), MaxLength(200, ErrorMessage = "The Service Summary must have a minimum of 50 and a maximum of 200 characters.")]
        [Display(Name = "Service Summary", Description = "Please add a short summary of the service. This summary will appear in ALISS search results and is what someone will see and hear about the service before they select it for more information.")]
        public string ServiceSummary { get; set; }
        [ConditionallyRequired("UseOrganisationDescription", false)]
        [Display(Name = "Service Description", Description = "Please provide a full description of the service. This should include any details that someone might wish to know about this service, including an explanation of what to expect from it, availability times, and any requirements. (Excluding any personal information)")]
        public string ServiceDescription { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation description")]
        public bool UseOrganisationDescription { get; set; }
        [Display(Name = "Tick here if all contact details are the same as the Organisation")]
        public bool UseAllOrganisationContactDetails { get; set; }
        [Display(Name = "Service Phone Number", Description = "Please enter a valid UK phone number.")]
        [PhoneNumberValidator]
        [RequiredContactDetails]
        public string PhoneNumber { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation phone number")]
        public bool UseOrganisationPhoneNumber { get; set; }
        [MaxLength(254)]
        [Display(Name = "Service Email", Description = "The most appropriate email address for someone to access or find out more about this service.")]
        [ValidEmailAddress]
        [RequiredContactDetails]
        public string Email { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation email")]
        public bool UseOrganisationEmail { get; set; }
        [MaxLength(200)]
        [Display(Name = "Service Web Address", Description = "The URL (web address) for the webpage of the service you would like to add. Copy and paste the link e.g. https://www.example.com/a-service")]
        [RequiredContactDetails]
        [UrlValidator]
        public string Url { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation web address")]
        public bool UseOrganisationUrl { get; set; }
        [MaxLength(500)]
        [Display(Name = "Service Referral Information URL", Description = "If the service requires a referral to access and/or has specific eligibility criteria for who can access it, please include the URL for the website page that best provides this information.")]
        [UrlValidator]
        public string ReferralUrl { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Name = "Service Facebook", Description = "A link to the official Facebook page for the service you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        [RequiredContactDetails]
        public string Facebook { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation Facebook page")]
        public bool UseOrganisationFacebook { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Name = "Service Twitter", Description = "A link to the official Twitter account of the service you would like to add e.g. https://twitter.com/rnibuk")]
        [RequiredContactDetails]
        public string Twitter { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation Twitter page")]
        public bool UseOrganisationTwitter { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Name = "Service Instagram", Description = "A link to the official Instagram page for the service you would like to add e.g. https://instagram.com/rnibuk")]
        [RequiredContactDetails]
        public string Instagram { get; set; }
        [Display(Name = "Tick here if it is the same as the Organisation Instagram page")]
        public bool UseOrganisationInstagram { get; set; }
        [MaxLength(200)]
        public string Slug { get; set; }
        public DataInputSummaryTypeEnum SummaryType { get; set; }

        public class RequiredContactDetailsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                ServiceViewModel instance = validationContext.ObjectInstance as ServiceViewModel;

                if (!instance.UseAllOrganisationContactDetails)
                {
                    if(instance.UseOrganisationPhoneNumber && 
                       !string.IsNullOrEmpty(instance.organisationModel.PhoneNumber) ||
                       !string.IsNullOrEmpty(instance.PhoneNumber))
                    {
                        return ValidationResult.Success;
                    }
                    else if (instance.UseOrganisationEmail && 
                       !string.IsNullOrEmpty(instance.organisationModel.Email) ||
                       !string.IsNullOrEmpty(instance.Email))
                    {
                        return ValidationResult.Success;
                    }
                    else if (instance.UseOrganisationUrl && 
                       !string.IsNullOrEmpty(instance.organisationModel.Url) ||
                       !string.IsNullOrEmpty(instance.Url))
                    {
                        return ValidationResult.Success;
                    }
                    else if (instance.UseOrganisationFacebook && 
                       !string.IsNullOrEmpty(instance.organisationModel.Facebook) ||
                       !string.IsNullOrEmpty(instance.Facebook))
                    {
                        return ValidationResult.Success;
                    }
                    else if (instance.UseOrganisationInstagram && 
                       !string.IsNullOrEmpty(instance.organisationModel.Instagram) ||
                       !string.IsNullOrEmpty(instance.Instagram))
                    {
                        return ValidationResult.Success;
                    }
                    else if (instance.UseOrganisationTwitter && 
                       !string.IsNullOrEmpty(instance.organisationModel.Twitter) ||
                       !string.IsNullOrEmpty(instance.Twitter))
                    {
                        return ValidationResult.Success;
                    }

                    return new ValidationResult("At least one contact field must be filled in before saving.");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }

   
        }
    }
}
