using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputCategoriesController : DataInputBaseController
    {
        [HttpGet]
        public ActionResult Index(Guid? id, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                ViewBag.CurrentStep = DataInputStepsEnum.CategoriesTestStep;
                CategoriesViewModel model;
                if (TempData.Peek("cancelData") != null)
                {
                    model = TempData["cancelData"] as CategoriesViewModel;
                    model = _dataInputService.RepopulateCategoriesModelForError(model);
                }
                else
                {
                    model = _dataInputService.GetCategoriesForEdit(id.Value);
                }
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                ViewBag.ServiceId = model.ServiceId.ToString();
                ViewBag.OrganisationId = model.OrganisationId;
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

                model.SummaryType = summaryType;

                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Categories");

                return View(model);
            }

            return RedirectToAction("Index", "AddToALISS");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CategoriesViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.CategoriesTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)ViewBag.CurrentStep });
            }

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;

            if (!ModelState.IsValid)
            {
                model = _dataInputService.RepopulateCategoriesModelForError(model);
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
                ViewBag.Error = true;
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Categories");
                return View(model);
            }

            string responseMessage = _dataInputService.EditCategories(model, CurrentUser.UserProfileId);

            if (submit.Equals("Return To Summary"))
            {
                return RedirectToAction("Index", "DataInputSummary", new { id = model.ServiceId });
            }
            else if (submit.Equals("Return To Review"))
            {
                return RedirectToAction("Index", "ServiceReviewSummary", new { id = model.ServiceId });
            }
            else if (submit.Equals("Save and Exit"))
            {
                return RedirectToAction("Index", "AddToALISS");
            }
            else
            {
                return RedirectToAction("Index", "DataInputWho", new { id = model.ServiceId });
            }
        }
    }
}