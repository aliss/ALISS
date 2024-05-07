using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.ServiceReviews;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class ServiceReviewsController : ALISSBaseController
    {
        private readonly ServiceService _serviceService = new ServiceService();
        // GET: ServiceReviews
        public ActionResult Index(string searchTerm = "", int page = 1, string orderBy = "status", int descending = 1, string reviewedService = "")
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            if(!string.IsNullOrEmpty(reviewedService))
            {
                ViewBag.ReviewedService = reviewedService;
            }

            ServiceReviewListingViewModel model = _serviceService.GetAllServiceReviews(UserManager, CurrentUser.Username, searchTerm, page, orderBy, descending);
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;
            return View(model);
        }

        public ActionResult Deprioritised(string searchTerm = "", int page = 1, string orderBy = "deprioritisedon", int descending = 0, string reviewedService = "")
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            if (!string.IsNullOrEmpty(reviewedService))
            {
                ViewBag.ReviewedService = reviewedService;
            }

            DeprioritisedServicesViewModel model = _serviceService.GetDeprioritisedServices(searchTerm, page, orderBy, descending);
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;
            return View(model);
        }
    }
}