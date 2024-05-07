using System;
using System.ComponentModel.DataAnnotations;
using Tactuum.Core.Attributes;

namespace ALISS.Business.ViewModels.Location
{
    public class EditLocationViewModel
    {
        public Guid LocationId { get; set; }

        [Display(Description = "The Name you wish to give to the new location.")]
        [MaxLength(100, ErrorMessage = "Location name must be no longer than 100 characters.")]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Description = "The street address of the new location to add.")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "City/Town", Description = "The town or city that the new location is located in.")]
        [MaxLength(30)]
        public string City { get; set; }
        [Required]
        [MaxLength(10)]
        [ValidPostcode]
        [Display(Description = "The postcode of the new location.")]
        public string Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
