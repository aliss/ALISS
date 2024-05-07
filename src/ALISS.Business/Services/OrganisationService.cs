using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Organisation;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Nest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Tactuum.Core.Util;
using ALISS.Business.ViewModels.User;
using System.Web;
using ALISS.Business.PresentationTransferObjects.Location;
using ALISS.Business.ViewModels.Location;
using ALISS.Business.Migrations;
using ALISS.Business.ViewModels.Service;
using System.Configuration;

namespace ALISS.Business.Services
{

    public class OrganisationService
    {
        private int _ItemsPerPage = 10;
        private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!/\\-, '']";
        private ElasticSearchService _elasticSearchService;
        private UserProfileService _userProfileService;
        private EmailService _emailService;
        private string publicBaseUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public OrganisationService()
        {
            _elasticSearchService = new ElasticSearchService();
            _emailService = new EmailService();
        }

        public OrganisationListingViewModel GetOrganisations(ApplicationUserManager userManager, string username, string searchTerm, int page = 1, bool unpublished = false, string orderBy = "createdon", int descending = 1)
        {
            OrganisationListingViewModel organisationList = new OrganisationListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Organisations = new List<OrganisationPTO>()
            };
            List<Guid> userOrganisations = GetUserOrganisations(userManager, username, out bool isAdmin, true);

            using (ALISSContext dc = new ALISSContext())
            {
                List<Organisation> organisations = new List<Organisation>();

                organisations = userOrganisations.Count > 0
                    ? dc.Organisations.Include(u => u.CreatedUser).Include(u => u.UpdatedUser).Include(u => u.ClaimedUser).Where(c => userOrganisations.Contains(c.OrganisationId) && c.Submitted).OrderBy(n => n.Name).ToList()
                    : isAdmin
                        ? unpublished
                            ? dc.Organisations.Include(u => u.CreatedUser).Include(u => u.UpdatedUser).Include(u => u.ClaimedUser).Where(p => p.Published == false && p.Submitted).OrderBy(n => n.Name).ToList()
                            : dc.Organisations.Include(u => u.CreatedUser).Include(u => u.UpdatedUser).Include(u => u.ClaimedUser).Where(s => s.Submitted).OrderBy(n => n.Name).ToList()
                        : new List<Organisation>();

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);

