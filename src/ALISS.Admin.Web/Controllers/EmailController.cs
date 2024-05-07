using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.BulkEmail;

namespace ALISS.Admin.Web.Controllers
{
    public class EmailController : ALISSBaseController
    {
        public readonly Business.Services.EmailService _emailService = new Business.Services.EmailService();
        public readonly ImprovementService _improvementService = new ImprovementService();
        public readonly OrganisationService _organisationService = new OrganisationService();
        public readonly ServiceService _serviceService = new ServiceService();
        [HttpGet]
        public ActionResult ImprovementEmail(Guid? Id)
        {
            if (!ViewBag.IsAdmin || !Id.HasValue)
            {
                return RedirectToAction("Index", "Improvement");
            }

            if (_improvementService.IsOrganisationImprovement(Id.Value))
            {
                ViewBag.ImprovementName = _improvementService.OrganisationImprovementName(Id.Value);
            }
            else
            {
                ViewBag.ImprovementName = _improvementService.ServiceImprovementName(Id.Value);
            }

            CustomEmailViewModel model = new CustomEmailViewModel
            {
                Subject = "RE: Improvement to " + ViewBag.ImprovementName,
                Body = string.Empty,
                Id = Id
            };

            ViewBag.ImprovementEmail = _improvementService.GetImprovementRequesterEmail(Id.Value);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ImprovementEmail(CustomEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }

            _emailService.SendCustomEmail(_improvementService.GetImprovementRequesterEmail(model.Id.Value), model);
            return RedirectToAction("Index", "Home", null);
        }
    }
}