using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Claim;
using ALISS.Business.PresentationTransferObjects.ServiceClaim;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.ServiceClaim;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Tactuum.Core.Extensions;

namespace ALISS.Business.Services
{
    public class ClaimService
    {

        private int _ItemsPerPage = 10;
        private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!/\\-, '']";
        private ElasticSearchService _elasticSearchService;
        private EmailService _emailService;
        private string publicBaseUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public ClaimService()
        {
            _elasticSearchService = new ElasticSearchService();
            _emailService = new EmailService();
        }

        public ClaimListingViewModel ListMyClaims(string searchTerm, int userId, int page = 1)
        {
            ClaimListingViewModel claimList = new ClaimListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claims = new List<ClaimPTO>(),
                TotalPages = 1
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<Claim> claims = dc.Claims.Include(o => o.Organisation).Include(o => o.ClaimedUser).Where(u => u.ClaimedUserId == userId).ToList();

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty).ToLower();

                claims = String.IsNullOrEmpty(searchTerm)
                    ? claims
                    : claims.Where(n => (!String.IsNullOrEmpty(n.ClaimedUser.Name) && Regex.Replace(n.ClaimedUser.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.ClaimedUser.Email) && Regex.Replace(n.ClaimedUser.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Organisation.Name) && Regex.Replace(n.Organisation.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Organisation.Email) && Regex.Replace(n.Organisation.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        ).ToList();

                claimList.TotalPages = (int)Math.Ceiling((double)claims.Count / _ItemsPerPage);
                claimList.TotalResults = claims.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claims = claims.OrderBy(s => s.Status).ThenByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                foreach (Claim claim in claims)
                {
                    ClaimPTO claimListItem = new ClaimPTO()
                    {
                        ClaimId = claim.ClaimId,
                        ClaimedUserName = String.IsNullOrEmpty(claim.ClaimedUser.Name) ? claim.ClaimedUser.Email : claim.ClaimedUser.Name,
                        OrganisationName = claim.Organisation.Name,
                        Status = ((ClaimStatusEnum)claim.Status).GetDisplayName(),
                        CreatedOn = claim.CreatedOn,
                        OrganisationId = claim.OrganisationId,
                        ClaimedUserId = claim.ClaimedUserId,
                        OrganisationPublished = claim.Organisation.Published
                    };

                    claimList.Claims.Add(claimListItem);
                }
                claimList.Claims = claimList.Claims.ToList();
            }

            return claimList;
        }

        public ClaimListingViewModel ListClaims(string searchTerm, string username, ApplicationUserManager userManager, int page = 1)
        {
            ApplicationUser systemUser = userManager.FindByName(username);
            bool isAdmin = userManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());