                organisations = string.IsNullOrEmpty(searchTerm)
                    ? organisations
                    : organisations.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower())).ToList();

                organisationList.TotalPages = (int)Math.Ceiling((double)organisations.Count / _ItemsPerPage);
                organisationList.TotalResults = organisations.Count;
                int skip = (page - 1) * _ItemsPerPage;
                //if (unpublished)
                //{
                //    organisations = organisations.OrderByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                //}
                //else
                //{
                //    organisations = organisations.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                //}
                switch (orderBy.ToLower())
                {
                case "organisationname":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "claimedon":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.ClaimedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.ClaimedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "updatedon":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "createdon":
                default:
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                }

                foreach (var organisation in organisations)
                {
                    OrganisationPTO organisationListItem = new OrganisationPTO()
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.Name,
                        CreatedUserId = organisation.CreatedUserId,
                        CreatedUserName = String.IsNullOrEmpty(organisation.CreatedUser.Name) ? organisation.CreatedUser.Email.Substring(0, organisation.CreatedUser.Email.IndexOf("@")) : organisation.CreatedUser.Name,
                        CreatedOn = organisation.CreatedOn,
                        UpdatedUserId = organisation.UpdatedUserId,
                        UpdatedUserName = organisation.UpdatedUserId.HasValue ? String.IsNullOrEmpty(organisation.UpdatedUser.Name) ? organisation.UpdatedUser.Email.Substring(0, organisation.UpdatedUser.Email.IndexOf("@")) : organisation.UpdatedUser.Name : "",
                        UpdatedOn = organisation.UpdatedUserId.HasValue ? organisation.UpdatedOn : null,
                        ClaimedUserId = organisation.ClaimedUserId,
                        ClaimedUserName = organisation.ClaimedUserId.HasValue ? String.IsNullOrEmpty(organisation.ClaimedUser.Name) ? organisation.ClaimedUser.Email.Substring(0, organisation.ClaimedUser.Email.IndexOf("@")) : organisation.ClaimedUser.Name : "",
                        ClaimedOn = organisation.ClaimedOn,
                        Published = organisation.Published,
                        CanEdit = (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(organisation.OrganisationId) || ValidOrganisationForEditor(userManager, username, organisation.OrganisationId),
                    };
                    organisationList.Organisations.Add(organisationListItem);
                }
            }

            return organisationList;
        }

        public OrganisationListingViewModel GetAllOrganisations(ApplicationUserManager userManager, string username, String searchTerm, int page = 1, string orderBy = "createdon", int descending = 1, bool isAdmin = true)
        {
            OrganisationListingViewModel organisationList = new OrganisationListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Organisations = new List<OrganisationPTO>()
            };

            List<Guid> userOrganisations = GetUserOrganisations(userManager, username, out isAdmin, true);

            using (ALISSContext dc = new ALISSContext())
            {
                List<Organisation> organisations = isAdmin 
                    ? dc.Organisations.Where(x => x.Submitted).OrderBy(n => n.Name).ToList() 
                    : dc.Organisations.Where(x => x.Published && x.Submitted).OrderBy(n => n.Name).ToList();

                searchTerm = Regex.Replace(searchTerm, regexMatch, String.Empty);

                organisations = string.IsNullOrEmpty(searchTerm) ? organisations : organisations
                    .Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty)
                    .Contains(searchTerm.ToLower())).ToList();

                organisationList.TotalPages = (int)Math.Ceiling((double)organisations.Count / _ItemsPerPage);
                organisationList.TotalResults = organisations.Count;
                int skip = (page - 1) * _ItemsPerPage;

                switch (orderBy.ToLower())
                {
                case "organisationname":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "claimedon":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.ClaimedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.ClaimedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "updatedon":
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "createdon":
                default:
                    organisations = descending == 1
                        ? organisations.OrderByDescending(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : organisations.OrderBy(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                }

                foreach (var organisation in organisations)
                {
                    OrganisationPTO organisationListitem = new OrganisationPTO()
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.Name,
                        CreatedOn = organisation.CreatedOn,
                        UpdatedOn = organisation.UpdatedOn,
                        ClaimedOn = organisation.ClaimedOn,
                        Published = organisation.Published,
                        UpdatedUserId = organisation.UpdatedUserId,
                        ClaimedUserId = organisation.ClaimedUserId,
                        CanEdit = (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(organisation.OrganisationId) || ValidOrganisationForEditor(userManager, username, organisation.OrganisationId),
                    };
                    organisationList.Organisations.Add(organisationListitem);
                }     
             }
                           
            return organisationList;
        }

        public string AddOrganisation(AddOrganisationViewModel model, int currentUserProfileId, ApplicationUserManager userManager, out Guid organisationId)
        {
            if (DoesOrganisationExist(model.Name))
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
                    Published = isBasicUser ? false : true,
                    Slug = String.IsNullOrEmpty(model.Slug) ? SlugGenerator.GenerateSlug(model.Name) : model.Slug,
                    Url = model.Url,
                    CreatedUserId = currentUserProfileId,
                    Submitted = true
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

                if (isBasicUser)
                {
                    _emailService.SendNewOrganisationEmail(String.IsNullOrEmpty(userProfile.Name) ? userProfile.Username : userProfile.Name, currentUserProfileId);
                }
                else
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(organisationId);
                }

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

        public string EditTemporaryOrganisation(AddOrganisationViewModel model, int currentUserProfileId, ApplicationUserManager userManager)
        {
            if (DoesOrganisationExist(model.Name))
            {
                return $"Error: The organisation {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Find(currentUserProfileId);
                ApplicationUser applicationUser = userManager.FindByName(userProfile.Username);
                bool isBasicUser = userManager.IsInRole(applicationUser.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(applicationUser.Id, RolesEnum.ClaimedUser.ToString());

                Organisation organisationToAdd = dc.Organisations.Find(model.OrganisationId);
                organisationToAdd.Name = model.Name;
                organisationToAdd.CreatedOn = DateTime.UtcNow.Date;
                organisationToAdd.Description = model.Description;
                organisationToAdd.Email = model.Email;
                organisationToAdd.Facebook = model.Facebook;
                organisationToAdd.PhoneNumber = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber;
                organisationToAdd.Twitter = model.Twitter;
                organisationToAdd.Instagram = model.Instagram;
                organisationToAdd.Published = isBasicUser ? false : true;
                organisationToAdd.Slug = String.IsNullOrEmpty(model.Slug) ? SlugGenerator.GenerateSlug(model.Name) : model.Slug;
                organisationToAdd.Url = model.Url;
                organisationToAdd.CreatedUserId = currentUserProfileId;

                dc.SaveChanges();

                var organisationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = organisationToAdd.OrganisationId,
                    UserProfileId = currentUserProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organisationAudit);

                dc.SaveChanges();

                if (model.OrganisationRepresentative)
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

                if (isBasicUser)
                {
                    _emailService.SendNewOrganisationEmail(String.IsNullOrEmpty(userProfile.Name) ? userProfile.Username : userProfile.Name, currentUserProfileId);
                }
                else
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                }
            }

            return "Organisation added successfully";
        }

        public EditOrganisationViewModel GetOrganisationForEdit(Guid organisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisationToEdit = dc.Organisations.Find(organisationId);
                return new EditOrganisationViewModel()
                {
                    OrganisationId = organisationToEdit.OrganisationId,
                    Name = organisationToEdit.Name,
                    Description = organisationToEdit.Description,
                    PhoneNumber = String.IsNullOrEmpty(organisationToEdit.PhoneNumber) ? organisationToEdit.PhoneNumber : organisationToEdit.PhoneNumber.Replace(" ", ""),
                    Email = organisationToEdit.Email,
                    Url = organisationToEdit.Url,
                    Facebook = organisationToEdit.Facebook,
                    Twitter = organisationToEdit.Twitter,
                    Instagram = organisationToEdit.Instagram,
                    Slug = organisationToEdit.Slug,
                    Published = organisationToEdit.Published,
                    Logo = organisationToEdit.Logo
                };
            }
        }

        public string ChangeLeadOrganisationClaimant(MakeLeadOrganisationClaimantViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Models.Models.OrganisationClaimUser oldClaimant = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == model.Claimant.OrganisationId && x.IsLeadClaimant).FirstOrDefault();
                oldClaimant.IsLeadClaimant = false;

                Models.Models.OrganisationClaimUser newLeadClaimant = dc.OrganisationClaimUsers.Find(model.Claimant.OrganisationClaimUserId);
                newLeadClaimant.IsLeadClaimant = true;

                Organisation organisation = dc.Organisations.Find(model.Claimant.OrganisationId);
                organisation.ClaimedUserId = newLeadClaimant.ClaimedUserId;
                organisation.ClaimedOn = DateTime.Now;

                dc.SaveChanges();

                if (organisation.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(organisation.OrganisationId);
                }

                UserProfile userProfile = dc.UserProfiles.Find(newLeadClaimant.ClaimedUserId.Value);

                _emailService.SendLeadClaimantChangeEmail(userProfile.Email, userProfile.Name, userProfile.Username, organisation.Name, $"{publicBaseUrl}/organisations/{organisation.Slug}", model.Message);
            }

            return "Lead claimant changed successfully";
        }

        public bool IsOrganisationPublished(Guid organisationId)
        {
            using (var dc = new ALISSContext())
            {
                var organisation = dc.Organisations.Find(organisationId);
                return organisation.Published;
            }
        }

        public string EditOrganisation(EditOrganisationViewModel model, int currentUserProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var organisationToEdit = dc.Organisations.Find(model.OrganisationId);
                if (model.Name.ToLower() != organisationToEdit.Name.ToLower() && DoesOrganisationExist(model.Name))
                {
                    return $"Error: The organisation {model.Name} already exists, please choose another name.";
                }

                organisationToEdit.Name = model.Name;
                organisationToEdit.Description = model.Description;
                organisationToEdit.PhoneNumber = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber;
                organisationToEdit.Email = model.Email;
                organisationToEdit.Url = model.Url;
                organisationToEdit.Facebook = model.Facebook;
                organisationToEdit.Twitter = model.Twitter;
                organisationToEdit.Instagram = model.Instagram;
                organisationToEdit.Slug = String.IsNullOrEmpty(model.Slug) ? SlugGenerator.GenerateSlug(model.Name) : model.Slug;
                organisationToEdit.UpdatedUserId = currentUserProfileId;
                organisationToEdit.UpdatedOn = DateTime.UtcNow.Date;

                var organisationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = model.OrganisationId,
                    UserProfileId = currentUserProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organisationAudit);

                dc.SaveChanges();
                
                if (organisationToEdit.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                    List<Service> organisationServices = dc.Services.Where(o => o.OrganisationId == model.OrganisationId && !o.Suggested).ToList();
                    foreach (Service service in organisationServices)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                    }
                }
            }

            return "Organisation edited successfully";
        }

        public void AddLogoToOrganisation(Guid organisationId, string logo)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(organisationId);
                organisation.Logo = logo;
                dc.SaveChanges();

                if (organisation.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(organisationId);
                }
            }
        }

        public void PublishOrganisation(Guid organisationId, int currentUserProfileId)
        {
            using (var dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(organisationId);
                organisation.Published = !organisation.Published;
                organisation.UpdatedUserId = currentUserProfileId;
                organisation.UpdatedOn = DateTime.UtcNow.Date;

                var organisationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = organisationId,
                    UserProfileId = currentUserProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organisationAudit);
                List<Service> organisationServices = dc.Services.Where(o => o.OrganisationId == organisationId && !o.Suggested).ToList();

                dc.SaveChanges();

                if (organisation.Published)
                {
                    foreach (Service service in organisationServices)
                    {
                        if (!service.Published)
                        {
                            service.Published = true;
                            dc.SaveChanges();
                        }

                        _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                    }

                    // We need to index the organisation after publishing the services as the indexed organisation includes the services
                    UserProfile userProfile = dc.UserProfiles.Find(organisation.CreatedUserId);
                    _emailService.SendOrganisationPublishedEmail(userProfile.Email, userProfile.Name, userProfile.Username, organisation.Name, organisation.Slug);
                    _elasticSearchService.AddOrganisationToElasticSearch(organisationId);
                }
                else
                {
                    _elasticSearchService.DeleteOrganisation(organisationId);
                    foreach (Service service in organisationServices)
                    {
                        _elasticSearchService.DeleteService(service.ServiceId);
                        if (service.Published)
                        {
                            var serviceUserProfile = dc.UserProfiles.Find(service.ClaimedUserId ?? service.CreatedUserId);
                            _emailService.SendServiceUnpublishedEmail(serviceUserProfile.Email, serviceUserProfile.Name, serviceUserProfile.Username, service.Name);
                            service.Published = false;
                            dc.SaveChanges();
                        }
                    }
                }
            }
        }

        public DeleteOrganisationViewModel GetOrganisationForDelete(Guid organisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var organisation = dc.Organisations.Find(organisationId);

                return new DeleteOrganisationViewModel()
                {
                    OrganisationId = organisationId,
                    OrganisationName = organisation.Name
                };
            }
        }

        public void DeleteOrganisation(Guid organisationId, out string errorMessage)
        {
            errorMessage = "";
            List<Guid> serviceIdsToRemove = new List<Guid>();

            try
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    var servicesToRemove = dc.Services.Where(o => o.OrganisationId == organisationId);
                    serviceIdsToRemove = servicesToRemove.Select(s => s.ServiceId).ToList();
                    Organisation organisationToDelete = dc.Organisations.Find(organisationId);

                    foreach (Guid serviceId in serviceIdsToRemove)
                    {
                        _elasticSearchService.DeleteService(serviceId);
                    }

                    foreach (Guid serviceId in serviceIdsToRemove)
                    {
                        if (_elasticSearchService.GetServiceById(serviceId) != null)
                        {
                            errorMessage = "Failed to delete all service of this organisation from the index, please try again.";
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        _elasticSearchService.DeleteOrganisation(organisationId);

                        if (_elasticSearchService.GetOrganisationById(organisationId) == null)
                        {
                            var collectionServicesToRemove = dc.CollectionServices.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (collectionServicesToRemove.Count() > 0)
                            {
                                dc.CollectionServices.RemoveRange(collectionServicesToRemove);
                            }

                            var serviceClaimUsersToRemove = dc.ServiceClaimUsers.Where(s => serviceIdsToRemove.Contains(s.ServiceId.Value));
                            if (serviceClaimUsersToRemove.Count() > 0)
                            {
                                dc.ServiceClaimUsers.RemoveRange(serviceClaimUsersToRemove);
                            }

                            var serviceClaimsToRemove = dc.ServiceClaims.Where(s => serviceIdsToRemove.Contains(s.ServiceId.Value));
                            if (serviceClaimsToRemove.Count() > 0)
                            {
                                dc.ServiceClaims.RemoveRange(serviceClaimsToRemove);
                            }

                            var serviceLocationsToRemove = dc.ServiceLocations.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (serviceLocationsToRemove.Count() > 0)
                            {
                                dc.ServiceLocations.RemoveRange(serviceLocationsToRemove);
                            }

                            var serviceAreasToRemove = dc.ServiceServiceAreas.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (serviceAreasToRemove.Count() > 0)
                            {
                                dc.ServiceServiceAreas.RemoveRange(serviceAreasToRemove);
                            }

                            var categoriesToRemove = dc.ServiceCategories.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (categoriesToRemove.Count() > 0)
                            {
                                dc.ServiceCategories.RemoveRange(categoriesToRemove);
                            }

                            var accessibilitiesToRemove = dc.ServiceAccessibilityFeatures.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (accessibilitiesToRemove.Count() > 0)
                            {
                                dc.ServiceAccessibilityFeatures.RemoveRange(accessibilitiesToRemove);
                            }

                            var communityGroupsToRemove = dc.ServiceCommunityGroups.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (communityGroupsToRemove.Count() > 0)
                            {
                                dc.ServiceCommunityGroups.RemoveRange(communityGroupsToRemove);
                            }

                            var reviewsToRemove = dc.ServiceReviews.Where(s => serviceIdsToRemove.Contains(s.ServiceId));
                            if (reviewsToRemove.Count() > 0)
                            {
                                dc.ServiceReviews.RemoveRange(reviewsToRemove);
                            }

                            var serviceAuditsToRemove = dc.ServiceAudits.Where(s => s.ServiceId.HasValue && serviceIdsToRemove.Contains(s.ServiceId.Value));
                            if (serviceAuditsToRemove.Count() > 0)
                            {
                                dc.ServiceAudits.RemoveRange(serviceAuditsToRemove);
                            }

                            if (servicesToRemove.Count() > 0)
                            {
                                dc.Services.RemoveRange(servicesToRemove);
                            }

                            var claimUsersToRemove = dc.OrganisationClaimUsers.Where(o => o.OrganisationId == organisationId).ToList();
                            if (claimUsersToRemove.Count() > 0)
                            {
                                dc.OrganisationClaimUsers.RemoveRange(claimUsersToRemove);
                            }

                            var claimsToRemove = dc.Claims.Where(o => o.OrganisationId == organisationId).ToList();
                            if (claimsToRemove.Count() > 0)
                            {
                                dc.Claims.RemoveRange(claimsToRemove);
                            }

                            var locationsToRemove = dc.Locations.Where(o => o.OrganisationId == organisationId);
                            if (locationsToRemove.Count() > 0)
                            {
                                dc.Locations.RemoveRange(locationsToRemove);
                            }

                            var organisationAuditsToRemove = dc.OrganisationAudits.Where(o => o.OrganisationId == organisationId);
                            if (organisationAuditsToRemove.Count() > 0)
                            {
                                dc.OrganisationAudits.RemoveRange(organisationAuditsToRemove);
                            }

                            var serviceImprovementsToRemove = dc.Improvements.Where(s => s.ServiceId.HasValue && serviceIdsToRemove.Contains(s.ServiceId.Value));
                            if (serviceImprovementsToRemove.Count() > 0)
                            {
                                foreach(Improvement improvement in serviceImprovementsToRemove)
                                {
                                    improvement.ServiceName = dc.Services.Find(improvement.ServiceId).Name;
                                    improvement.ServiceId = null;
                                }
                            }

                            var organisationImprovementsToRemove = dc.Improvements.Where(o => o.OrganisationId == organisationId);
                            if (organisationImprovementsToRemove.Count() > 0)
                            {
                                foreach (Improvement improvement in organisationImprovementsToRemove)
                                {
                                    improvement.OrganisationName = organisationToDelete.Name;
                                    improvement.OrganisationId = null;
                                }
                            }

                            dc.Organisations.Remove(organisationToDelete);
                            dc.SaveChanges();
                        }
                        else
                        {
                            errorMessage = "Failed to delete the organisation from the index, please try again.";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public List<Guid> GetUserOrganisations(ApplicationUserManager userManager, string username, out bool isAdmin, bool myOrgs = false)
        {
            List<Guid> userOrganisations = new List<Guid>();
            ApplicationUser user = userManager.FindByName(username);
            bool isBaseUser = userManager.IsInRole(user.Id, RolesEnum.BaseUser.ToString());
            bool isClaimedUser = userManager.IsInRole(user.Id, RolesEnum.ClaimedUser.ToString());
            bool isEditor = userManager.IsInRole(user.Id, RolesEnum.Editor.ToString());
            isAdmin = !isBaseUser && !isClaimedUser && !isEditor;

            using (ALISSContext dc = new ALISSContext())
            {
                var userId = dc.UserProfiles.Where(u => u.Username == username).FirstOrDefault().UserProfileId;

                if (isClaimedUser || isBaseUser)
                {
                    List<Guid> userClaimedOrganisations = dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == userId).Select(o => o.OrganisationId.Value).ToList();
                    List<Guid> userCreatedOrganisations = dc.Organisations.Where(c => (c.CreatedUserId == userId && (!c.ClaimedUserId.HasValue || c.ClaimedUserId == userId)) || c.ClaimedUserId == userId).Select(o => o.OrganisationId).ToList();
                    userOrganisations = userCreatedOrganisations.Union(userClaimedOrganisations).ToList();
                }
                else if (isEditor && myOrgs)
                {
                    List<Guid> userClaimedOrganisations = dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == userId).Select(o => o.OrganisationId.Value).ToList();
                    List<Guid> userCreatedOrganisations = dc.Organisations.Where(c => (c.CreatedUserId == userId && (!c.ClaimedUserId.HasValue || c.ClaimedUserId == userId)) || c.ClaimedUserId == userId).Select(o => o.OrganisationId).ToList();
                    List<Guid> userServiceOrganisations = dc.Services.Include(o => o.Organisation).Where(o => (o.CreatedUserId == userId && !o.Organisation.ClaimedUserId.HasValue)).Select(s => s.OrganisationId).ToList();
                    userOrganisations = userCreatedOrganisations.Union(userServiceOrganisations).Union(userClaimedOrganisations).ToList();
                }
                else if (isEditor)
                {
                    // Since users flagged as editors don't ever get the claimed user role, we need to include their claims here too
                    List<Guid> userClaimedOrganisations = dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == userId).Select(o => o.OrganisationId.Value).ToList();
                    List<Guid> unclaimedOrganisations = dc.Organisations.Where(c => !c.ClaimedUserId.HasValue).Select(o => o.OrganisationId).ToList();
                    userOrganisations = unclaimedOrganisations.Union(userClaimedOrganisations).ToList();
                }
            }

            return userOrganisations;
        }

        public Models.Models.OrganisationClaimUser GetOrganisationClaimant(Guid organisationClaimUserId)
        {
            Models.Models.OrganisationClaimUser organisationClaimUser = new Models.Models.OrganisationClaimUser();

            using (ALISSContext dc = new ALISSContext())
            {
                organisationClaimUser = dc.OrganisationClaimUsers.Find(organisationClaimUserId);
                _ = organisationClaimUser.ClaimedUser;
            }

            return organisationClaimUser;
        }

        public OrganisationClaimantListingViewModel ListClaimants(string searchTerm, Guid organisationId, int page = 1)
        {
            OrganisationClaimantListingViewModel claimantList = new OrganisationClaimantListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claimants = new List<OrganisationClaimantPTO>(),
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<Models.Models.OrganisationClaimUser> claimants = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == organisationId).OrderByDescending(x => x.IsLeadClaimant).ThenBy(x => x.ClaimedUser.Name).ToList();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                claimants = string.IsNullOrEmpty(searchTerm)
                    ? claimants
                    : claimants.Where(n => (string.IsNullOrEmpty(n.ClaimedUser.Name) == false && n.ClaimedUser.Name.ToLower().Contains(searchTerm.ToLower())) || (n.ClaimedUser.Email.ToLower().Contains(searchTerm.ToLower()))).ToList();

                claimantList.OrganisationName = dc.Organisations.Find(organisationId).Name;
                claimantList.TotalPages = (int)Math.Ceiling((double)claimants.Count / _ItemsPerPage);
                claimantList.TotalResults = claimants.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claimants = claimants.Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (Models.Models.OrganisationClaimUser claimant in claimants)
                {
                    OrganisationClaimantPTO organisationClaimantListItem = new OrganisationClaimantPTO()
                    {
                        OrganisationClaimUserId = claimant.OrganisationClaimUserId,
                        OrganisationId = claimant.OrganisationId.Value,
                        ClaimantUserId = claimant.ClaimedUserId.Value,
                        ClaimantName = claimant.ClaimedUser.Name,
                        ClaimantEmail = claimant.ClaimedUser.Email,
                        IsLeadClaimant = claimant.IsLeadClaimant,
                        ApprovedOn = claimant.ApprovedOn
                    };

                    claimantList.Claimants.Add(organisationClaimantListItem);
                }
            }

            return claimantList;
        }

        public bool CanUserEditOrganisation(ApplicationUserManager userManager, string username, Guid organisationId)
        {
            List<Guid> userOrganisations = GetUserOrganisations(userManager, username, out bool isAdmin);
            return (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(organisationId) || ValidOrganisationForEditor(userManager, username, organisationId);
        }

        public int GetNumberOfServicesForOrganisation(Guid organisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                int serviceCount = dc.Services.Count(o => o.OrganisationId == organisationId);

                return serviceCount;
            }
        }

        public bool DoesOrganisationExist(string organisationName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Where(n => n.Name.ToLower() == organisationName.ToLower()).FirstOrDefault();

                if (organisation != null)
                {
                    return true;
                }

                return false;
            }
        }

        public bool DoesOrganisationIdExist(Guid newOrganisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(newOrganisationId);

                return organisation != null;
            }
        }

        public bool ValidOrganisationForEditor(ApplicationUserManager userManager, string username, Guid organisationId)
        {
            ApplicationUser user = userManager.FindByName(username);

            if (!userManager.IsInRole(user.Id, RolesEnum.Editor.ToString()))
            {
                return false;
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(organisationId);

                if (organisation.ClaimedUser == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
