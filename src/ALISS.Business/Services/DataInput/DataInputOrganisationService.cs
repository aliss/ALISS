using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Tactuum.Core.Util;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public OrganisationViewModel GetEmptyOrganisationModel(Guid organisationId, bool newOrganisation = false)
        {
            string organisationName = null;
            using (var dc = new ALISSContext())
            {
                if (!newOrganisation)
                {
                    var organisation = dc.Organisations.FirstOrDefault(o => o.OrganisationId == organisationId);
                    organisationName = organisation.Name;
                }
            }

            OrganisationViewModel model = new OrganisationViewModel()
            {
                OrganisationId = organisationId,
                OrganisationName = organisationName
            };

            return model;
        }

        public OrganisationViewModel GetOrganisationForEdit(Guid organisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisationToEdit = dc.Organisations.Find(organisationId);
                OrganisationViewModel organisation = new OrganisationViewModel()
                {
                    OrganisationId = organisationToEdit.OrganisationId,
                    OrganisationName = organisationToEdit.Name,
                    OrganisationDescription = organisationToEdit.Description,
                    PhoneNumber = organisationToEdit.PhoneNumber,
                    Email = organisationToEdit.Email,
                    Url = organisationToEdit.Url,
                    Facebook = organisationToEdit.Facebook,
                    Twitter = organisationToEdit.Twitter,
                    Instagram = organisationToEdit.Instagram,
                    Slug = organisationToEdit.Slug,
                };

                Claim claim = dc.Claims.FirstOrDefault(o => o.OrganisationId == organisation.OrganisationId);
                organisation.OrganisationRepresentative = claim != null;
                organisation.OrganisationRepresentativeName = claim?.RepresentativeName;
                organisation.OrganisationRepresentativePhone = claim?.RepresentativePhone;
                organisation.OrganisationRepresentativeRole = claim?.RepresentativeRole;
                organisation.OrganisationAcceptDataStandards = claim != null;

                return organisation;
            }
        }

        public string AddOrganisation(AddOrganisationViewModel model, int currentUserProfileId, ApplicationUserManager userManager, out Guid organisationId)
        {
            if (_organisationService.DoesOrganisationExist(model.Name))
            {
                organisationId = new Guid();
                return $"Error: The organisation {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Find(currentUserProfileId);
                ApplicationUser applicationUser = userManager.FindByName(userProfile.Username);
                bool isBasicUser = userManager.IsInRole(applicationUser.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(applicationUser.Id, RolesEnum.ClaimedUser.ToString());

                Organisation organisationToAdd = new Organisation()
                {
                    OrganisationId = model.OrganisationId == null ? Guid.NewGuid() : model.OrganisationId,
                    Name = model.Name,
                    CreatedOn = DateTime.UtcNow.Date,
                    Description = model.Description,
                    Email = model.Email,
                    Facebook = model.Facebook,
                    PhoneNumber = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber,
                    Twitter = model.Twitter,
                    Instagram = model.Instagram,
                    Published = false,
                    Slug = String.IsNullOrEmpty(model.Slug) || model.Slug.StartsWith("-") ? SlugGenerator.GenerateSlug(model.Name) : model.Slug,
                    Url = model.Url,
                    CreatedUserId = currentUserProfileId,
                    Submitted = false
                };

                dc.Organisations.Add(organisationToAdd);

                var organisationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = organisationToAdd.OrganisationId,
                    UserProfileId = currentUserProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organisationAudit);

                organisationId = organisationToAdd.OrganisationId;

                dc.SaveChanges();

                if (model.OrganisationRepresentative)
                {
                    try
                    {
                        AddClaimViewModel claimToAdd = new AddClaimViewModel()
                        {
                            ClaimedUserId = currentUserProfileId,
                            Id = organisationToAdd.OrganisationId,
                            RepresentativeName = model.OrganisationRepresentativeName,
                            RepresentativePhone = model.OrganisationRepresentativePhone,
                            RequestLeadClaimant = true,
                            RepresentativeRole = model.OrganisationRepresentativeRole
                        };

                        ClaimService.AddClaim(claimToAdd);
                    }
                    catch (Exception)
                    {
                        return $"Claim Error: The organisation was created but there was a problem when trying to set up your claim.  Once your organisation is published, you can submit a new claim.";
                    }
                }
            }

            return "Organisation added successfully";
        }

        public bool isOrganisationSubmitted(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(id);
                return organisation != null ? organisation.Submitted : false;
            }
        }
    }
}
