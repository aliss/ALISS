using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputAccessibilityController : DataInputBaseController
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
                ViewBag.CurrentStep = DataInputStepsEnum.AccessibilityTestStep;
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                AccessibilityViewModel model;
                if (TempData.Peek("cancelData") != null)
                {
                    model = TempData["cancelData"] as AccessibilityViewModel;
                    model = _dataInputService.RepopulateAccessibilityModelForError(model);
                }
                else
                {
                    model = _dataInputService.GetAccessibilityForEdit(id.Value);
                }
                ViewBag.ServiceId = model.ServiceId.ToString();
                ViewBag.OrganisationId = model.OrganisationId;
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

                model.SummaryType = summaryType;

                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Accessibility");

                return View(model);
            }

            return RedirectToAction("Index", "AddToALISS");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AccessibilityViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.AccessibilityTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)ViewBag.CurrentStep });
            }

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

            if (!string.IsNullOrWhiteSpace(model.SelectedAccessibilityFeatures))
            {
                foreach (var feature in model.SelectedAccessibilityFeatures.Split('¬').ToList())
                {
                    string[] accessibilityLocationDetails = System.Text.RegularExpressions.Regex.Split(feature, @"\|\|");
                    string accessibilityFeatureId = accessibilityLocationDetails[1].Split('|')[0];
                    string additionalInfo = accessibilityLocationDetails[1].Split('|')[1];
                    if (additionalInfo.Length > 500)
                    {
                        model = _dataInputService.RepopulateAccessibilityModelForError(model);
                        string accessibilityFeatureName;
                        string locationName;
                        if (accessibilityLocationDetails[0] == "virtual")
                        {
                            accessibilityFeatureName = model.ServiceLocations.FirstOrDefault(l => l.LocationId == null).AccessibilityFeatures.Find(af => af.AccessibilityFeatureId == int.Parse(accessibilityFeatureId)).Name;
                            locationName = "Virtual";
                        }
                        else
                        {
                            var location = model.ServiceLocations.FirstOrDefault(l => l.LocationId == Guid.Parse(accessibilityLocationDetails[0]));
                            accessibilityFeatureName = location.AccessibilityFeatures.Find(af => af.AccessibilityFeatureId == int.Parse(accessibilityFeatureId)).Name;
                            locationName = location.Title;
                        }
                         
                        ModelState.AddModelError("accessibility-" + accessibilityFeatureId + "-moreinfo-container-" + accessibilityLocationDetails[0], "Additional Information for "+ locationName +" - '" + accessibilityFeatureName + "' has exceeded the character limit");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
                ViewBag.Error = true;
                model = _dataInputService.RepopulateAccessibilityModelForError(model);
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Accessibility");
                return View(model);
            }

            string responseMessage = _dataInputService.EditAccessibility(model, CurrentUser.UserProfileId);
            if (submit.Equals("Return To Summary"))
            {
                return RedirectToAction("Index", "DataInputSummary", new { id = model.ServiceId });
            }
            else if(submit.Equals("Return To Review"))
            {
                return RedirectToAction("Index", "ServiceReviewSummary", new { id = model.ServiceId });
            }
            else if (submit.Equals("Save and Exit"))
            {
                return RedirectToAction("Index", "AddToALISS");
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