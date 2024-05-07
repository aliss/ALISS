using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Service;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers
{
    public class ServiceSuggestionsController : ALISSBaseController
    {
        private readonly ServiceService _serviceService = new ServiceService();
        // GET: ServiceSuggestions
        public ActionResult Index(string searchTerm = "", int page = 1, string orderBy = "createdon", int descending = 1)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            ServiceListingViewModel model = _serviceService.GetSuggestedServices(UserManager, CurrentUser.Username, searchTerm, page, false, orderBy, descending);
            model.Unpublished = false;
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;

            ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
            ViewBag.IsAdmin = UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());
            return View(model);
        }
    }
}