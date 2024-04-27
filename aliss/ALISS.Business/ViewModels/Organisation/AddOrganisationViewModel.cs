using System;
using System.ComponentModel.DataAnnotations;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;

namespace ALISS.Business.ViewModels.Organisation
{
    public class AddOrganisationViewModel
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
        public string PhoneNumber { get; set; }
        [MaxLength(254)]
        [Display(Description = "The most appropriate email address for someone to contact the organisation with")]
        [ValidEmailAddress]
        public string Email { get; set; }
        [MaxLength(200)]
        [Display(Name = "Web Address", Description = "The URL (website address) of the organisation\'s offical webpage e.g. https://www.organisation.com")]
        [UrlValidator]
        public string Url { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Description = "A link to the official Facebook page for the organisation you would like to add e.g. https://en-gb.facebook.com/rnibuk")]
        public string Facebook { get; set; }
        [MaxLength(200)]
        [UrlValidator]
        [Display(Description = "A link to the official Twitter account of the organisation you would like to add e.g. https://twitter.com/rnibuk")]
        public string Twitter { get; set; }
        [MaxLength(120)]
        [UrlValidator]
        [Display(Description = "A link to the official Twitter account of the organisation you would like to add e.g. https://instagram.com/rnibuk")]
        public string Instagram { get; set; }
        [MaxLength(120)]
        public string Slug { get; set; }
        [Display(Description = "An image file for the logo of the organisation you would like to add.")]
        public string Logo { get; set; }
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
    }
}