using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Organisation;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputCancelController : DataInputBaseController
    {
        // GET: DataInputCancel
        public ActionResult Index(Guid? id, int prevStep)
        {
            if (id.HasValue && id.Value != Guid.Empty && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            CancelApplicationViewModel model;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = _dataInputService.CancelServiceApplication(id.Value, prevStep);
            }
            else
            {
                model = new CancelApplicationViewModel() 
                {
                    PreviousStep = prevStep
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CancelApplicationViewModel model)
        {
            string organisationErrorMessage = "";
            Guid organisationId = _serviceService.DeleteService(model.ServiceId, out string serviceErrorMessage);

            if (serviceErrorMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", serviceErrorMessage);
                return View(model);
            }

            if (organisationId != Guid.Empty && string.IsNullOrWhiteSpace(serviceErrorMessage))
            {
                if (_organisationService.GetNumberOfServicesForOrganisation(organisationId) == 0)
                {
                    _organisationService.DeleteOrganisation(organisationId, out organisationErrorMessage);
                }
            }

            if (organisationErrorMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", organisationErrorMessage);
                return View(model);
            }

            return RedirectToAction("Index", "AddToALISS");
        }
    }
}