            ClaimListingViewModel claimList = new ClaimListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claims = new List<ClaimPTO>(),
                TotalPages = 1
            };

            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.FirstOrDefault(u => u.Username == username);
                List<Guid> myOrganisations = new List<Guid>();
                List<Claim> claims = new List<Claim>();

                if (isAdmin)
                {
                    claims = dc.Claims.Include(o => o.Organisation).Include(o => o.ClaimedUser).Where(s => s.Organisation.Submitted).ToList();
                }
                else
                {
                    myOrganisations = dc.OrganisationClaimUsers
                        .Where(x => x.ClaimedUserId == userProfile.UserProfileId && x.IsLeadClaimant)
                        .Select(x => x.OrganisationId.Value).ToList();
                    claims = dc.Claims
                        .Include(o => o.Organisation)
                        .Include(o => o.ClaimedUser)
                        .Where(o => myOrganisations.Contains(o.OrganisationId) && o.ClaimedUserId != userProfile.UserProfileId)
                        .ToList();
                }

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty).ToLower();

                claims = String.IsNullOrEmpty(searchTerm)
                    ? claims
                    : claims.Where(n => (!String.IsNullOrEmpty(n.ClaimedUser.Name) && Regex.Replace(n.ClaimedUser.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.ClaimedUser.Email) && Regex.Replace(n.ClaimedUser.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Organisation.Name) && Regex.Replace(n.Organisation.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        || (!String.IsNullOrEmpty(n.Organisation.Email) && Regex.Replace(n.Organisation.Email.ToLower(), regexMatch, string.Empty).Contains(searchTerm))
                        ).ToList();
                
                claimList.TotalPages = (int)Math.Ceiling((double)claims.Count / _ItemsPerPage);
                claimList.TotalResults = claims.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claims = claims.OrderBy(s => s.Status).ThenByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                foreach (Claim claim in claims)
                {
                    ClaimPTO claimListItem = new ClaimPTO()
                    {
                        ClaimId = claim.ClaimId,
                        ClaimedUserName = String.IsNullOrEmpty(claim.ClaimedUser.Name) ? claim.ClaimedUser.Email : claim.ClaimedUser.Name,
                        OrganisationName = claim.Organisation.Name,
                        Status = ((ClaimStatusEnum)claim.Status).GetDisplayName(),
                        CreatedOn = claim.CreatedOn,
                        OrganisationId = claim.OrganisationId,
                        ClaimedUserId = claim.ClaimedUserId,
                        OrganisationPublished = claim.Organisation.Published,
                        ExistingClaimedUserId = claim.Organisation.ClaimedUserId,
                        ExistingClaimedUserName = claim.Organisation.ClaimedUser == null ? "" : string.IsNullOrEmpty(claim.Organisation.ClaimedUser?.Name) ? claim.Organisation.ClaimedUser.Username : claim.Organisation.ClaimedUser.Name,
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
                Claim claimToAdd = new Claim()
                {
                    ClaimId = Guid.NewGuid(),
                    OrganisationId = model.Id.Value,
                    RepresentativeRole = model.RepresentativeRole,
                    RepresentativeName = model.RepresentativeName,
                    RepresentativePhone = model.RepresentativePhone,
                    RequestLeadClaimant = model.RequestLeadClaimant,
                    ClaimedUserId = model.ClaimedUserId,
                    Status = (int)ClaimStatusEnum.Unreviewed,
                    CreatedOn = DateTime.UtcNow.Date
                };

                dc.Claims.Add(claimToAdd);
                dc.SaveChanges();
            }
        }

        public EditClaimViewModel GetClaimForEdit(Guid ClaimId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var claimForEdit = dc.Claims.Find(ClaimId);

                return new EditClaimViewModel()
                {
                    OrganisationName = claimForEdit.Organisation.Name,
                    UserName = claimForEdit.ClaimedUser.Name,
                    Email = claimForEdit.ClaimedUser.Email,
                    RepresentativeName = claimForEdit.RepresentativeName,
                    RepresentativePhone = claimForEdit.RepresentativePhone,
                    RequestLeadClaimant = claimForEdit.RequestLeadClaimant || claimForEdit.Organisation.ClaimedUserId == null,
                    RepresentativeRole = claimForEdit.RepresentativeRole,
                    Status = (ClaimStatusEnum)Convert.ToInt32(claimForEdit.Status),
                    ClaimId = claimForEdit.ClaimId
                };
            }
        }

        public string ApproveClaims(Guid claimId, int userProfileId, bool setAsLead, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Claim claimToEdit = dc.Claims.Find(claimId);

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

                Organisation organisation = dc.Organisations.Find(claimToEdit.OrganisationId);
                if (!organisation.ClaimedUserId.HasValue || setAsLead)
                {
                    organisation.ClaimedUserId = claimToEdit.ClaimedUserId;
                    organisation.ClaimedOn = DateTime.UtcNow.Date;
                    setAsLead = true;
                }

                OrganisationClaimUser organisationClaimUser = dc.OrganisationClaimUsers.Where(
                    x => x.OrganisationId == claimToEdit.OrganisationId &&
                    x.ClaimedUserId == claimToEdit.ClaimedUserId).FirstOrDefault();
                if (organisationClaimUser != null)
                {
                    if (setAsLead)
                    {
                        OrganisationClaimUser existingLeadClaimUser = dc.OrganisationClaimUsers.Where(
                            x => x.OrganisationId == claimToEdit.OrganisationId &&
                            x.IsLeadClaimant == true).FirstOrDefault();
                        existingLeadClaimUser.IsLeadClaimant = false;
                    }
                    organisationClaimUser.IsLeadClaimant = setAsLead;
                    organisationClaimUser.ApprovedOn = DateTime.UtcNow.Date;
                }
                else
                {
                    if (setAsLead)
                    {
                        OrganisationClaimUser existingLeadClaimUser = dc.OrganisationClaimUsers.Where(
                            x => x.OrganisationId == claimToEdit.OrganisationId &&
                            x.IsLeadClaimant == true).FirstOrDefault();
                        if (existingLeadClaimUser != null)
                        {
                            existingLeadClaimUser.IsLeadClaimant = false;
                        }
                    }
                    organisationClaimUser = new OrganisationClaimUser()
                    {
                        OrganisationClaimUserId = Guid.NewGuid(),
                        OrganisationId = claimToEdit.OrganisationId,
                        ClaimedUserId = claimToEdit.ClaimedUserId,
                        ClaimId = claimToEdit.ClaimId,
                        IsLeadClaimant = setAsLead,
                        ApprovedOn = DateTime.UtcNow.Date
                    };
                    dc.OrganisationClaimUsers.Add(organisationClaimUser);
                }

                dc.SaveChanges();

                if (organisation.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(claimToEdit.OrganisationId);
                    List<Guid> organisationServiceIds = dc.Services.Where(o => o.OrganisationId == organisation.OrganisationId).Select(s => s.ServiceId).ToList();
                    foreach (Guid id in organisationServiceIds)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(id);
                    }
                }

                _emailService.SendClaimApprovedEmail(userProfile.Email, userProfile.Name, userProfile.Username, organisation.Name, $"{publicBaseUrl}/organisations/{organisation.Slug}", organisation.ClaimedUser == null || setAsLead);

                return "Claim approved";
            }
        }

        public string RevokeClaims(Guid claimId, int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Claim claimToEdit = dc.Claims.Find(claimId);

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

                Organisation organisation = dc.Organisations.Find(claimToEdit.OrganisationId);
                if (organisation.ClaimedUserId == claimToEdit.ClaimedUserId)
                {
                    organisation.ClaimedUserId = null;
                    organisation.ClaimedOn = null;
                }

                OrganisationClaimUser organisationClaimUser = dc.OrganisationClaimUsers.Find(claimToEdit.ClaimId);
                if (organisationClaimUser != null)
                {
                    dc.OrganisationClaimUsers.Remove(organisationClaimUser);
                }

                dc.SaveChanges();

                if (organisation.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(claimToEdit.OrganisationId);
                    List<Guid> organisationServiceIds = dc.Services.Where(o => o.OrganisationId == organisation.OrganisationId).Select(s => s.ServiceId).ToList();
                    foreach (Guid id in organisationServiceIds)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(id);
                    }
                }

                return "Claim revoked";
            }
        }

        public string Unreviewed(Guid claimId, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Claim claimToEdit = dc.Claims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Unreviewed);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;


                dc.SaveChanges();

                return "Claim revoked";
            }
        }

        public string DenyClaims(Guid claimId, int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Claim claimToEdit = dc.Claims.Find(claimId);

                claimToEdit.Status = Convert.ToInt32(ClaimStatusEnum.Denied);
                claimToEdit.ReviewedOn = DateTime.UtcNow.Date;
                claimToEdit.ReviewedByUserId = userProfileId;

                // There shouldnt be a OrganisationClaimUser record, but better safe.
                OrganisationClaimUser organisationClaimUser = dc.OrganisationClaimUsers.Find(claimToEdit.ClaimId);
                if (organisationClaimUser != null)
                {
                    dc.OrganisationClaimUsers.Remove(organisationClaimUser);
                }

                Organisation organisation = dc.Organisations.Find(claimToEdit.OrganisationId);
                UserProfile userProfile = dc.UserProfiles.Find(claimToEdit.ClaimedUserId);

                _emailService.SendClaimDeniedEmail(userProfile.Email, userProfile.Name, userProfile.Username, organisation.Name, organisation.ClaimedUser == null || claimToEdit.RequestLeadClaimant);

                dc.SaveChanges();

                return "Claim denied";
            }
        }

        public List<UserProfile> GetManagers(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                return dc.ServiceClaimUsers.Where(c => c.ServiceId == serviceId && !c.IsLeadClaimant && c.ClaimedUserId.HasValue).Select(c => c.ClaimedUser).ToList();
            }
        }
    }
}


