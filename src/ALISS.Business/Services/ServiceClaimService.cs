using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Claim;
using ALISS.Business.PresentationTransferObjects.ServiceClaim;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.ServiceClaim;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Tactuum.Core.Extensions;

namespace ALISS.Business.Services
{
    public class ServiceClaimService
    {

        private int _ItemsPerPage = 10;
        private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!/\\-, '']";
        private ElasticSearchService _elasticSearchService;
        private EmailService _emailService;
        private string publicBaseUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public ServiceClaimService()
        {
            _elasticSearchService = new ElasticSearchService();
            _emailService = new EmailService();
        }

        public ServiceClaimListingViewModel ListMyClaims(string searchTerm, int userId, int page = 1)
        {
            ServiceClaimListingViewModel claimList = new ServiceClaimListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claims = new List<ServiceClaimPTO>(),
                TotalPages = 1
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceClaim> claims = dc.ServiceClaims
                    .Include(o => o.Service)
                    .Include(o => o.ClaimedUser)
                    .Where(u => u.ClaimedUserId == userId)
                    .ToList();

                List<Claim> organisationClaims = dc.Claims
                    .Include(o => o.Organisation)
                    .Include(o => o.ClaimedUser)
                    .Where(u => u.ClaimedUserId == userId)
                    .ToList();

                foreach (Claim claim in organisationClaims)
                {
                    List<Service> services = dc.Services.Where(s => s.OrganisationId == claim.OrganisationId).ToList();
                    foreach (Service service in services)
                    {
                        if (!claims.Any(c => c.ServiceId == service.ServiceId))
                        {
                            claims.Add(new ServiceClaim
                            {
                                ClaimId = Guid.Empty,
                                ServiceId = service.ServiceId,
                                RepresentativeRole = claim.RepresentativeRole,
                                RepresentativeName = claim.RepresentativeName,
                                RepresentativePhone = claim.RepresentativePhone,
                                RequestLeadClaimant = claim.RequestLeadClaimant,
                                CreatedOn = claim.CreatedOn,
                                Status = claim.Status,
                                ClaimedUserId = claim.ClaimedUserId,
                                ReviewedByUserId = claim.ReviewedByUserId,
                                ReviewedOn = claim.ReviewedOn,
                                Service = service,
                                ClaimedUser = claim.ClaimedUser,
                                ReviewedUser = claim.ReviewedUser
                            });
                        }
                    }
                }

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty).ToLower();

                claims = String.IsNullOrEmpty(searchTerm)
                    ? claims
                    : claims.Where(n => (!String.IsNullOrEmpty(n.ClaimedUser.Name) && Regex.Replace(n.ClaimedUser.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.ClaimedUser.Email) && Regex.Replace(n.ClaimedUser.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Service.Name) && Regex.Replace(n.Service.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Service.Email) && Regex.Replace(n.Service.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        ).ToList();

                claimList.TotalPages = (int)Math.Ceiling((double)claims.Count / _ItemsPerPage);
                claimList.TotalResults = claims.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claims = claims.OrderBy(s => s.Status).ThenByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                foreach (ServiceClaim claim in claims)
                {
                    ServiceClaimPTO claimListItem = new ServiceClaimPTO()
                    {
                        ClaimId = claim.ClaimId,
                        ClaimedUserName = String.IsNullOrEmpty(claim.ClaimedUser.Name) ? claim.ClaimedUser.Email : claim.ClaimedUser.Name,
                        ServiceName = claim.Service.Name,
                        Status = ((ClaimStatusEnum)claim.Status).GetDisplayName(),
                        CreatedOn = claim.CreatedOn,
                        ServiceId = claim.ServiceId.Value,
                        ClaimedUserId = claim.ClaimedUserId.Value,
                        ServicePublished = claim.Service.Published
                    };

                    claimList.Claims.Add(claimListItem);
                }
                claimList.Claims = claimList.Claims.ToList();
            }

            return claimList;
        }

        public ServiceClaimListingViewModel ListClaims(string searchTerm, string username, ApplicationUserManager userManager, int page = 1)
        {
            ApplicationUser systemUser = userManager.FindByName(username);
            bool isAdmin = userManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());

