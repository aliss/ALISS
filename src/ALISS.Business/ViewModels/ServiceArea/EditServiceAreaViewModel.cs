using ALISS.Business.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ALISS.Business.ViewModels.ServiceArea
{
    public class EditServiceAreaViewModel
    {
        public int ServiceAreaId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Service Area Name must be no longer than 50 characters.")]
        [Display(Description = "The name of the service area")]
        public string Name { get; set; }
        [Display(Description = "The URL friendly slug for this service area - this is auto generated")]
        public string Slug { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Service Area Code must be no longer than 10 characters.")]
        [Display(Description = "The Scottish Government Standard Code")]
        public string Code { get; set; }
        [Display(Name = "Type e.g Health Board..", Description = "Country, Health Board, Location Authority")]
        public ServiceAreaTypeEnum Type { get; set; }
        public List<SelectListItem> ServiceAreaType { get; set; }
        [Display(Name = "GeoJSON data", Description = "The boundary coordinates for the service area")]
        public string GeoJson { get; set; }
    }
}
