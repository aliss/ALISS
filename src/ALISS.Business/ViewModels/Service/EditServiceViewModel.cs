using ALISS.Business.PresentationTransferObjects.Category;
using ALISS.Business.PresentationTransferObjects.ServiceArea;
using ALISS.Business.ViewModels.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;
using ALISS.Business.PresentationTransferObjects.Location;

namespace ALISS.Business.ViewModels.Service
{
    public class EditServiceViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Service Name", Description = "Please enter the official name of the service you would like to add.")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "The summary field must be a maximum of 200 characters long.")]
        [Display(Name = "Service Summary", Description = "Please add a short summary of the service. This summary will appear in the ALISS search results page and is what someone will see about the service before they select it for more information.")]
        public string Summary { get; set; }
        [Required]
        [Display(Name = "Service Description", Description = " Include information on what your service does, who you help and opening times. Feature words you think people would search for, assume those searching have never heard of your organisation or service.")]
        public string Description { get; set; }
        [Display(Name = "Service Phone Number", Description = "Please enter a valid UK phone number.")]
        [PhoneNumberValidator]
		[RequiredContactDetails]
        public string PhoneNumber { get; set; }
        [MaxLength(254)]
        [Display(Description = "The most appropriate email address for someone to access or find out more about this service.")]
        [ValidEmailAddress]
        [RequiredContactDetails]
        public string Email { get; set; }
        [MaxLength(200)]
        [RequiredContactDetails]
        [Display(Name = "Service Web Address", Description = "The URL (web address) for the webpage of the service you would like to add. Copy and paste the link e.g. https://www.example.com/a-service")]
        [UrlValidator]
        public string Url { get; set; }
        [MaxLength(500)]
        [Display(Name = "Referral Information URL", Description = "If the service requires a referral to access and/or has specific eligibility criteria for who can access it, please include the URL for the website page that best provides this information.")]
        [RequiredContactDetails]
        [UrlValidator]
        public string ReferralUrl { get; set; }
        [MaxLength(200)]
        [RequiredContactDetails]
        [UrlValidator]
        [Display(Description = "A link to the official Facebook page for the service you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        public string Facebook { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [RequiredContactDetails]
        [Display(Description = "A link to the official Twitter account of the service you would like to add e.g. https://twitter.com/rnibuk")]
        public string Twitter { get; set; }
        [MaxLength(120)]
        [Display(Description = "A link to the official Instagram page for the service you would like to add e.g. https://www.instagram.com/rnibuk")]
        [RequiredContactDetails]
        [UrlValidator]
        public string Instagram { get; set; }
        [MaxLength(200)]
        public string Slug { get; set; }
        public List<ServiceCategoryPTO> ServiceCategories { get; set; }
        [Required(ErrorMessage = "You must select at least one category for this service")]
        public string SelectedCategories { get; set; }
        public string HowServiceAccessed { get; set; }
        [LocationValidator]
        public string SelectedLocations { get; set; }
        public List<ServiceLocationPTO> Locations { get; set; }
        [Display(Name = "Name", Description = "The Name you wish to give to the new location.")]
        public string NewLocationName { get; set; }
        [Display(Name = "Address", Description = "The street address of the new location to add.")]
        public string NewLocationAddress { get; set; }
        [Display(Name = "City/Town ", Description = "The town or city that the new location is located in.")]
        public string NewLocationCity { get; set; }
        [Display(Name = "Postcode", Description = "The postcode of the new location.")]
        public string NewLocationPostcode { get; set; }
        public string NewLocationLatitude { get; set; }
        public string NewLocationLongitude { get; set; }
        public string SelectedServiceAreas { get; set; }
        public List<ServiceServiceAreaPTO> ServiceServiceAreas { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsClaimed { get; set; }
        public bool Published { get; set; }
        public string Logo { get; set; }

        public bool NewOrganisation { get; set; }
        [Display(Name = "Organisation Name", Description = "You must provide an organisation name to link your service to.")]
        [Required]
        public string NewOrganisationName { get; set; }

        [Display(Name = "I want to claim this organisation", Description = "By checking this box you claim editorial control of the organisation. This ensures only you can make changes without approval of the ALISS team.")]
        public bool OrganisationRepresentative { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "Representative Name")]
        public string OrganisationRepresentativeName { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "What is your role in the organisation?")]
        public string OrganisationRepresentativeRole { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "Representative Phone Number")]
        [PhoneNumberValidator]
        public string OrganisationRepresentativePhone { get; set; }
        [ConditionallyRequired("OrganisationRepresentative", true)]
        [Display(Name = "I understand and acknowledge the importance of data quality and agree to follow the guidance outlined in the ALISS Data Standards")]
        public bool OrganisationAcceptDataStandards { get; set; }

        [Display(Name = "I want to claim this service", Description = "By checking this box you claim editorial control of the service. This ensures only you can make changes without approval of the ALISS team.")]
        public bool ServiceRepresentative { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "Representative Name")]
        public string ServiceRepresentativeName { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "What is your role in the service?")]
        public string ServiceRepresentativeRole { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "Representative Phone Number")]
        [PhoneNumberValidator]
        public string ServiceRepresentativePhone { get; set; }
        [ConditionallyRequired("ServiceRepresentative", true)]
        [Display(Name = "I understand and acknowledge the importance of data quality and agree to follow the guidance outlined in the ALISS Data Standards")]
        public bool ServiceAcceptDataStandards { get; set; }

        public class RequiredContactDetailsAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var instance = validationContext.ObjectInstance as EditServiceViewModel;

                if (string.IsNullOrEmpty(instance.PhoneNumber) &&
                    string.IsNullOrEmpty(instance.Email) &&
                    string.IsNullOrEmpty(instance.Url) &&
                    string.IsNullOrEmpty(instance.ReferralUrl) &&
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