            ServiceClaimListingViewModel claimList = new ServiceClaimListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claims = new List<ServiceClaimPTO>(),
                TotalPages = 1
            };

            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.FirstOrDefault(u => u.Username == username);
                List<Guid> myOrganisations = new List<Guid>();
                List<Guid> myServices = new List<Guid>();
                List<ServiceClaim> claims = new List<ServiceClaim>();

                if (isAdmin)
                {
                    claims = dc.ServiceClaims.Include(o => o.Service).Include(o => o.ClaimedUser).ToList();
                }
                else
                {
                    myOrganisations = dc.OrganisationClaimUsers
                        .Where(x => x.ClaimedUserId == userProfile.UserProfileId && x.IsLeadClaimant)
                        .Select(x => x.OrganisationId.Value).ToList();
                    myServices = dc.Services.Where(x => myOrganisations.Contains(x.OrganisationId)).Select(x => x.ServiceId).ToList();
                    myServices.AddRange(dc.ServiceClaimUsers.Where(x => x.ClaimedUserId == userProfile.UserProfileId && x.IsLeadClaimant).Select(x => x.ServiceId.Value));
                    claims = dc.ServiceClaims
                        .Include(o => o.Service)
                        .Include(o => o.ClaimedUser)
                        .Where(o => myServices.Contains(o.ServiceId.Value) && o.ClaimedUserId != userProfile.UserProfileId)
                        .ToList();
                }

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty).ToLower();

                claims = String.IsNullOrEmpty(searchTerm)
                    ? claims
                    : claims.Where(n => (!String.IsNullOrEmpty(n.ClaimedUser.Name) && Regex.Replace(n.ClaimedUser.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.ClaimedUser.Email) && Regex.Replace(n.ClaimedUser.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Service.Name) && Regex.Replace(n.Service.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Service.Email) && Regex.Replace(n.Service.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        ).ToList();

                claimList.TotalPages = (int)Math.Ceiling((double)claims.Count / _ItemsPerPage);
                claimList.TotalResults = claims.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claims = claims.OrderBy(s => s.Status).ThenByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                foreach (ServiceClaim claim in claims)
                {
                    ServiceClaimPTO claimListItem = new ServiceClaimPTO()
                    {
                        ClaimId = claim.ClaimId,
                        ClaimedUserName = String.IsNullOrEmpty(claim.ClaimedUser.Name) ? claim.ClaimedUser.Email : claim.ClaimedUser.Name,
                        ServiceName = claim.Service.Name,
                        Status = ((ClaimStatusEnum)claim.Status).GetDisplayName(),
                        CreatedOn = claim.CreatedOn,
                        ServiceId = claim.ServiceId.Value,
                        ClaimedUserId = claim.ClaimedUserId.Value,
                        ServicePublished = claim.Service.Published,
                        ExistingClaimedUserId = claim.Service.ClaimedUserId,
                        ExistingClaimedUserName = claim.Service.ClaimedUser == null ? "" : string.IsNullOrEmpty(claim.Service.ClaimedUser?.Name) ? claim.Service.ClaimedUser.Username : claim.Service.ClaimedUser.Name
                    };

                    claimList.Claims.Add(claimListItem);
                }
                claimList.Claims = claimList.Claims.ToList();
            }

            return claimList;
        }

        public static void AddClaim(AddClaimViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claimToAdd = new ServiceClaim()
                {
                    ClaimId = Guid.NewGuid(),
                    ServiceId = model.Id.Value,
                    RepresentativeRole = model.RepresentativeRole,
                    RepresentativeName = model.RepresentativeName,
                    RepresentativePhone = model.RepresentativePhone,
                    RequestLeadClaimant = model.RequestLeadClaimant,
                    ClaimedUserId = model.ClaimedUserId,
                    Status = (int)ClaimStatusEnum.Unreviewed,
                    CreatedOn = DateTime.UtcNow.Date
                };

                dc.ServiceClaims.Add(claimToAdd);
                dc.SaveChanges();
            }
        }

        public EditServiceClaimViewModel GetClaimForEdit(Guid ClaimId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var claimForEdit = dc.ServiceClaims.Find(ClaimId);

                return new EditServiceClaimViewModel()
                {
                    ServiceName = claimForEdit.Service.Name,
                    UserName = claimForEdit.ClaimedUser.Name,
                    Email = claimForEdit.ClaimedUser.Email,
                    RepresentativeName = claimForEdit.RepresentativeName,
                    RepresentativePhone = claimForEdit.RepresentativePhone,
                    RequestLeadClaimant = claimForEdit.RequestLeadClaimant || claimForEdit.Service.ClaimedUserId == null && claimForEdit.Service.Organisation.ClaimedUserId == null,
                    RepresentativeRole = claimForEdit.RepresentativeRole,
                    Status = (ClaimStatusEnum)Convert.ToInt32(claimForEdit.Status),
                    ClaimId = claimForEdit.ClaimId
                };
            }
        }

        public string ApproveClaim(Guid claimId, int userProfileId, bool setAsLead, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claimToEdit = dc.ServiceClaims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Approved);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;

                UserProfile userProfile = dc.UserProfiles.Find(claimToEdit.ClaimedUserId);
                var appUser = userManager.FindByName(userProfile.Username);
                if (userManager.IsInRole(appUser.Id, RolesEnum.BaseUser.ToString()))
                {
                    userManager.RemoveFromRole(appUser.Id, RolesEnum.BaseUser.ToString());
                    userManager.AddToRole(appUser.Id, RolesEnum.ClaimedUser.ToString());
                }

                Service service = dc.Services.Find(claimToEdit.ServiceId.Value);
                if ((!service.ClaimedUserId.HasValue && ! service.Organisation.ClaimedUserId.HasValue) || setAsLead)
                {
                    service.ClaimedUserId = claimToEdit.ClaimedUserId;
                    service.ClaimedOn = DateTime.UtcNow.Date;
                    setAsLead = true;
                }

                ServiceClaimUser serviceClaimUser = dc.ServiceClaimUsers.Where(
                    x => x.ServiceId == claimToEdit.ServiceId &&
                    x.ClaimedUserId == claimToEdit.ClaimedUserId).FirstOrDefault();
                if (serviceClaimUser != null)
                {
                    if (setAsLead)
                    {
                        ServiceClaimUser existingLeadClaimUser = dc.ServiceClaimUsers.Where(
                            x => x.ServiceId == claimToEdit.ServiceId.Value &&
                            x.IsLeadClaimant == true).FirstOrDefault();
                        existingLeadClaimUser.IsLeadClaimant = false;
                    }
                    serviceClaimUser.IsLeadClaimant = setAsLead;
                    serviceClaimUser.ApprovedOn = DateTime.UtcNow.Date;
                }
                else
                {
                    if (setAsLead)
                    {
                        ServiceClaimUser existingLeadClaimUser = dc.ServiceClaimUsers.Where(
                            x => x.ServiceId == claimToEdit.ServiceId &&
                            x.IsLeadClaimant == true).FirstOrDefault();
                        if (existingLeadClaimUser != null)
                        {
                            existingLeadClaimUser.IsLeadClaimant = false;
                        }
                    }
                    serviceClaimUser = new ServiceClaimUser()
                    {
                        ServiceClaimUserId = Guid.NewGuid(),
                        ServiceId = claimToEdit.ServiceId.Value,
                        ClaimedUserId = claimToEdit.ClaimedUserId,
                        ServiceClaimId = claimToEdit.ClaimId,
                        IsLeadClaimant = setAsLead,
                        ApprovedOn = DateTime.UtcNow.Date
                    };
                    dc.ServiceClaimUsers.Add(serviceClaimUser);
                }

                dc.SaveChanges();

                if (service.Organisation.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(claimToEdit.ServiceId.Value);
                    _elasticSearchService.AddOrganisationToElasticSearch(claimToEdit.Service.OrganisationId);
                }

                _emailService.SendClaimApprovedEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.Name, $"{publicBaseUrl}/services/{service.Slug}", service.ClaimedUser == null || setAsLead);

                return "Claim approved";
            }
        }

        public string RevokeClaim(Guid claimId, int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claimToEdit = dc.ServiceClaims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Revoked);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;
                UserProfile userProfile = dc.UserProfiles.Find(claimToEdit.ClaimedUserId);
                var appUser = userManager.FindByName(userProfile.Username);
                if (userManager.IsInRole(appUser.Id, RolesEnum.ClaimedUser.ToString()) && dc.Claims.Count(u => u.ClaimedUserId == claimToEdit.ClaimedUserId && u.Status == 10) == 1)
                {
                    userManager.RemoveFromRole(appUser.Id, RolesEnum.ClaimedUser.ToString());
                    userManager.AddToRole(appUser.Id, RolesEnum.BaseUser.ToString());
                }

                Service service = dc.Services.Find(claimToEdit.ServiceId);
                if (service.ClaimedUserId == claimToEdit.ClaimedUserId)
                {
                    service.ClaimedUserId = null;
                    service.ClaimedOn = null;
                }

                ServiceClaimUser serviceClaimUser = dc.ServiceClaimUsers.Find(claimToEdit.ClaimId);
                if (serviceClaimUser != null)
                {
                    dc.ServiceClaimUsers.Remove(serviceClaimUser);
                }

                dc.SaveChanges();

                if (service.Organisation.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(claimToEdit.ServiceId.Value);
                    _elasticSearchService.AddOrganisationToElasticSearch(claimToEdit.Service.OrganisationId);
                }

                return "Claim revoked";
            }
        }

        public string UnreviewedClaim(Guid claimId, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claimToEdit = dc.ServiceClaims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Unreviewed);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;

                dc.SaveChanges();

                return "Claim revoked";
            }
        }

        public string DenyClaim(Guid claimId, int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claimToEdit = dc.ServiceClaims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Denied);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;

                // There shouldnt be a ServiceClaimUser record, but better safe.
                ServiceClaimUser serviceClaimUser = dc.ServiceClaimUsers.Find(claimToEdit.ClaimId);
                if (serviceClaimUser != null)
                {
                    dc.ServiceClaimUsers.Remove(serviceClaimUser);
                }

                Service service = dc.Services.Find(claimToEdit.ServiceId);
                UserProfile userProfile = dc.UserProfiles.Find(claimToEdit.ClaimedUserId);

                _emailService.SendClaimDeniedEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.Name, service.ClaimedUser == null || claimToEdit.RequestLeadClaimant);

                dc.SaveChanges();

                return "Claim denied";
            }
        }
    }
}


