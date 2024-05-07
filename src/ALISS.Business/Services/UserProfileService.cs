using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Organisation;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.PresentationTransferObjects.User;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebSockets;
using Tactuum.Core.Extensions;

namespace ALISS.Business.Services
{
    public class UserProfileService
    {

        private int _ItemsPerPage = 10;
        private OrganisationService _organisationService = new OrganisationService();
        //private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!]";
        public UserListingViewModel ListUsers(string searchTerm, ApplicationUserManager userManager, int page = 1)
        {
            UserListingViewModel userList = new UserListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Users = new List<UserPTO>(),
                TotalPages = 1
            };

            using (ALISSContext dc = new ALISSContext())
            {

                List<UserProfile> users = dc.UserProfiles.Where(a => a.Active).OrderBy(n => n.Name).ToList();
                //searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                users = String.IsNullOrEmpty(searchTerm) ? users : users.Where(n => (String.IsNullOrEmpty(n.Name) == false && n.Name.ToLower().Contains(searchTerm.ToLower())) || (String.IsNullOrEmpty(n.Email) == false && n.Email.ToLower().Contains(searchTerm.ToLower())) || (String.IsNullOrEmpty(n.Email) == false && n.Email.ToLower().Contains(searchTerm.ToLower()))).ToList();

                userList.TotalPages = (int)Math.Ceiling((double)users.Count / _ItemsPerPage);
                userList.TotalResults = users.Count;
                int skip = (page - 1) * _ItemsPerPage;

                users = users.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                foreach (UserProfile user in users)
                {
                    UserPTO UserListItem = new UserPTO()
                    {
                        UserProfileId = user.UserProfileId,
                        Name = user.Name,
                        Email = user.Email,
                        DateJoined = user.DateJoined,
                        PhoneNumber = user.PhoneNumber,
                        OrganisationCount = dc.OrganisationAudits.Where(c => c.UserProfileId == user.UserProfileId).GroupBy(o => o.OrganisationId).Count(),
                        ServiceCount = dc.ServiceAudits.Where(c => c.UserProfileId == user.UserProfileId).GroupBy(o => o.ServiceId).Count(),
                        ClaimCount = dc.Organisations.Count(c => c.ClaimedUserId == user.UserProfileId),
                    };
                    UserProfile userProfile = dc.UserProfiles.Find(user.UserProfileId);
                    var appUser = userManager.FindByName(userProfile.Username);
                    if (appUser != null)
                    {
                        if (userManager.IsInRole(appUser.Id, RolesEnum.BaseUser.ToString()))
                        {
                            UserListItem.Role = RolesEnum.BaseUser.GetDisplayName();
                        }
                        else if (userManager.IsInRole(appUser.Id, RolesEnum.ClaimedUser.ToString()))
                        {
                            UserListItem.Role = RolesEnum.ClaimedUser.GetDisplayName();
                        }
                        else if (userManager.IsInRole(appUser.Id, RolesEnum.SuperAdmin.ToString()))
                        {
                            UserListItem.Role = RolesEnum.SuperAdmin.GetDisplayName();
                        }
                        if (userManager.IsInRole(appUser.Id, RolesEnum.Editor.ToString()))
                        {
                            UserListItem.IsEditor = true;
                        }
                        if (userManager.IsInRole(appUser.Id, RolesEnum.ALISSAdmin.ToString()))
                        {
                            UserListItem.IsAdmin = true;
                        }
                    }
                    else
                    {
                        UserListItem.Role = RolesEnum.BaseUser.GetDisplayName();
                        UserListItem.IsEditor = false;
                        UserListItem.IsAdmin = false;
                    }
                    userList.Users.Add(UserListItem);
                }
            }

            return userList;
        }

        //GET: User
        public EditUserViewModel GetUserForEdit(int userProfileId, ApplicationUserManager userManager, int organisationPage = 1, int servicePage = 1)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userToEdit = dc.UserProfiles.Find(userProfileId);
                ApplicationUser applicationUser = userManager.FindByName(userToEdit.Username);
                OrganisationListingViewModel userOrganisations = GetUserOrganisationsList(userProfileId, organisationPage);
                ServiceListingViewModel userServices = GetUserServicesList(userProfileId, servicePage);

