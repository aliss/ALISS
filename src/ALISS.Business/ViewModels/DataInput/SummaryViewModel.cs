using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.AccessibilityFeature;
using ALISS.Business.PresentationTransferObjects.DataInput;
using ALISS.Business.Validators;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;
using Nest;
using Tactuum.Core.Attributes;

namespace ALISS.Business.ViewModels.DataInput
{
    public class validation
    {
        public const int NameMax = 100;
        public const int SummaryMin = 50;
        public const int SummaryMax = 200;
        public const int EmailMax = 254;
        public const int UrlMax = 200;
        public const int ReferalUrlMax = 500;
        public const int AltTextMax = 140;
        public const int CaptionMax = 200;
    }

    public class SummaryViewModel
    {
        public bool ServiceSubmitted { get; set; }
        public DataInputSummaryTypeEnum SummaryType { get; set; }

        //Organisation
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationRepresentative { get; set; }
        public string OrganisationDescription { get; set; }
        public string OrganisationPhoneNumber { get; set; }
        public string OrganisationEmail { get; set; }
        public string OrganisationUrl { get; set; }
        public string OrganisationFacebook { get; set; }
        public string OrganisationTwitter { get; set; }
        public string OrganisationInstagram { get; set; }

        //Service
        public Guid ServiceId { get; set; }
        [Required]
        [MaxLength(validation.NameMax)]
        [Display(Name = "Service Name", Description = "You must provide a service name.")]
        public string ServiceName { get; set; }
        public bool ServiceRepresentative { get; set; }
        [Required]
        [MinLength(validation.SummaryMin, ErrorMessage = "The Service Summary must have a minimum of 50 and a maximum of 200 characters."), MaxLength(validation.SummaryMax, ErrorMessage = "The Service Summary must have a minimum of 50 and a maximum of 200 characters.")]
        [Display(Name = "Service Summary", Description = "Please add a short summary of the service. This summary will appear in ALISS search results and is what someone will see and hear about the service before they select it for more information.")]
        public string ServiceSummary { get; set; }
        [Required]
        [Display(Name = "Service Description", Description = "Please provide a full description of the service. This should include any details that someone might wish to know about this service, including an explanation of what to expect from it, availability times, and any requirements. (Excluding any personal information)")]
        public string ServiceDescription { get; set; }
        [PhoneNumberValidator]
        [RequiredServiceContactDetails]
        [Display(Name = "Service Phone Number", Description = "Please enter a valid UK phone number.")]
        public string ServicePhoneNumber { get; set; }
        [MaxLength(validation.EmailMax)]
        [RequiredServiceContactDetails]
        [Display(Name = "Service Email", Description = "The most appropriate email address for someone to access or find out more about this service.")]
        public string ServiceEmail { get; set; }
        [RequiredServiceContactDetails]
        [MaxLength(validation.UrlMax)]
        [UrlValidator]
        [Display(Name = "Service Web Address", Description = "The URL (web address) for the webpage of the service you would like to add. Copy and paste the link e.g. https://www.example.com/a-service")]
        public string ServiceUrl { get; set; }
        [MaxLength(validation.ReferalUrlMax)]
        [UrlValidator]
        [Display(Name = "Service Referral Information URL", Description = "If the service requires a referral to access and/or has specific eligibility criteria for who can access it, please include the URL for the website page that best provides this information.")]
        public string ServiceReferalUrl { get; set; }
        [RequiredServiceContactDetails]
        [MaxLength(validation.UrlMax)]
        [UrlValidator]
        [Display(Name = "Service Facebook", Description = "A link to the official Facebook page for the service you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        public string ServiceFacebook { get; set; }
        [RequiredServiceContactDetails]
        [MaxLength(validation.UrlMax)]
        [UrlValidator]
        [Display(Name = "Service Twitter", Description = "A link to the official Twitter account of the service you would like to add e.g. https://twitter.com/rnibuk")]
        public string ServiceTwitter { get; set; }
        [RequiredServiceContactDetails]
        [MaxLength(validation.UrlMax)]
        [UrlValidator]
        [Display(Name = "Service Instagram", Description = "A link to the official Instagram page for the service you would like to add e.g. https://instagram.com/rnibuk")]
        public string ServiceInstagram { get; set; }

        //Where
        public List<WhereLocationPTO> SelectedLocations { get; set; }
        public List<string> SelectedServiceAreas { get; set; }
        public string HowServiceAccessed { get; set; }
        public List<ALISS.Models.Models.Location> ServiceLocations { get; set; }
        public List<ALISS.Models.Models.ServiceArea> ServiceServiceAreas { get; set; }

        //Categories
        [Required(ErrorMessage = "You must select at least one category for this service")]
        public List<string> SelectedCategories { get; set; }

        //Who
        public List<string> SelectedCommunityGroups { get; set; }

        //Accessibility
        public List<string> SelectedAccessibilityFeatures { get; set; }

        public List<LocationAccessibilityFeaturePTO> SelectedAccessibilityFeatureLocationObjects { get; set; }

        //Media
        public string OrganisationLogo { get; set; }

        [FileTypeValidator]
        public string ServiceLogo { get; set; }

        [FileTypeValidator]
        public string ServiceImage1 { get; set; }
        [MaxLength(validation.AltTextMax)]
        public string ServiceImage1AltText { get; set; }
        [MaxLength(validation.CaptionMax)]
        public string ServiceImage1Caption { get; set; }

        [FileTypeValidator]
        public string ServiceImage2 { get; set; }
        [MaxLength(validation.AltTextMax)]
        public string ServiceImage2AltText { get; set; }
        [MaxLength(validation.CaptionMax)]
        public string ServiceImage2Caption { get; set; }

        [FileTypeValidator]
        public string ServiceImage3 { get; set; }
        [MaxLength(validation.AltTextMax)]
        public string ServiceImage3AltText { get; set; }
        [MaxLength(validation.CaptionMax)]
        public string ServiceImage3Caption { get; set; }

        [EmbededVideoValidator]
        [Url]
        public string ServiceVideo { get; set; }
    }

    public class RequiredServiceContactDetailsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance as SummaryViewModel;

            if (string.IsNullOrEmpty(instance.ServicePhoneNumber) &&
                string.IsNullOrEmpty(instance.ServiceEmail) &&
                string.IsNullOrEmpty(instance.ServiceUrl) &&
                string.IsNullOrEmpty(instance.ServiceTwitter) &&
                string.IsNullOrEmpty(instance.ServiceFacebook) &&
                string.IsNullOrEmpty(instance.ServiceInstagram))
            {
                return new ValidationResult("At least one contact field must be filled in before saving.");
            }
            return ValidationResult.Success;
        }
    }
}
