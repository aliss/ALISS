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
    public class OrganisationViewModelNoValidation
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationRepresentative { get; set; }
        public string OrganisationRepresentativeName { get; set; }
        public string OrganisationRepresentativeRole { get; set; }
        public string OrganisationRepresentativePhone { get; set; }
        public bool OrganisationAcceptDataStandards { get; set; }
        public string OrganisationDescription { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Slug { get; set; }

        public OrganisationViewModelNoValidation() { }

        public OrganisationViewModelNoValidation(OrganisationViewModel model)
        {
            OrganisationId = model.OrganisationId;
            OrganisationName = model.OrganisationName;
            OrganisationRepresentative = model.OrganisationRepresentative;
            OrganisationRepresentativeName = model.OrganisationRepresentativeName;
            OrganisationRepresentativeRole = model.OrganisationRepresentativeRole;
            OrganisationRepresentativePhone = model.OrganisationRepresentativePhone;
            OrganisationAcceptDataStandards = model.OrganisationAcceptDataStandards;
            OrganisationDescription = model.OrganisationDescription;
            PhoneNumber = model.PhoneNumber;
            Email = model.Email;
            Url = model.Url;
            Facebook = model.Facebook;
            Twitter = model.Twitter;
            Instagram = model.Instagram;
            Slug = model.Slug;
        }
    }
}
