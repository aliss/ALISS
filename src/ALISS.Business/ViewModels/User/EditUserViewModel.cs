using ALISS.Business.Enums;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web.Mvc;


namespace ALISS.Business.ViewModels.User
{
    public class EditUserViewModel
    {
        public int UserProfileId { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9-_.]+$", ErrorMessage = "Username must be a single word (no spaces) and only consist of alphanumeric charachters.  You may also use dash (-) or underscore (_) or period (.)")]
        [Display(Description = "Your username for loggin into the ALISS admin site.")]
        public string Username { get; set; }
        [Required]
        [MaxLength(254, ErrorMessage = "Please enter a valid email address.")]
        [Display(Description = "The email from where we will contact you in future")]
        public string Email { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name must be no longer than 50 characters.")]
        [Display(Description = "Name and Surname")]
        public string Name { get; set; }

        [MaxLength(15)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Is this user an editor?")]
        public bool IsEditor { get; set; }
        [Display(Name = "Is this user an ALISS Admin?")]
        public bool IsAdmin { get; set; }
        public OrganisationListingViewModel UserOrganisations { get; set; }
        public ServiceListingViewModel UserServices { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public string SearchTerm { get; set; }

        public EditUserViewModel()
        {
            UserOrganisations = new OrganisationListingViewModel();
            UserServices = new ServiceListingViewModel();
        }
    }
}

