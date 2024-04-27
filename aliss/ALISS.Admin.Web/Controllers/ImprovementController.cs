using ALISS.Business.Services;
using ALISS.Business.ViewModels.Improvement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin,Editor,ClaimedUser")]
    public class ImprovementController : ALISSBaseController
    {
        private readonly ImprovementService _improvementService;

        public ImprovementController()
        {
            _improvementService = new ImprovementService();
        }

        // GET: Improvement
        public ActionResult Index(int page = 1)
        {
            ImprovementListingViewModel model = _improvementService.GetSuggestedImprovements(UserManager, CurrentUser.Username, page);
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            return View(model);
        }

        public ActionResult ViewImprovement(Guid id)
        {
            ViewImprovementViewModel model = _improvementService.GetImprovementForView(id);
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            return View(model);
        }

        public ActionResult ResolveImprovement(Guid id)
        {
            _improvementService.ResolveImprovement(id);
            return RedirectToAction("Index");
        }
    }
}