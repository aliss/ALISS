using ALISS.Business.Services;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class ServiceController : ALISSBaseController
    {
        private readonly ServiceService _serviceService;
        private readonly UserProfileService _userProfileService;
        private readonly ElasticSearchService _elasticsearchService;

        public ServiceController()
        {
            _serviceService = new ServiceService();
            _userProfileService = new UserProfileService();
            _elasticsearchService = new ElasticSearchService();
        }

        // GET: Service
        public ActionResult Index(string searchTerm = "", int page = 1, bool unpublished = false, string orderBy = "createdon", int descending = 1)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            ServiceListingViewModel model = _serviceService.GetAllServices(UserManager, CurrentUser.Username, searchTerm, page, unpublished, orderBy, descending);
            model.Unpublished = unpublished;
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;
            return View(model);
        }

        public ActionResult AllServices(string searchTerm = "", int page = 1, string orderBy = "createdon", int descending = 1)
        {
            EditUserViewModel user = _userProfileService.GetUserForEdit(CurrentUser.UserProfileId, UserManager);
            if (!user.IsEditor)
            {
                orderBy = "servicename";
                descending = 0;
            }
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            ServiceListingViewModel model = _serviceService.GetUnfilteredServices(UserManager, CurrentUser.Username, searchTerm, page, orderBy, descending, user.IsAdmin);

            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;
            ViewBag.IsEditor = user.IsEditor;

            return View(model);
        }
    }
}