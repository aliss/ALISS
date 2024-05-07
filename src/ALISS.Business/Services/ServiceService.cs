using ALISS.API.Models.Elasticsearch;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Location;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.PresentationTransferObjects.ServiceReviews;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Location;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.ServiceReviews;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using ALISS.Business.Enums;

namespace ALISS.Business.Services
{
    public class ServiceService
    {
        private int _ItemsPerPage = 10;
        private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!/\\-, '']";
        private CategoryService _categoryService = new CategoryService();
        private LocationService _locationService = new LocationService();
        private ServiceAreaService _serviceAreaService = new ServiceAreaService();
        private ElasticSearchService _elasticSearchService;
        private EmailService _emailService;
        private ClaimService _claimService;
        private OrganisationService _organisationService;
        private UserProfileService _userProfileService;
        private string publicBaseUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public ServiceService()
        {
            _elasticSearchService = new ElasticSearchService();
            _emailService = new EmailService();
            _claimService = new ClaimService();
            _organisationService = new OrganisationService();
            _userProfileService = new UserProfileService();
        }

        public ServiceListingViewModel GetAllServices(ApplicationUserManager userManager, string username, string searchTerm, int page, bool unpublished = false, string orderBy = "createdon", int descending = 1)
        {
            ServiceListingViewModel serviceList = new ServiceListingViewModel()
            {
                Services = new List<ServicePTO>(),
                SearchTerm = searchTerm,
                Page = page
            };
            bool isAdmin;
            List<Guid> userServices = GetUserServices(userManager, username, out isAdmin);
            List<Guid> userOrganisations = _organisationService.GetUserOrganisations(userManager, username, out isAdmin);

            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = userServices.Count > 0
                    ? dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(s => userServices.Contains(s.ServiceId) && s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !s.Suggested).ToList()
                    : isAdmin
                        ? unpublished 
                            ? dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(p => p.Published == false && p.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !p.Suggested).ToList()
                            : dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(s => s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !s.Suggested).ToList()
                        : new List<Service>();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                services = String.IsNullOrEmpty(searchTerm)
                    ? services
                    : services.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                switch (orderBy.ToLower())
                {
                case "servicename":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "organisationname":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "updatedon":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "createdon":
                default:
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                }

                foreach (var service in services)
                {
                    ServicePTO serviceToAdd = new ServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        CreatedUserId = service.CreatedUserId,
                        OrganisationId = service.OrganisationId,
                        CreatedOn = service.CreatedOn,
                        CreatedUserName = String.IsNullOrEmpty(service.CreatedUser.Name) ? service.CreatedUser.Email.Substring(0, service.CreatedUser.Email.IndexOf("@")) : service.CreatedUser.Name,
                        LastUpdatedUserId = service.UpdatedUserId,
                        LastUpdated = service.UpdatedOn,
                        OrganisationName = service.Organisation.Name,
                        LastUpdatedUserName = service.UpdatedUserId.HasValue ? String.IsNullOrEmpty(service.UpdatedUser.Name) ? service.UpdatedUser.Email.Substring(0, service.UpdatedUser.Email.IndexOf("@")) : service.UpdatedUser.Name : "",
                        LinkedLocations = dc.ServiceLocations.Count(s => s.ServiceId == service.ServiceId),
                        LinkedAreas = dc.ServiceServiceAreas.Count(s => s.ServiceId == service.ServiceId),
                        Published = service.Published,
                        OrganisationPublished = dc.Organisations.Find(service.OrganisationId).Published,
                        Deprioritised = service.Deprioritised,
                        CanEdit = (userServices.Count == 0 && isAdmin) || userServices.Contains(service.ServiceId) || ValidServiceForEditor(userManager, username, service.ServiceId),
                        CanEditOrganisation = (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(service.OrganisationId) || _organisationService.ValidOrganisationForEditor(userManager, username, service.OrganisationId),
                    };
                    serviceList.Services.Add(serviceToAdd);
                }
            }

