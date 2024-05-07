using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputWhereController : DataInputBaseController
    {
        [HttpGet]
        public ActionResult Index(Guid? id, bool claimError = false, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
               return RedirectToAction("Index", "AddToALISS");
            }

			if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
			{
                ViewBag.CurrentStep = DataInputStepsEnum.WhereTestStep;
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                WhereViewModel model;
                if (TempData.Peek("cancelData") != null)
                {
                    model = TempData["cancelData"] as WhereViewModel;
                    model = _dataInputService.RepopulateWhereModelForError(model);
                }
                else
                {
                    model = _dataInputService.GetWhereToEdit(id.Value);
                }
                ViewBag.ServiceId = model.ServiceId.ToString();
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                ViewBag.OrganisationId = model.OrganisationId;
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

                if (claimError)
                {
                    ViewBag.Error = true;
                    ModelState.AddModelError("Error", "Claim Error: Your data has been saved but there was a problem when trying to set up your claim.  Once your data is published, you can submit a new claim.");
                }

                model.SummaryType = summaryType;

                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Where");

                return View(model);
			}

			return RedirectToAction("Index", "AddToALISS");
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(WhereViewModel model, string submit)
		{
            ViewBag.CurrentStep = DataInputStepsEnum.WhereTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)ViewBag.CurrentStep });
            }

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

            if (!ModelState.IsValid)
			{
				model = _dataInputService.RepopulateWhereModelForError(model);
				ViewBag.Error = true;
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Where");
                return View(model);
			}

			string responseMessage = _dataInputService.EditWhere(model, CurrentUser.UserProfileId);
			
			if (responseMessage.Contains("Error"))
			{
				ViewBag.Error = true;
				ModelState.AddModelError("Error", responseMessage);
				return View(model);
			}

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
                return RedirectToAction("Index", "DataInputCategories", new { id = model.ServiceId });
            }
		}
	}
}