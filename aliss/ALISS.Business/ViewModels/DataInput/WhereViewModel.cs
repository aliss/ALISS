using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ALISS.Business.PresentationTransferObjects.ServiceArea;
using ALISS.Business.ViewModels.Service;
using Nest;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;
using ALISS.Business.PresentationTransferObjects.Location;
using ALISS.Business.Enums;

namespace ALISS.Business.ViewModels.DataInput
{
    public class WhereViewModel
    {
		public Guid OrganisationId { get; set; }

        public Guid ServiceId { get; set; }

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
        public DataInputSummaryTypeEnum SummaryType { get; set; }

        [Display(Name = "Name", Description = "The Name you wish to give to the location.")]
        public string EditLocationName { get; set; }

        [Display(Name = "Address", Description = "The street address of the location to add.")]
        public string EditLocationAddress { get; set; }

        [Display(Name = "City/Town ", Description = "The town or city that the location is located in.")]
        public string EditLocationCity { get; set; }

        [Display(Name = "Postcode", Description = "The postcode of the location.")]

        public string EditLocationPostcode { get; set; }

        public string EditLocationLatitude { get; set; }

        public string EditLocationLongitude { get; set; }

		public string EditLocationId {  get; set; }
	}
}
