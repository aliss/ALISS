using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business.Enums;
using ALISS.Business.Validators;
using ALISS.Business.ViewModels.DataInput;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputWhoController : DataInputBaseController
    {
        readonly MinMaxValidator validator = new MinMaxValidator();
        [HttpGet]
        public ActionResult Index(Guid? id, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                ViewBag.CurrentStep = DataInputStepsEnum.WhoTestStep;
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                WhoViewModel model;
                if (TempData.Peek("cancelData") != null)
                {
                    model = TempData["cancelData"] as WhoViewModel;
                    model = _dataInputService.RepopulateWhoModelForError(model);
                }
                else
                {
                    model = _dataInputService.GetWhoForEdit(id.Value);
                }
                ViewBag.ServiceId = model.ServiceId.ToString();
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                ViewBag.OrganisationId = model.OrganisationId;
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                
                model.SummaryType = summaryType;

                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Who");

                return View(model);
            }

            return RedirectToAction("Index", "AddToALISS"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(WhoViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.WhoTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)ViewBag.CurrentStep });
            }

            if (!string.IsNullOrEmpty(model.SelectedCommunityGroups))
            {
                foreach (string commgroup in model.SelectedCommunityGroups.Split(','))
                {
                    if (!validator.IsValidMinMax(commgroup))
                    {
                        model = _dataInputService.RepopulateWhoModelForError(model);
                        ModelState.AddModelError("commgroup-" + commgroup.Split('|')[0] + "-minmax-container", "Please ensure that the age range entered for '" + model.ServiceCommunityGroups.Find(cg => cg.CommunityGroupId == int.Parse(commgroup.Split('|')[0])).Name + "' contains only numbers.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
                ViewBag.ServiceId = model.ServiceId.ToString();
                ViewBag.Error = true;
                model = _dataInputService.RepopulateWhoModelForError(model);
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Who");
                return View(model);
            }

            string responseMessage = _dataInputService.EditWho(model, CurrentUser.UserProfileId);
            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;

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
                if (_dataInputService.ShowAcessibility(model.ServiceId))
                {
                    return RedirectToAction("Index", "DataInputAccessibility", new { id = model.ServiceId });
                }
                else
                {
                    if (_dataInputService.isServiceSubmitted(model.ServiceId))
                    {
                        return RedirectToAction("Index", "DataInputMedia", new { id = model.ServiceId, summaryType = DataInputSummaryTypeEnum.Service });
                    }
                    else
                    {
                        return RedirectToAction("Index", "DataInputMedia", new { id = model.ServiceId, summaryType = DataInputSummaryTypeEnum.NotSubmitted });
                    }
                }
            }
        }
    }
}