using ALISS.Business.PresentationTransferObjects.Service;
using System;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.Location
{
    public class DeleteLocationViewModel
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid OrganisationId { get; set; }
        public bool CanDelete { get; set; }
        public List<RelatedServicePTO> RelatedServices { get; set; }
    }
}