                return new EditUserViewModel()
                {
                    UserProfileId = userToEdit.UserProfileId,
                    Username = userToEdit.Username,
                    Name = userToEdit.Name,
                    Email = userToEdit.Email,
                    PhoneNumber = userToEdit.PhoneNumber,
                    IsAdmin = userManager.IsInRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString()),
                    IsEditor = userManager.IsInRole(applicationUser.Id, RolesEnum.Editor.ToString()),
                    UserOrganisations = userOrganisations,
                    UserServices = userServices
                };
            }
        }

        //POST: User
        public string EditUser(EditUserViewModel model, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userToEdit = dc.UserProfiles.Find(model.UserProfileId);

                if ((model.Username != userToEdit.Username) && IsUsernameUsed(model.Username))
                {
                    return $"Error: The username {model.Username} is already in use, please choose another username.";
                }

                if ((model.Email != userToEdit.Email) && DoesUserExist(model.Email))
                {
                    return $"Error: The email {model.Email} is already in use, please supply another email address.";
                }

                ApplicationUser applicationUser = userManager.FindByName(userToEdit.Username);

                userToEdit.Username = model.Username;
                userToEdit.Name = model.Name;
                userToEdit.Email = model.Email;
                userToEdit.PhoneNumber = model.PhoneNumber;

                dc.SaveChanges();
                if (userManager.IsInRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString()) && !model.IsAdmin)
                {
                    userManager.RemoveFromRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString());
                    if (!userManager.IsInRole(applicationUser.Id, RolesEnum.Editor.ToString()) && !userManager.IsInRole(applicationUser.Id, RolesEnum.ClaimedUser.ToString()))
                    {
                        userManager.AddToRole(applicationUser.Id, RolesEnum.BaseUser.ToString());
                    }
                }
                else if (!userManager.IsInRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString()) && model.IsAdmin)
                {
                    userManager.AddToRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString());
                    {
                        userManager.RemoveFromRole(applicationUser.Id, RolesEnum.BaseUser.ToString());
                    }
                }

                if (userManager.IsInRole(applicationUser.Id, RolesEnum.Editor.ToString()) && !model.IsEditor)
                {
                    userManager.RemoveFromRole(applicationUser.Id, RolesEnum.Editor.ToString());
                    if (!userManager.IsInRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString()) && !userManager.IsInRole(applicationUser.Id, RolesEnum.ClaimedUser.ToString()))
                    {
                        userManager.AddToRole(applicationUser.Id, RolesEnum.BaseUser.ToString());
                    }
                }
                else if (!userManager.IsInRole(applicationUser.Id, RolesEnum.Editor.ToString()) && model.IsEditor)
                {
                    userManager.AddToRole(applicationUser.Id, RolesEnum.Editor.ToString());
                    if (userManager.IsInRole(applicationUser.Id, RolesEnum.BaseUser.ToString()))
                    {
                        userManager.RemoveFromRole(applicationUser.Id, RolesEnum.BaseUser.ToString());
                    }
                }

                if (applicationUser.Email != model.Email || applicationUser.UserName != model.Username)
                {
                    applicationUser.Email = model.Email;
                    applicationUser.UserName = model.Username;
                    userManager.Update(applicationUser);
                }

                return "User edited successfully";
            }
        }

        public DeleteUserViewModel GetUserToDelete(int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                List<Organisation> userClaimedOrganisations = dc.Organisations.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<Service> userClaimedServices = dc.Services.Where(c => c.ClaimedUserId == userProfileId).ToList();

                List<OrganisationClaimUser> managersOfUsersOrganisations = new List<OrganisationClaimUser>();
                foreach (Organisation organisation in userClaimedOrganisations)
                {
                    managersOfUsersOrganisations.AddRange(dc.OrganisationClaimUsers.Where(ou => ou.OrganisationId == organisation.OrganisationId && !ou.IsLeadClaimant).ToList());
                }

                List<ServiceClaimUser> managersOfUsersServices = new List<ServiceClaimUser>();
                foreach (Service service in userClaimedServices)
                {
                    managersOfUsersServices.AddRange(dc.ServiceClaimUsers.Where(ou => ou.ServiceId == service.ServiceId && !ou.IsLeadClaimant).ToList());
                }

                DeleteUserViewModel model = new DeleteUserViewModel()
                {
                    UserProfileId = userProfileId,
                    Name = String.IsNullOrEmpty(userProfile.Name) ? userProfile.Email : userProfile.Name,
                    IsDeleteValid = managersOfUsersOrganisations.Count == 0 && managersOfUsersServices.Count == 0,
                };

                return model;
            }
        }

        public void DeleteUser(int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                ApplicationUser userToDelete = userManager.FindByName(userProfile.Username);
                List<Service> userCreatedServices = dc.Services.Where(c => c.CreatedUserId == userProfileId).ToList();
                List<Service> userEditedServices = dc.Services.Where(c => c.UpdatedUserId == userProfileId).ToList();
                List<Organisation> userCreatedOrganisations = dc.Organisations.Where(c => c.CreatedUserId == userProfileId).ToList();
                List<Organisation> userEditedOrganisations = dc.Organisations.Where(c => c.UpdatedUserId == userProfileId).ToList();
                List<Organisation> userClaimedOrganisations = dc.Organisations.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<ServiceAudit> userServiceAudits = dc.ServiceAudits.Where(s => s.UserProfileId == userProfileId).ToList();
                List<OrganisationAudit> userOrganisationAudits = dc.OrganisationAudits.Where(s => s.UserProfileId == userProfileId).ToList();
                List<Claim> userOrganisationsClaims = dc.Claims.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<Claim> userOrganisationsClaimReviews = dc.Claims.Where(c => c.ReviewedByUserId == userProfileId).ToList();
                List<ServiceClaim> userServiceClaims = dc.ServiceClaims.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<ServiceClaim> userServiceClaimReviews = dc.ServiceClaims.Where(c => c.ReviewedByUserId == userProfileId).ToList();
                List<OrganisationClaimUser> userOrganisationsClaimUser = dc.OrganisationClaimUsers.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<ServiceClaimUser> userServicesClaimUser = dc.ServiceClaimUsers.Where(c => c.ClaimedUserId == userProfileId).ToList();
                List<Collection> userCollections = dc.Collections.Where(u => u.UserProfileId == userProfileId).ToList();
                List<Guid> userCollectionIds = userCollections.Select(c => c.CollectionId).ToList();
                List<Models.Models.CollectionService> userCollectionServices = dc.CollectionServices.Where(c => userCollectionIds.Contains(c.CollectionId)).ToList();
                List<MediaGallery> userMediaGallery = dc.MediaGallery.Where(c => c.UploadUserId == userProfileId).ToList();

                userCreatedServices.ForEach(s => s.CreatedUserId = 1);
                userEditedServices.ForEach(s => s.UpdatedUserId = 1);
                userCreatedOrganisations.ForEach(s => s.CreatedUserId = 1);
                userEditedOrganisations.ForEach(s => s.UpdatedUserId = 1);
                userClaimedOrganisations.ForEach(s => s.ClaimedUserId = null);
                userClaimedOrganisations.ForEach(s => s.ClaimedOn = null);
                userServiceAudits.ForEach(s => s.UserProfileId = 1);
                userOrganisationAudits.ForEach(s => s.UserProfileId = 1);
                userOrganisationsClaimReviews.ForEach(s => s.ReviewedByUserId = 1);
                userServiceClaimReviews.ForEach(s => s.ReviewedByUserId = 1);
                dc.Claims.RemoveRange(userOrganisationsClaims);
                dc.ServiceClaims.RemoveRange(userServiceClaims);
                dc.OrganisationClaimUsers.RemoveRange(userOrganisationsClaimUser);
                dc.ServiceClaimUsers.RemoveRange(userServicesClaimUser);
                dc.CollectionServices.RemoveRange(userCollectionServices);
                dc.Collections.RemoveRange(userCollections);
                dc.UserProfiles.Remove(userProfile);

                dc.SaveChanges();
                userManager.Delete(userToDelete);
            }
        }

        private bool IsUsernameUsed(string username)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Where(n => n.Username.ToLower() == username.ToLower()).FirstOrDefault();

                if (userProfile != null)
                {
                    return true;
                }
                return false;
            }
        }

        private bool DoesUserExist(string email)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Where(n => n.Email.ToLower() == email.ToLower()).FirstOrDefault();

                if (userProfile != null)
                {
                    return true;
                }
                return false;
            }
        }

        //GET Register User
        public CurrentUserViewModel GetCurentLoggedInUser(string username)
        {
            var currentUserModel = new CurrentUserViewModel();
            using (ALISSContext dc = new ALISSContext())
            {
                var userProfile = dc.UserProfiles.FirstOrDefault(e => e.Username == username && e.Active);
                if (userProfile != null)
                {
                    currentUserModel.UserProfileId = userProfile.UserProfileId;
                    currentUserModel.Username = userProfile.Username;
                    currentUserModel.Name = userProfile.Name;
                    currentUserModel.Email = userProfile.Email;
                }
            }

            return currentUserModel;
        }

        public bool IsAdmin(int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userToEdit = dc.UserProfiles.Find(userProfileId);
                ApplicationUser applicationUser = userManager.FindByName(userToEdit.Username);
                return userManager.IsInRole(applicationUser.Id, RolesEnum.ALISSAdmin.ToString());
            }
        }

        //Organisations List related to the user
        public OrganisationListingViewModel GetUserOrganisationsList(int userProfileId, int page = 1)
        {
            OrganisationListingViewModel organisationList = new OrganisationListingViewModel()
            {
                Organisations = new List<OrganisationPTO>(),
                Page = page
            };
            using (ALISSContext dc = new ALISSContext())
            {
                
                List<Organisation> organisations = dc.Organisations.Where(c => ((c.ClaimedUser == null && c.CreatedUserId == userProfileId) || c.ClaimedUserId == userProfileId) && c.Submitted).ToList();
                organisationList.TotalPages = (int)Math.Ceiling((double)organisations.Count / _ItemsPerPage);
                organisationList.TotalResults = organisations.Count;
                int skip = (page - 1) * _ItemsPerPage;
                organisations = organisations.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (var organisation in organisations)
                {

                    OrganisationPTO organisationListItem = new OrganisationPTO()
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.Name,
                        CreatedOn = organisation.CreatedOn,
                        UpdatedOn = organisation.UpdatedOn,
                        ClaimedUserId = organisation.ClaimedUserId,
                        ClaimedOn = organisation.ClaimedOn,
                        Published = organisation.Published
                    };
                    organisationList.Organisations.Add(organisationListItem);
                }


            }

            return organisationList;
        }

        //Services List related to the user
        public ServiceListingViewModel GetUserServicesList(int userProfileId, int page = 1)
        {

            ServiceListingViewModel serviceList = new ServiceListingViewModel()
            {
                Services = new List<ServicePTO>(),
                Page = page
            };
            using (ALISSContext dc = new ALISSContext())
            {

                List<Service> services = dc.Services.Where(s => ((s.ClaimedUser == null && s.CreatedUserId == userProfileId) || s.ClaimedUserId == userProfileId) && s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).ToList();
                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                services = services.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (var service in services)
                {
                    ServicePTO serviceListItem = new ServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        CreatedOn = service.CreatedOn,
                        CreatedUserName = service.CreatedUser.Name,
                        CreatedUserId = service.CreatedUserId,
                        Published = service.Published,
                    };
                    serviceList.Services.Add(serviceListItem);
                }
            }
            return serviceList;
        }

        public List<string> GetAllUsersForBulkEmail()
        {
            using (var appDbContext = new ApplicationDbContext())
            {
                return appDbContext.Users.Where(e => e.Email != null && e.Email != string.Empty).Select(e => e.Email).ToList();
            }
        }

        public List<string> GetAllEditorsForBulkEmail()
        {
            using (var appDbContext = new ApplicationDbContext())
            {
                string roleId = appDbContext.Roles.FirstOrDefault(n => n.Name == "Editor").Id;
                return appDbContext.Users.Include(s => s.Roles).Where(u => u.Roles.Any(r => r.RoleId == roleId) && u.Email != null && u.Email != string.Empty).Select(e => e.Email).ToList();
            }
        }

        public List<string> GetAllClaimantsForBulkEmail()
        {
            using (var appDbContext = new ApplicationDbContext())
            {
                string roleId = appDbContext.Roles.FirstOrDefault(n => n.Name == "ClaimedUser").Id;
                return appDbContext.Users.Include(s => s.Roles).Where(u => u.Roles.Any(r => r.RoleId == roleId) && u.Email != null && u.Email != string.Empty).Select(e => e.Email).ToList();
            }
        }
    }
}




