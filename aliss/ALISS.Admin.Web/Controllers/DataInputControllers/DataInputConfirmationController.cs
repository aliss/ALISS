using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputConfirmationController : DataInputBaseController
    {
        // GET: DataInputConfirmation
        public ActionResult Index(Guid? id)
        {
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.PublicURL = publicBaseURL;

            TempData.Clear();
            ConfirmationViewModel model = null;

            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                model = _dataInputService.GetConfirmationModel(id.Value);
                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
                TempData["newServiceOrgId"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId).OrganisationId;
                ViewBag.ServiceId = model.ServiceId;
                ViewBag.OrganisationId = model.OrganisationId;
            }

            return View(model);
        }

        public ActionResult NextForm(string next)
        {
            if (next.Equals("sameOrg"))
            {
                return RedirectToAction("Index", "DataInputService", new { id = Guid.Empty });
            }

            TempData.Clear();
            return RedirectToAction("Index", "DataInputOrganisation", new { id = Guid.Empty });
        }
    }
}