            return serviceList;
        }

        public ServiceListingViewModel GetUnfilteredServices(ApplicationUserManager userManager, string username, string searchTerm, int page, string orderBy = "createdon", int descending = 1, bool isAdmin = true)
        {
            ServiceListingViewModel serviceList = new ServiceListingViewModel()
            {
                Services = new List<ServicePTO>(),
                SearchTerm = searchTerm,
                Page = page
            };

            List<Guid> userServices = GetUserServices(userManager, username, out isAdmin);
            List<Guid> userOrganisations = _organisationService.GetUserOrganisations(userManager, username, out isAdmin);

            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.Where(x => x.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !x.Suggested).Include(o => o.Organisation).ToList();

                if (!isAdmin)
                {
                    services = services.Where(x => x.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && x.Organisation.Published && !x.Suggested).ToList();
                }

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                services = String.IsNullOrEmpty(searchTerm) ? services : services.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                switch (orderBy.ToLower())
                {
                case "servicename":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "organisationname":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "updatedon":
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "createdon":
                default:
                    services = descending == 1
                        ? services.OrderBy(d => d.Deprioritised).ThenByDescending(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : services.OrderBy(d => d.Deprioritised).ThenBy(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                }

                foreach (var service in services)
                {
                    ServicePTO serviceToAdd = new ServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        OrganisationId = service.OrganisationId,
                        CreatedOn = service.CreatedOn,
                        LastUpdated = service.UpdatedOn,
                        OrganisationName = service.Organisation.Name,
                        LinkedLocations = dc.ServiceLocations.Count(s => s.ServiceId == service.ServiceId),
                        LinkedAreas = dc.ServiceServiceAreas.Count(s => s.ServiceId == service.ServiceId),
                        CanEdit = (userServices.Count == 0 && isAdmin) || userServices.Contains(service.ServiceId) || ValidServiceForEditor(userManager, username, service.ServiceId),
                        CanEditOrganisation = (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(service.OrganisationId) || _organisationService.ValidOrganisationForEditor(userManager, username, service.OrganisationId),
                        Deprioritised = service.Deprioritised,
                    };
                    serviceList.Services.Add(serviceToAdd);
                }
            }

            return serviceList;
        }

        public ServiceReviewListingViewModel GetAllServiceReviews(ApplicationUserManager userManager, string username, string searchTerm, int page, string orderBy = "status", int descending = 1)
        {
            ServiceReviewListingViewModel reviewList = new ServiceReviewListingViewModel()
            {
                Reviews = new List<ReviewPTO>(),
                SearchTerm = searchTerm,
                Page = page
            };

            bool isAdmin = userManager.IsInRole(userManager.FindByName(username).Id, RolesEnum.ALISSAdmin.ToString());

            List<Guid> userServices = isAdmin
                ? GetAdminServices(userManager, username)
                : GetUserServices(userManager, username, out isAdmin);
            List<Guid> userOrganisations = _organisationService.GetUserOrganisations(userManager, username, out isAdmin);
            int skip = (page - 1) * _ItemsPerPage;

            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceReview> serviceReviews = new List<ServiceReview>();

                if (userServices.Count > 0)
                {
                    serviceReviews = dc.ServiceReviews
                        .Include(r => r.Service)
                        .Include(r => r.Service.Organisation)
                        .Where(r => userServices.Contains(r.ServiceId) && r.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !r.Service.Suggested)
                        .ToList();
                }
                else if (userServices.Count == 0 && isAdmin)
                {
                    serviceReviews = dc.ServiceReviews
                        .Include(r => r.Service)
                        .Include(r => r.Service.Organisation)
                        .Where(r => r.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !r.Service.Suggested)
                        .ToList();
                }

                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                serviceReviews = string.IsNullOrEmpty(searchTerm)
                    ? serviceReviews
                    : serviceReviews.Where(n => Regex.Replace(n.Service.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Service.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                reviewList.TotalPages = (int)Math.Ceiling((double)serviceReviews.Count / _ItemsPerPage);
                reviewList.TotalResults = serviceReviews.Count;

                switch (orderBy.ToLower())
                {
                case "servicename":
                    serviceReviews = descending == 1
                        ? serviceReviews.OrderByDescending(n => n.Service.Name.Trim()).Skip(skip).Take(_ItemsPerPage).ToList()
                        : serviceReviews.OrderBy(n => n.Service.Name.Trim()).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "organisationname":
                    serviceReviews = descending == 1
                        ? serviceReviews.OrderByDescending(n => n.Service.Organisation.Name.Trim()).Skip(skip).Take(_ItemsPerPage).ToList()
                        : serviceReviews.OrderBy(n => n.Service.Organisation.Name.Trim()).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;

                case "createdon":
                    serviceReviews = descending == 1
                        ? serviceReviews.OrderByDescending(n => n.Service.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                        : serviceReviews.OrderBy(n => n.Service.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                case "lastreviewed":
                    serviceReviews = descending == 1
                        ? serviceReviews.OrderByDescending(n => n.LastReviewedDate).Skip(skip).Take(_ItemsPerPage).ToList()
                        : serviceReviews.OrderBy(n => n.LastReviewedDate).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                default:
                    serviceReviews = descending == 1
                        ? serviceReviews.OrderByDescending(n => n.Service.Deprioritised).ThenBy(n => n.LastReviewedDate).Skip(skip).Take(_ItemsPerPage).ToList()
                        : serviceReviews.OrderBy(n => n.Service.Deprioritised).ThenBy(n => n.LastReviewedDate).Skip(skip).Take(_ItemsPerPage).ToList();
                    break;
                }

                foreach (ServiceReview serviceReview in serviceReviews)
                {
                    ReviewPTO reviewToAdd = new ReviewPTO()
                    {
                        LastReviewedDate = serviceReview.LastReviewedDate ?? DateTime.MinValue,
                        ReviewStatus = serviceReview.ReviewEmailState ?? 0,
                        Service = new ServicePTO()
                        {
                            ServiceId = serviceReview.Service.ServiceId,
                            Name = serviceReview.Service.Name,
                            CreatedUserId = serviceReview.Service.CreatedUserId,
                            OrganisationId = serviceReview.Service.OrganisationId,
                            CreatedOn = serviceReview.Service.CreatedOn,
                            CreatedUserName = string.IsNullOrEmpty(serviceReview.Service.CreatedUser.Name) ? serviceReview.Service.CreatedUser.Email.Substring(0, serviceReview.Service.CreatedUser.Email.IndexOf("@")) : serviceReview.Service.CreatedUser.Name,
                            LastUpdatedUserId = serviceReview.Service.UpdatedUserId,
                            LastUpdated = serviceReview.Service.UpdatedOn,
                            OrganisationName = serviceReview.Service.Organisation.Name,
                            LastUpdatedUserName = serviceReview.Service.UpdatedUserId.HasValue ? string.IsNullOrEmpty(serviceReview.Service.UpdatedUser.Name) ? serviceReview.Service.UpdatedUser.Email.Substring(0, serviceReview.Service.UpdatedUser.Email.IndexOf("@")) : serviceReview.Service.UpdatedUser.Name : "",
                            LinkedLocations = dc.ServiceLocations.Count(s => s.ServiceId == serviceReview.Service.ServiceId),
                            LinkedAreas = dc.ServiceServiceAreas.Count(s => s.ServiceId == serviceReview.Service.ServiceId),
                            Published = serviceReview.Service.Published,
                            OrganisationPublished = dc.Organisations.Find(serviceReview.Service.OrganisationId).Published,
                            Deprioritised = serviceReview.Service.Deprioritised,
                            CanEdit = (userServices.Count == 0 && isAdmin) || userServices.Contains(serviceReview.Service.ServiceId) || ValidServiceForEditor(userManager, username, serviceReview.Service.ServiceId),
                            CanEditOrganisation = (userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(serviceReview.Service.OrganisationId) || _organisationService.ValidOrganisationForEditor(userManager, username, serviceReview.Service.OrganisationId),
                        }
                    };

                    reviewList.Reviews.Add(reviewToAdd);
                }
            }

            return reviewList;
        }

        public List<Guid> GetAdminServices(ApplicationUserManager userManager, string username)
        {

            List<Guid> userServices = new List<Guid>();
            List<Guid> userOrganisations = new List<Guid>();
            ApplicationUser user = userManager.FindByName(username);
            bool isAdmin = userManager.IsInRole(user.Id, RolesEnum.ALISSAdmin.ToString());

            if (isAdmin)
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    var userId = dc.UserProfiles.Where(u => u.Username == username).FirstOrDefault().UserProfileId;
                    userOrganisations = dc.Organisations.Where(c => (c.CreatedUserId == userId && !c.ClaimedUserId.HasValue) || c.ClaimedUserId == userId).Select(o => o.OrganisationId).ToList();
                    userOrganisations.AddRange(dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == userId).Select(x => x.OrganisationId.Value));
                    userServices = dc.Services.Where(o => userOrganisations.Contains(o.OrganisationId)).Select(s => s.ServiceId).ToList();
                    userServices.AddRange(dc.ServiceClaimUsers.Where(x => x.ClaimedUserId == userId).Select(x => x.ServiceId.Value));
                }
            }

            return userServices;
        }

        public DeprioritisedServicesViewModel GetDeprioritisedServices(string searchTerm, int page, string orderBy = "deprioritisedon", int descending = 1)
        {
            DeprioritisedServicesViewModel serviceList = new DeprioritisedServicesViewModel()
            {
                Services = new List<DeprioritisedServicePTO>(),
                SearchTerm = searchTerm,
                Page = page
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.Where(s => s.Deprioritised).ToList();
                List<ServiceReview> reviews = dc.ServiceReviews.Where(s => s.Service.Deprioritised).ToList();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                services = String.IsNullOrEmpty(searchTerm)
                    ? services
                    : services.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                reviews = String.IsNullOrEmpty(searchTerm)
                    ? reviews
                    : reviews.Where(n => Regex.Replace(n.Service.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Service.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                switch (orderBy.ToLower())
                {
                    case "servicename":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "organisationname":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "createduser":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.CreatedUser.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.CreatedUser.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "claimeduser":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.ClaimedUser?.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.ClaimedUser?.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "deprioritisedon":
                    default:
                        services = descending == 1
                            ? reviews.OrderByDescending(n => n.ReviewEmailDate).Skip(skip).Take(_ItemsPerPage).Select(s => s.Service).ToList()
                            : reviews.OrderBy(n => n.ReviewEmailDate).Skip(skip).Take(_ItemsPerPage).Select(s => s.Service).ToList();
                        break;
                }

                foreach (var service in services)
                {
                    int managers = dc.ServiceClaimUsers.Where(c => c.ServiceId == service.ServiceId && !c.IsLeadClaimant).Count();
                    ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == service.ServiceId);
                    DeprioritisedServicePTO serviceToAdd = new DeprioritisedServicePTO
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.Name,
                        OrganisationId = service.Organisation.OrganisationId,
                        OrganisationName = service.Organisation.Name,

                        DeprioritisedDate = review.ReviewEmailDate ?? DateTime.MinValue,
                        CreatedUserId = service.CreatedUser.UserProfileId,
                        CreatedUserName = string.IsNullOrWhiteSpace(service.CreatedUser.Name) ? service.CreatedUser.Username : service.CreatedUser.Name,
                        ClaimedUserId = service.ClaimedUser != null ? service.ClaimedUser?.UserProfileId : 0,
                        ClaimedUserName = service.ClaimedUser != null ? string.IsNullOrWhiteSpace(service.ClaimedUser?.Name) ? service.ClaimedUser?.Username : service.ClaimedUser.Name : "",
                        Managers = managers,
                        ServicePublished = service.Published,
                        OrganisationPublished = service.Organisation.Published
                    };
                    serviceList.Services.Add(serviceToAdd);
                }
            }

            return serviceList;
        }

        public ServiceListingViewModel GetSuggestedServices(ApplicationUserManager userManager, string username, string searchTerm, int page, bool unpublished = false, string orderBy = "createdon", int descending = 1)
        {
            ServiceListingViewModel serviceList = new ServiceListingViewModel()
            {
                Services = new List<ServicePTO>(),
                SearchTerm = searchTerm,
                Page = page
            };
            List<Guid> userServices = GetUserServices(userManager, username, out bool isAdmin);

            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = userServices.Count > 0
                    ? dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(s => userServices.Contains(s.ServiceId) && s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && s.Suggested).ToList()
                    : isAdmin
                        ? unpublished
                            ? dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(p => p.Published == false && p.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).ToList()
                            : dc.Services.Include(c => c.CreatedUser).Include(o => o.Organisation).Where(s => s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && s.Suggested).ToList()
                        : new List<Service>();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                services = String.IsNullOrEmpty(searchTerm)
                    ? services
                    : services.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty).Contains(searchTerm.ToLower()) || n.Organisation.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                switch (orderBy.ToLower())
                {
                    case "servicename":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "organisationname":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.Organisation.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "updatedon":
                        services = descending == 1
                            ? services.OrderByDescending(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.UpdatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                    case "createdon":
                    default:
                        services = descending == 1
                            ? services.OrderByDescending(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList()
                            : services.OrderBy(n => n.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();
                        break;
                }

                foreach (var service in services)
                {
                    ServicePTO serviceToAdd = new ServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        CreatedUserId = service.CreatedUserId,
                        OrganisationId = service.OrganisationId,
                        CreatedOn = service.CreatedOn,
                        CreatedUserName = String.IsNullOrEmpty(service.CreatedUser.Name) ? service.CreatedUser.Email.Substring(0, service.CreatedUser.Email.IndexOf("@")) : service.CreatedUser.Name,
                        CreatedUserEmail = service.CreatedUser.Email,
                        LastUpdatedUserId = service.UpdatedUserId,
                        LastUpdated = service.UpdatedOn,
                        OrganisationName = service.Organisation.Name,
                        LastUpdatedUserName = service.UpdatedUserId.HasValue ? String.IsNullOrEmpty(service.UpdatedUser.Name) ? service.UpdatedUser.Email.Substring(0, service.UpdatedUser.Email.IndexOf("@")) : service.UpdatedUser.Name : "",
                        LinkedLocations = dc.ServiceLocations.Count(s => s.ServiceId == service.ServiceId),
                        LinkedAreas = dc.ServiceServiceAreas.Count(s => s.ServiceId == service.ServiceId),
                        Published = service.Published,
                        OrganisationPublished = dc.Organisations.Find(service.OrganisationId).Published,
                        Deprioritised = service.Deprioritised,
                        ServiceEditable = username == service.ClaimedUser?.Username || username == service.Organisation.ClaimedUser?.Username || isAdmin,
                        OrganisationEditable = username == service.Organisation.ClaimedUser?.Username || isAdmin,
                    };
                    serviceList.Services.Add(serviceToAdd);
                }
            }

            return serviceList;
        }

        public ServiceListingViewModel GetServices(Guid organisationId, string searchTerm, int page)
        {
            ServiceListingViewModel serviceList = new ServiceListingViewModel()
            {
                Services = new List<ServicePTO>(),
                SearchTerm = searchTerm,
                Page = page
            };

            using (ALISSContext dc = new ALISSContext())
            {
                serviceList.OrganisationName = dc.Organisations.Find(organisationId).Name;

                List<Service> services = dc.Services.Include(c => c.CreatedUser).Include(u => u.UpdatedUser).Where(o => o.OrganisationId == organisationId && o.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).ToList();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                services = String.IsNullOrEmpty(searchTerm) ? services : services.Where(n => Regex.Replace(n.Name.ToLower(), regexMatch, string.Empty)
                .Contains(searchTerm.ToLower())).ToList();

                serviceList.TotalPages = (int)Math.Ceiling((double)services.Count / _ItemsPerPage);
                serviceList.TotalResults = services.Count;
                int skip = (page - 1) * _ItemsPerPage;
                services = services.OrderBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (var service in services)
                {
                    ServicePTO serviceToAdd = new ServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        CreatedUserId = service.CreatedUserId,
                        CreatedOn = service.CreatedOn,
                        CreatedUserName = service.CreatedUser.Name,
                        LastUpdatedUserId = service.UpdatedUserId,
                        LastUpdated = service.UpdatedUserId.HasValue ? service.UpdatedOn : null,
                        LastUpdatedUserName = service.UpdatedUserId.HasValue ? service.UpdatedUser.Name : "",
                        LinkedLocations = dc.ServiceLocations.Count(s => s.ServiceId == service.ServiceId),
                        LinkedAreas = dc.ServiceServiceAreas.Count(s => s.ServiceId == service.ServiceId),
                        Published = service.Published,
                        OrganisationPublished = service.Organisation.Published,
                    };
                    serviceList.Services.Add(serviceToAdd);
                }
            }

            return serviceList;
        }

        public List<Guid> GetServicesToBeReviewed(int emailNum)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri($"{ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/')}")
            };
            Dictionary<string, string> reviewInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.GetStringAsync("/umbraco/api/config/GetDataGovernance").Result);
            int firstEmail = int.Parse(reviewInformation["Email1NotificationMonths"]);
            int secondEmail = int.Parse(reviewInformation["Email2NotificationMonths"]);
            int thirdEmail = int.Parse(reviewInformation["Email3NotificationMonths"]);
            int bulkReview = int.Parse(reviewInformation["BulkReviewMonths"]);

            bool enableReviewTest = ConfigurationManager.AppSettings["Settings:EnableReviewEmailTest"] != null
                ? ConfigurationManager.AppSettings["Settings:EnableReviewEmailTest"].ToString().ToLower() == "true"
                : false;

            List<Service> emailServices;
            List<int> userList;
            DateTime emailThreshold = DateTime.Today;
            DateTime adaptedEmailThreshold = DateTime.Today;
            DateTime reviewEmailThreshold = DateTime.Today;
            ReviewEmailEnum requiredState = ReviewEmailEnum.ReceviedEmail3;

            switch (emailNum)
            {
            case 1:
                emailThreshold = enableReviewTest ? DateTime.Today.AddDays(-firstEmail) : DateTime.Today.AddMonths(-firstEmail);
                requiredState = ReviewEmailEnum.RecentlyReviewed;
                    adaptedEmailThreshold = enableReviewTest ? emailThreshold.AddDays(bulkReview) : emailThreshold.AddMonths(bulkReview);
                break;
            case 2:
                requiredState = ReviewEmailEnum.ReceviedEmail1;
                reviewEmailThreshold = enableReviewTest ? DateTime.Today.AddDays(-secondEmail) : DateTime.Today.AddMonths(-secondEmail);
                    adaptedEmailThreshold = enableReviewTest ? reviewEmailThreshold.AddDays(bulkReview) : reviewEmailThreshold.AddMonths(bulkReview);
                break;
            case 3:
                requiredState = ReviewEmailEnum.ReceviedEmail2;
                reviewEmailThreshold = enableReviewTest ? DateTime.Today.AddDays(-thirdEmail) : DateTime.Today.AddMonths(-thirdEmail);
                    adaptedEmailThreshold = enableReviewTest ? reviewEmailThreshold.AddDays(bulkReview) : reviewEmailThreshold.AddMonths(bulkReview);
                break;
            }

            using (ALISSContext dc = new ALISSContext())
            {
                emailServices = emailNum == 1
                    ? dc.ServiceReviews
                        .Where(s => DateTime.Compare(s.LastReviewedDate.Value, emailThreshold) <= 0
                            && !s.Service.Deprioritised
                            && s.ReviewEmailState == (int)requiredState)
                        .Select(s => s.Service)
                        .ToList()
                    : dc.ServiceReviews
                        .Where(s => DateTime.Compare(s.ReviewEmailDate.Value, reviewEmailThreshold) <= 0
                            && !s.Service.Deprioritised
                            && s.ReviewEmailState == (int)requiredState)
                        .Select(s => s.Service)
                        .ToList();

                userList = emailServices.Select(s => s.ClaimedUser != null ? s.ClaimedUserId.Value : s.CreatedUserId).Distinct().ToList();

                List<Guid> serviceIds = emailServices.Select(s => s.ServiceId).Distinct().ToList();

                List<Service> extraEmailServices = emailNum == 1
                    ? dc.ServiceReviews
                        .Where(s => DateTime.Compare(s.LastReviewedDate.Value, adaptedEmailThreshold) <= 0
                            && !s.Service.Deprioritised
                            && s.ReviewEmailState == (int)requiredState)
                        .Select(s => s.Service)
                        .ToList()
                    : dc.ServiceReviews
                        .Where(s => DateTime.Compare(s.ReviewEmailDate.Value, adaptedEmailThreshold) <= 0
                            && !s.Service.Deprioritised
                            && s.ReviewEmailState == (int)requiredState)
                    .Select(s => s.Service)
                    .ToList();
                emailServices.AddRange(extraEmailServices.Where(s => userList.Contains(s.ClaimedUserId != null ? s.ClaimedUserId.Value : s.CreatedUserId)).Distinct());
            }

            // Get a list of service and claimed/created user IDs
            IEnumerable<KeyValuePair<Guid, int>> serviceUserList = emailServices
                .Distinct()
                .OrderBy(s => s.ClaimedUserId ?? s.CreatedUserId)
                .ToDictionary(s => s.ServiceId, s => s.ClaimedUserId ?? s.CreatedUserId);

            if (serviceUserList.Count() > 500)
            {
                // Get a distinct list of User IDs of the first 500 services
                List<int> batchUsers = serviceUserList.Take(500).Select(u => u.Value).Distinct().ToList();

                // Return the service IDs where the claimed user or created user is in the distinct list of User IDs (should be 500+)
                return emailServices
                    .Where(s => (s.ClaimedUserId.HasValue && batchUsers.Contains(s.ClaimedUserId.Value))
                        || (!s.ClaimedUserId.HasValue && batchUsers.Contains(s.CreatedUserId)))
                    .Select(s => s.ServiceId)
                    .Distinct()
                    .ToList();
            }

            return emailServices
                .Select(s => s.ServiceId)
                .Distinct()
                .ToList();
        }

        public EditServiceViewModel GetEmptyModelForAdd(Guid organisationId, bool newOrganisation = false)
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

            EditServiceViewModel model = new EditServiceViewModel()
            {
                ServiceCategories = _categoryService.GetCategoryListForService(null, new List<string>()),
                Locations = _locationService.GetLocationListForServices(organisationId, null),
                ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(null, new List<string>()),
                OrganisationId = organisationId,
                NewOrganisation = newOrganisation,
                NewOrganisationName = organisationName
            };

            return model;
        }

        public EditServiceViewModel RepopulateModelForError(EditServiceViewModel model)
        {
            List<string> selectedCategories = String.IsNullOrEmpty(model.SelectedCategories) ? new List<string>() : model.SelectedCategories.Split(',').ToList();
            List<string> selectedServiceAreas = String.IsNullOrEmpty(model.SelectedServiceAreas) ? new List<string>() : model.SelectedServiceAreas.Split(',').ToList();

            model.ServiceCategories = _categoryService.GetCategoryListForService(null, selectedCategories);
            model.Locations = _locationService.GetLocationListForServices(model.OrganisationId, null);
            model.ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(null, selectedServiceAreas);

            if (!String.IsNullOrEmpty(model.SelectedLocations))
            {
                List<string> serviceLocations = model.SelectedLocations.Split(',').ToList();
                foreach (var serviceLocation in serviceLocations)
                {
                    model.Locations.FirstOrDefault(c => c.Value == serviceLocation).Selected = true;
                }
            }

            if (!String.IsNullOrEmpty(model.SelectedServiceAreas))
            {
                List<string> serviceAreas = model.SelectedServiceAreas.Split(',').ToList();
                foreach (var serviceArea in serviceAreas)
                {
                    model.ServiceServiceAreas.FirstOrDefault(c => c.ServiceAreaId.ToString() == serviceArea).Selected = true;
                }
            }

            return model;
        }

        public string AddService(EditServiceViewModel model, int userProfileId, int currentTestStep = 0)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var organisation = dc.Organisations.Find(model.OrganisationId);

                Service serviceToAdd = new Service()
                {
                    ServiceId = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Phone = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber,
                    Email = model.Email,
                    Facebook = model.Facebook,
                    Twitter = model.Twitter,
                    Instagram = model.Instagram,
                    CreatedUserId = userProfileId,
                    CreatedOn = DateTime.UtcNow.Date,
                    OrganisationId = model.OrganisationId,
                    Slug = model.Slug,
                    Url = model.Url,
                    ReferralUrl = model.ReferralUrl,
                    LastEditedStep = currentTestStep,
                    Published = organisation.Published
                };

                int slugAppend = 0;
                while (DoesServiceSlugExist(serviceToAdd.Slug, serviceToAdd.ServiceId))
                {
                    slugAppend++;
                    serviceToAdd.Slug = $"{model.Slug}-{slugAppend}";
                }

                dc.Services.Add(serviceToAdd);

                var serviceReview = new ServiceReview
                {
                    ReviewId = Guid.NewGuid(),
                    ServiceId = serviceToAdd.ServiceId,
                    LastReviewedDate = DateTime.UtcNow.Date,
                    ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed,
                };
                dc.ServiceReviews.Add(serviceReview);

                if (!String.IsNullOrEmpty(model.SelectedCategories))
                {
                    List<string> serviceCategories = model.SelectedCategories.Split(',').ToList();
                    foreach (var serviceCategory in serviceCategories)
                    {
                        var categoryToAdd = new ServiceCategory()
                        {
                            ServiceId = serviceToAdd.ServiceId,
                            CategoryId = Convert.ToInt32(serviceCategory)
                        };
                        dc.ServiceCategories.Add(categoryToAdd);
                    }
                }

                if (!String.IsNullOrEmpty(model.SelectedLocations))
                {
                    List<string> serviceLocations = model.SelectedLocations.Split(',').ToList();
                    foreach (var serviceLocation in serviceLocations)
                    {
                        var locationToAdd = new ServiceLocation()
                        {
                            ServiceId = serviceToAdd.ServiceId,
                            LocationId = Guid.Parse(serviceLocation)
                        };
                        dc.ServiceLocations.Add(locationToAdd);
                    }
                }

                if (!String.IsNullOrEmpty(model.SelectedServiceAreas))
                {
                    List<string> serviceServiceAreas = model.SelectedServiceAreas.Split(',').ToList();
                    foreach (var serviceServiceArea in serviceServiceAreas)
                    {
                        var serviceAreaToAdd = new ServiceServiceArea()
                        {
                            ServiceId = serviceToAdd.ServiceId,
                            ServiceAreaId = Convert.ToInt32(serviceServiceArea)
                        };
                        dc.ServiceServiceAreas.Add(serviceAreaToAdd);
                    }
                }

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = serviceToAdd.ServiceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();

                if (model.ServiceRepresentative)
                {
                    AddClaimViewModel claimToAdd = new AddClaimViewModel()
                    {
                        ClaimedUserId = userProfileId,
                        Id = serviceToAdd.ServiceId,
                        RepresentativeName = model.ServiceRepresentativeName,
                        RepresentativePhone = model.ServiceRepresentativePhone,
                        RequestLeadClaimant = true,
                        RepresentativeRole = model.ServiceRepresentativeRole
                    };

                    ServiceClaimService.AddClaim(claimToAdd);
                }

                if (organisation.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToAdd.ServiceId);
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                }
            }

            return "Service added successfully";
        }

        public EditServiceViewModel GetServiceToEdit(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                return new EditServiceViewModel()
                {
                    ServiceId = service.ServiceId,
                    OrganisationId = service.OrganisationId,
                    Name = service.Name,
                    Description = service.Description,
                    PhoneNumber = String.IsNullOrEmpty(service.Phone) ? service.Phone : service.Phone.Replace(" ", ""),
                    Email = service.Email,
                    Url = service.Url,
                    ReferralUrl = service.ReferralUrl,
                    Facebook = service.Facebook,
                    Twitter = service.Twitter,
                    Instagram = service.Instagram,
                    Slug = service.Slug,
                    ServiceCategories = _categoryService.GetCategoryListForService(service.ServiceId, new List<string>()),
                    Locations = _locationService.GetLocationListForServices(service.OrganisationId, service.ServiceId),
                    ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(service.ServiceId, new List<string>()),
                    SelectedCategories = String.Join(",", dc.ServiceCategories.Where(s => s.ServiceId == serviceId).Select(c => c.CategoryId).ToList()),
                    SelectedLocations = String.Join(",", dc.ServiceLocations.Where(s => s.ServiceId == serviceId).Select(l => l.LocationId).ToList()),
                    SelectedServiceAreas = String.Join(",", dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceId).Select(s => s.ServiceAreaId).ToList()),
                    Published = service.Published,
                    NewOrganisationName = service.Organisation.Name,
                    Logo = service.Logo,
                };
            }
        }

        public string ChangeLeadServiceClaimant(MakeLeadServiceClaimantViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaimUser oldClaimant = dc.ServiceClaimUsers.Where(x => x.ServiceId == model.Claimant.ServiceId && x.IsLeadClaimant).FirstOrDefault();
                oldClaimant.IsLeadClaimant = false;

                ServiceClaimUser newLeadClaimant = dc.ServiceClaimUsers.Find(model.Claimant.ServiceClaimUserId);
                newLeadClaimant.IsLeadClaimant = true;
                
                Service service = dc.Services.Find(model.Claimant.ServiceId);
                service.ClaimedUserId = newLeadClaimant.ClaimedUserId;
                service.ClaimedOn = DateTime.Now;

                dc.SaveChanges();

                if (service.Organisation.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                }

                UserProfile userProfile = dc.UserProfiles.Find(newLeadClaimant.ClaimedUserId.Value);

                _emailService.SendLeadClaimantChangeEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.Name, $"{publicBaseUrl}/services/{service.Slug}", model.Message);
            }

            return "Lead claimant changed successfully";
        }

		public string EditService(EditServiceViewModel model, int userProfileId, int currentTestStep = (int)DataInputStepsEnum.DataInputSubmitted)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);
                serviceToEdit.Name = model.Name;
                serviceToEdit.Summary = model.Summary;
                serviceToEdit.Description = model.Description;
                serviceToEdit.Phone = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber;
                serviceToEdit.Email = model.Email;
                serviceToEdit.Url = model.Url;
                serviceToEdit.ReferralUrl = model.ReferralUrl;
                serviceToEdit.Facebook = model.Facebook;
                serviceToEdit.Instagram = model.Instagram;
                serviceToEdit.Twitter = model.Twitter;
                serviceToEdit.Slug = model.Slug;
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                if (serviceToEdit.LastEditedStep != (int)DataInputStepsEnum.DataInputSubmitted)
                {
                    serviceToEdit.LastEditedStep = currentTestStep;
                }

                int slugAppend = 0;
                while (DoesServiceSlugExist(serviceToEdit.Slug, serviceToEdit.ServiceId))
                {
                    slugAppend++;
                    serviceToEdit.Slug = $"{model.Slug}-{slugAppend}";
                }

                var selectedCategories = dc.ServiceCategories.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceCategories.RemoveRange(selectedCategories);
                var selectedLocations = dc.ServiceLocations.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceLocations.RemoveRange(selectedLocations);
                var selectedAreas = dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceServiceAreas.RemoveRange(selectedAreas);

                if (!String.IsNullOrEmpty(model.SelectedCategories))
                {
                    List<string> serviceCategories = model.SelectedCategories.Split(',').ToList();
                    foreach (var serviceCategory in serviceCategories)
                    {
                        var categoryToAdd = new ServiceCategory()
                        {
                            ServiceId = serviceToEdit.ServiceId,
                            CategoryId = Convert.ToInt32(serviceCategory)
                        };
                        dc.ServiceCategories.Add(categoryToAdd);
                    }
                }

                if (!String.IsNullOrEmpty(model.SelectedLocations))
                {
                    List<string> serviceLocations = model.SelectedLocations.Split(',').ToList();
                    foreach (var serviceLocation in serviceLocations)
                    {
                        var locationToAdd = new ServiceLocation()
                        {
                            ServiceId = serviceToEdit.ServiceId,
                            LocationId = Guid.Parse(serviceLocation)
                        };
                        dc.ServiceLocations.Add(locationToAdd);
                    }
                }

                if (!String.IsNullOrEmpty(model.SelectedServiceAreas))
                {
                    List<string> serviceServiceAreas = model.SelectedServiceAreas.Split(',').ToList();
                    foreach (var serviceServiceArea in serviceServiceAreas)
                    {
                        var serviceAreaToAdd = new ServiceServiceArea()
                        {
                            ServiceId = serviceToEdit.ServiceId,
                            ServiceAreaId = Convert.ToInt32(serviceServiceArea)
                        };
                        dc.ServiceServiceAreas.Add(serviceAreaToAdd);
                    }
                }

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = model.ServiceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();

                if (dc.Organisations.Find(model.OrganisationId).Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                }
            }

            return "Service Edited Successfully";
        }

        public DeleteServiceViewModel GetServiceNameForDeleteConfirmation(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToDelete = dc.Services.Find(serviceId);
                return new DeleteServiceViewModel()
                {
                    ServiceId = serviceToDelete.ServiceId,
                    OrganisationId = serviceToDelete.OrganisationId,
                    ServiceName = serviceToDelete.Name
                };
            }
        }

        public Guid DeleteService(Guid serviceId, out string errorMessage)
        {
            errorMessage = "";

            Guid organisationId = Guid.NewGuid();

            _elasticSearchService.DeleteService(serviceId);

            if (_elasticSearchService.GetServiceById(serviceId) == null)
            {
                try
                {
                    using (ALISSContext dc = new ALISSContext())
                    {
                        Service serviceToDelete = dc.Services.Find(serviceId);

                        var collectionServicesToRemove = dc.CollectionServices.Where(s => s.ServiceId == serviceId);
                        if (collectionServicesToRemove.Count() > 0)
                        {
                            dc.CollectionServices.RemoveRange(collectionServicesToRemove);
                        }

                        var claimUsersToRemove = dc.ServiceClaimUsers.Where(s => s.ServiceId == serviceId);
                        if (claimUsersToRemove.Count() > 0)
                        {
                            dc.ServiceClaimUsers.RemoveRange(claimUsersToRemove);
                        }

                        var claimsToRemove = dc.ServiceClaims.Where(s => s.ServiceId == serviceId);
                        if (claimsToRemove.Count() > 0)
                        {
                            dc.ServiceClaims.RemoveRange(claimsToRemove);
                        }

                        var locationsToRemove = dc.ServiceLocations.Where(s => s.ServiceId == serviceId);
                        if (locationsToRemove.Count() > 0)
                        {
                            dc.ServiceLocations.RemoveRange(locationsToRemove);
                        }

                        var serviceAreasToRemove = dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceId);
                        if (serviceAreasToRemove.Count() > 0)
                        {
                            dc.ServiceServiceAreas.RemoveRange(serviceAreasToRemove);
                        }

                        var categoriesToRemove = dc.ServiceCategories.Where(s => s.ServiceId == serviceId);
                        if (categoriesToRemove.Count() > 0)
                        {
                            dc.ServiceCategories.RemoveRange(categoriesToRemove);
                        }

                        var accessibilitiesToRemove = dc.ServiceAccessibilityFeatures.Where(s => s.ServiceId == serviceId);
                        if (accessibilitiesToRemove.Count() > 0)
                        {
                            dc.ServiceAccessibilityFeatures.RemoveRange(accessibilitiesToRemove);
                        }

                        var communityGroupsToRemove = dc.ServiceCommunityGroups.Where(s => s.ServiceId == serviceId);
                        if (communityGroupsToRemove.Count() > 0)
                        {
                            dc.ServiceCommunityGroups.RemoveRange(communityGroupsToRemove);
                        }

                        var reviewsToRemove = dc.ServiceReviews.Where(s => s.ServiceId == serviceId);
                        if (reviewsToRemove.Count() > 0)
                        {
                            dc.ServiceReviews.RemoveRange(reviewsToRemove);
                        }

                        var improvementsToUpdate = dc.Improvements.Where(s => s.ServiceId == serviceId);
                        if (improvementsToUpdate.Count() > 0)
                        {
                            foreach (Improvement improvement in improvementsToUpdate)
                            {
                                improvement.ServiceName = serviceToDelete.Name;
                                improvement.ServiceId = null;
                            }
                        }

                        var serviceAuditsToRemove = dc.ServiceAudits.Where(s => s.ServiceId == serviceId);
                        if (serviceAuditsToRemove.Count() > 0)
                        {
                            dc.ServiceAudits.RemoveRange(serviceAuditsToRemove);
                        }

                        organisationId = serviceToDelete.OrganisationId;
                        dc.Services.Remove(serviceToDelete);

                        dc.SaveChanges();

                        _elasticSearchService.AddOrganisationToElasticSearch(organisationId);
                    }
                }
                catch(Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            else
            {
                errorMessage = "Failed to delete the service from the index, please try again.";
            }

            return organisationId;
        }

        public ShareServiceViewModel GetServiceToShare(ShareServiceViewModel model)
        {
            model.ServiceId = _elasticSearchService.GetServiceIdFromSlug(model.ServiceSlug);
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(model.ServiceId);
                model.ServiceName = service.Name;
                model.OrganisationName = service.Organisation.Name;
                model.Link = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString() + "services/" + model.ServiceSlug;
            }

            return model;
        }

        public List<Guid> GetUserServices(ApplicationUserManager userManager, string username, out bool isAdmin)
        {

            List<Guid> userServices = new List<Guid>();
            List<Guid> userOrganisations = new List<Guid>();
            ApplicationUser user = userManager.FindByName(username);
            bool isEditor = userManager.IsInRole(user.Id, RolesEnum.Editor.ToString());
            bool isBasic = userManager.IsInRole(user.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(user.Id, RolesEnum.ClaimedUser.ToString());
            isAdmin = !isEditor && !isBasic;

            using (ALISSContext dc = new ALISSContext())
            {
                var userId = dc.UserProfiles.Where(u => u.Username == username).FirstOrDefault().UserProfileId;
                if (isEditor)
                {
                    List<Guid> userCreatedOrganisations = dc.Organisations.Where(c => (c.CreatedUserId == userId && (!c.ClaimedUserId.HasValue || c.ClaimedUserId == userId)) || c.ClaimedUserId == userId).Select(o => o.OrganisationId).ToList();
                    List<Guid> userServiceOrganisations = dc.Services.Include(o => o.Organisation).Where(o => o.CreatedUserId == userId && !o.Organisation.ClaimedUserId.HasValue && !o.Suggested && o.LastEditedStep != (int)DataInputStepsEnum.DataInputSubmitted).Select(s => s.OrganisationId).ToList();
                    userOrganisations = userCreatedOrganisations.Union(userServiceOrganisations).ToList();
                }
                else if (isBasic)
                {
                    userOrganisations = dc.Organisations.Where(c => (c.CreatedUserId == userId && !c.ClaimedUserId.HasValue) || c.ClaimedUserId == userId).Select(o => o.OrganisationId).ToList();
                }
                if (!isAdmin)
                {
                    userOrganisations.AddRange(dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == userId).Select(x => x.OrganisationId.Value));
                    userServices = dc.Services.Where(o => userOrganisations.Contains(o.OrganisationId) || (o.CreatedUserId == userId && o.Suggested && o.LastEditedStep != (int)DataInputStepsEnum.DataInputSubmitted)).Select(s => s.ServiceId).ToList();
                    userServices.AddRange(dc.ServiceClaimUsers.Where(x => x.ClaimedUserId == userId).Select(x => x.ServiceId.Value));
                }
            }

            return userServices;
        }

        public ServiceClaimUser GetServiceClaimant(Guid serviceClaimUserId)
        {
            ServiceClaimUser serviceClaimUser = new ServiceClaimUser();

            using (ALISSContext dc = new ALISSContext())
            {
                serviceClaimUser = dc.ServiceClaimUsers.Find(serviceClaimUserId);
                _ = serviceClaimUser.ClaimedUser;
            }

            return serviceClaimUser;
        }

        public ServiceClaimantListingViewModel ListClaimants(string searchTerm, Guid organisationId, Guid serviceId, int page = 1)
        {
            ServiceClaimantListingViewModel claimantList = new ServiceClaimantListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Claimants = new List<ServiceClaimantPTO>(),
                OrganisationLeadClaimant = new OrganisationClaimantPTO(),
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceClaimUser> claimants = dc.ServiceClaimUsers.Where(x => x.ServiceId == serviceId).OrderByDescending(x => x.IsLeadClaimant).ThenBy(x => x.ClaimedUser.Name).ToList();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                claimants = string.IsNullOrEmpty(searchTerm)
                    ? claimants
                    : claimants.Where(n => (string.IsNullOrEmpty(n.Service.ClaimedUser.Name) == false && n.Service.ClaimedUser.Name.ToLower().Contains(searchTerm.ToLower()))).ToList();

                claimantList.ServiceName = dc.Services.Find(serviceId).Name;
                claimantList.TotalPages = (int)Math.Ceiling((double)claimants.Count / _ItemsPerPage);
                claimantList.TotalResults = claimants.Count;
                int skip = (page - 1) * _ItemsPerPage;

                claimants = claimants.Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (ServiceClaimUser claimant in claimants)
                {
                    ServiceClaimantPTO serviceClaimantListItem = new ServiceClaimantPTO()
                    {
                        ServiceClaimUserId = claimant.ServiceClaimUserId,
                        ServiceId = claimant.ServiceId.Value,
                        ClaimantUserId = claimant.ClaimedUserId.Value,
                        ClaimantName = claimant.ClaimedUser.Name,
                        ClaimantEmail = claimant.ClaimedUser.Email,
                        IsLeadClaimant = claimant.IsLeadClaimant,
                        ApprovedOn = claimant.ApprovedOn
                    };

                    claimantList.Claimants.Add(serviceClaimantListItem);
                }

                OrganisationClaimUser organisationClaimant = dc.OrganisationClaimUsers.FirstOrDefault(x => x.OrganisationId == organisationId && x.IsLeadClaimant);
                if (organisationClaimant != null) {
                    claimantList.OrganisationLeadClaimant.ApprovedOn = organisationClaimant.ApprovedOn;
                    claimantList.OrganisationLeadClaimant.ClaimantName = organisationClaimant.ClaimedUser?.Name;
                    claimantList.OrganisationLeadClaimant.ClaimantEmail = organisationClaimant?.ClaimedUser?.Email;
                    claimantList.OrganisationLeadClaimant.IsLeadClaimant = organisationClaimant.IsLeadClaimant;
                    claimantList.OrganisationLeadClaimant.ClaimantUserId = organisationClaimant.ClaimedUserId.Value;
                    claimantList.OrganisationLeadClaimant.OrganisationClaimUserId = organisationClaimant.OrganisationClaimUserId;
                }
                else
                {
                    claimantList.OrganisationLeadClaimant = null;
                }

                ServiceClaimUser serviceClaimant = dc.ServiceClaimUsers.FirstOrDefault(x => x.ServiceId == serviceId && x.IsLeadClaimant);
                if(serviceClaimant == null && claimantList.OrganisationLeadClaimant != null)
                {
                    claimantList.Claimants.Add(new ServiceClaimantPTO
                    {
                        ServiceId = serviceId,
                        ClaimantUserId = claimantList.OrganisationLeadClaimant.ClaimantUserId,
                        ClaimantName = claimantList.OrganisationLeadClaimant.ClaimantName,
                        ClaimantEmail = claimantList.OrganisationLeadClaimant.ClaimantEmail,
                        IsLeadClaimant = claimantList.OrganisationLeadClaimant.IsLeadClaimant,
                        ApprovedOn = claimantList.OrganisationLeadClaimant.ApprovedOn
                    });
                }
            }

            return claimantList;
        }

        public bool DoesServiceSlugExist(string serviceSlug, Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Where(n => n.Slug.ToLower() == serviceSlug.ToLower()).FirstOrDefault();

                if (service != null && service.ServiceId != serviceId)
                {
                    return true;
                }

                return false;
            }
        }

        public bool DoesServiceIdExist(Guid newServiceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(newServiceId);

                return service != null;
            }
        }

        public bool IsServiceSubmitted(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);


                return service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted;
            }
        }

        public bool DoesServiceExist(string serviceName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Where(n => n.Name.ToLower() == serviceName.ToLower()).FirstOrDefault();

                if (service != null)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanUserEditService(ApplicationUserManager userManager, string username, Guid serviceId)
        {
            List<Guid> userServices = GetUserServices(userManager, username, out bool isAdmin);
            return (userServices.Count == 0 && isAdmin) || userServices.Contains(serviceId) || ValidServiceForEditor(userManager, username, serviceId);
        }

        public bool ValidServiceForEditor(ApplicationUserManager userManager, string username, Guid serviceId)
        {
            ApplicationUser user = userManager.FindByName(username);

            if (!userManager.IsInRole(user.Id, RolesEnum.Editor.ToString())) 
            { 
                return false; 
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);

                if (service.ClaimedUser == null && service.ClaimedOn == null && service.Organisation.ClaimedUser == null && service.Organisation.ClaimedOn == null)
                {
                    if(service.Organisation.ClaimedUser == null && service.Suggested && service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        public void PublishService(Guid serviceId, int currentUserProfileId)
        {
            using (var dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                service.Published = !service.Published;
                service.UpdatedUserId = currentUserProfileId;
                service.UpdatedOn = DateTime.UtcNow.Date;

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = serviceId,
                    UserProfileId = currentUserProfileId,
                    DateOfAction = DateTime.UtcNow.Date
                };
                dc.ServiceAudits.Add(serviceAudit);
                dc.SaveChanges();

                UserProfile userProfile = dc.UserProfiles.Find(service.ClaimedUserId ?? service.CreatedUserId);
                if (service.Published)
                {
                    _emailService.SendServicePublishedEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.Name, service.Slug);
                    _elasticSearchService.AddServiceToElasticSearch(serviceId);
                }
                else
                {
                    _emailService.SendServiceUnpublishedEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.Name);
                    _elasticSearchService.DeleteService(serviceId);
                }

                if (service.Organisation.Published)
                {
                    _elasticSearchService.AddOrganisationToElasticSearch(service.OrganisationId);
                }
            }
        }

        public void ProcessServiceReviewEmails(int emailNumber)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Guid> servicesToBeReviewed = GetServicesToBeReviewed(emailNumber);
                List<int> sentUserIds = new List<int>();

                foreach (Guid serviceId in servicesToBeReviewed)
                {
                    Service service = dc.Services.Find(serviceId);
                    int userId = service.ClaimedUserId ?? service.CreatedUserId;

                    if (!sentUserIds.Contains(userId))
                    {
                        UserProfile user = service.ClaimedUser ?? service.CreatedUser;
                        _emailService.SendInformationReview(user.Email, string.IsNullOrEmpty(user.Name) ? "" : user.Name, user.Username, emailNumber);
                        sentUserIds.Add(user.UserProfileId);

                        if (emailNumber > 1)
                        {
                            List<UserProfile> serviceManagers = _claimService.GetManagers(serviceId);
                            foreach (UserProfile manager in serviceManagers)
                            {
                                if (!sentUserIds.Contains(manager.UserProfileId))
                                {
                                    _emailService.SendInformationReview(manager.Email, string.IsNullOrEmpty(manager.Name) ? "" : manager.Name, manager.Username, emailNumber, true);
                                    sentUserIds.Add(manager.UserProfileId);
                                }
                            }
                        }
                    }

                    ServiceReview review = dc.ServiceReviews.First(s => s.ServiceId == serviceId);

                    review.ReviewEmailDate = DateTime.Today;
                    review.ReviewEmailState = emailNumber;

                    if (emailNumber == 3)
                    {
                        service.Deprioritised = true;
                        dc.SaveChanges();

                        if (service.Published)
                        {
                        _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                    }
                    }
                    else
                    {
                        dc.SaveChanges();
                    }
                }
            }
        }

        public void ServiceReviewFlowTest(int emailNumber)
        {
            _emailService.SendInformationReview("canderson@tactuum.com", "Colin Anderson", "canderson", emailNumber);
        }
    }
}
