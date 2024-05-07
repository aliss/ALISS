using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;
using Ganss.Xss;
using WebApplication.Common;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputOrganisationController : DataInputBaseController
    {
        private readonly HtmlSanitizer _sanitizer, _lengthSanitizer;
        private readonly int _descriptionMinimumLength = ConfigurationManager.AppSettings["Validation:DescriptionMin"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMin"].ToString())
            : 50;
        private readonly int _descriptionMaximumLength = ConfigurationManager.AppSettings["Validation:DescriptionMax"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMax"].ToString())
            : 1000;

        public DataInputOrganisationController()
        {
            _sanitizer = new HtmlSanitizer();
            _lengthSanitizer = new HtmlSanitizer();

            _lengthSanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");

            _sanitizer.AllowedAtRules.Clear();
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedClasses.Clear();
            _sanitizer.AllowedCssProperties.Clear();
            _sanitizer.AllowedSchemes.Clear();
            _lengthSanitizer.AllowedAtRules.Clear();
            _lengthSanitizer.AllowedAttributes.Clear();
            _lengthSanitizer.AllowedClasses.Clear();
            _lengthSanitizer.AllowedCssProperties.Clear();
            _lengthSanitizer.AllowedSchemes.Clear();

            _sanitizer.AllowDataAttributes = false;
            _lengthSanitizer.AllowDataAttributes = false;

            _sanitizer.KeepChildNodes = true;
            _lengthSanitizer.KeepChildNodes = true;
        }

        [HttpGet]
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty && !_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            ViewBag.CurrentStep = DataInputStepsEnum.OrganisationTestStep;
            if (TempData.Peek("serviceId") != null)
            {
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(TempData.Peek("serviceId").ToString());
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(Guid.Parse(TempData.Peek("serviceId").ToString()));
                ViewBag.ServiceId = TempData.Peek("serviceId").ToString();
            }
            else if (id.HasValue && id.Value != Guid.Empty && _dataInputService.isOrganisationSubmitted(id.Value))
            {
                ViewBag.BeenToSummary = true;
                ViewBag.Submitted = _dataInputService.isOrganisationSubmitted(id.Value);
            }

            if(id.HasValue && _organisationService.DoesOrganisationIdExist(id.Value))
            {
                ViewBag.OrganisationExists = true;
            }

            ViewBag.OrganisationSubmitted = false;
            OrganisationViewModel model = new OrganisationViewModel();

            if (id.HasValue && (TempData.Peek("newOrganisation") != null || TempData.Peek("cancelData") != null || id.Value != Guid.Empty))
            {
                if (id.Value != Guid.Empty)
                {
                    ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(id.Value);
                    model = _dataInputService.GetOrganisationForEdit(id.Value);
                }
                else if (!(TempData.Peek("newOrganisation") == null))
                {
                    model = TempData["newOrganisation"] as OrganisationViewModel;
                }
                else if (!(TempData.Peek("cancelData") == null))
                {
                    model = TempData["cancelData"] as OrganisationViewModel;
                }
            }
            else
            {
                TempData.Clear();
                Guid newOrganisationId = Guid.NewGuid();
                while (_organisationService.DoesOrganisationIdExist(newOrganisationId))
                {
                    newOrganisationId = Guid.NewGuid();
                }

                model = _dataInputService.GetEmptyOrganisationModel(newOrganisationId, true);
            }

            ViewBag.OrganisationId = model.OrganisationId;

            ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Organisation Details");

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index(OrganisationViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.OrganisationTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                Guid serviceId = Guid.Empty;
                if (TempData.Peek("serviceId") != null)
                {
                    serviceId = Guid.Parse(TempData["serviceId"].ToString());
                }
                else if (TempData.Peek("currentService") != null)
                {
                    ServiceViewModel service = TempData["currentService"] as ServiceViewModel;
                    serviceId = service.ServiceId;
                }
                return RedirectToAction("Index", "DataInputCancel", new { id = serviceId, prevStep = (int)ViewBag.CurrentStep });
            }

            if(TempData.Peek("serviceId") != null)
            {
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(TempData.Peek("serviceId").ToString());
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(Guid.Parse(TempData.Peek("serviceId").ToString()));
                ViewBag.ServiceId = TempData.Peek("serviceId").ToString();
            }
            
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.OrganisationSubmitted = _organisationService.DoesOrganisationIdExist(model.OrganisationId) && _dataInputService.isOrganisationSubmitted(model.OrganisationId);

            int descriptionLength = _lengthSanitizer.Sanitize(model.OrganisationDescription).Length;
            if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
            {
                ModelState.AddModelError("OrganisationDescription", $"The organisation description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.RemoveDuplicateErrorMessages();
                ViewBag.Error = true;
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Organisation Details");
                return View(model);
            }

            model.OrganisationDescription = _sanitizer.Sanitize(model.OrganisationDescription);

            string orgResponseMessage = "";
            if (!_organisationService.DoesOrganisationIdExist(model.OrganisationId))
            {
                if (_organisationService.DoesOrganisationExist(model.OrganisationName))
                {
                    ViewBag.Error = true;
                    ModelState.AddModelError("OrganisationName", $"The organisation {model.OrganisationName} already exists, please choose another name.");
                    ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Organisation Details");
                    return View(model);
                }

                TempData["newOrganisation"] = model;
            }
            else
            {
                //Refactor to add parser to editOrganisationViewModel
                EditOrganisationViewModel organisationToEdit = new EditOrganisationViewModel()
                {
                    OrganisationId = model.OrganisationId,
                    Name = model.OrganisationName,
                    Description = model.OrganisationDescription,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Url = model.Url,
                    Facebook = model.Facebook,
                    Twitter = model.Twitter,
                    Instagram = model.Instagram
                };

                orgResponseMessage = _organisationService.EditOrganisation(organisationToEdit, CurrentUser.UserProfileId);
            }

            if (orgResponseMessage.Contains("Error") )
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", orgResponseMessage);
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Organisation Details");
                return View(model);
            }

            if (TempData.Peek("currentService") != null)
            {
                ServiceViewModel service = TempData["currentService"] as ServiceViewModel;
                if (service.OrganisationId == model.OrganisationId)
                {
                    if (submit.Equals("Return To Summary"))
                    {
                        if (ViewBag.OrganisationSubmitted)
                        {
                            return RedirectToAction("Index", "DataInputOrganisationSummary", new { id = model.OrganisationId });
                        }
                        else
                        {
                            return RedirectToAction("Index", "DataInputSummary", new { id = service.ServiceId });
                        }
                    }
                    else if (submit.Equals("Save and Exit"))
                    {
                        return RedirectToAction("Index", "AddToALISS");
                    }
                    else if (submit.Equals("Next") && ViewBag.OrganisationSubmitted)
                    {
                        return RedirectToAction("Index", "DataInputMedia", new { id = model.OrganisationId, summaryType = DataInputSummaryTypeEnum.Organisation });
                    }
                    else
                    {
                        return RedirectToAction("Index", "DataInputService", new { id = service.ServiceId });
                    }
                }
            }
            else if (TempData["serviceId"] != null)
            {
                Guid serviceId = Guid.Parse(TempData["serviceId"].ToString());

                if (submit.Equals("Return To Summary"))
                {
                    if (ViewBag.OrganisationSubmitted)
                    {
                        return RedirectToAction("Index", "DataInputOrganisationSummary", new { id = model.OrganisationId });
                    }
                    else
                    {
                        return RedirectToAction("Index", "DataInputSummary", new { id = serviceId });
                    }
                }
                else if (submit.Equals("Save and Exit"))
                {
                    return RedirectToAction("Index", "AddToALISS");
                }
                else if (submit.Equals("Next") && ViewBag.OrganisationSubmitted)
                {
                    return RedirectToAction("Index", "DataInputMedia", new { id = model.OrganisationId, summaryType = DataInputSummaryTypeEnum.Organisation });
                }
                else
                {
                    return RedirectToAction("Index", "DataInputService", new { id = serviceId });
                }
            }

            if (submit.Equals("Next") && ViewBag.OrganisationSubmitted)
            {
                return RedirectToAction("Index", "DataInputMedia", new { id = model.OrganisationId, summaryType = DataInputSummaryTypeEnum.Organisation });
            }
            else if (submit.Equals("Return To Summary") && ViewBag.OrganisationSubmitted)
            {
                return RedirectToAction("Index", "DataInputOrganisationSummary", new { id = model.OrganisationId });
            }
            else
            {
                return RedirectToAction("Index", "DataInputService");
            }
        }
    }
}