using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;
using Ganss.Xss;
using WebApplication.Common;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputServiceController : DataInputBaseController
    {
        private readonly HtmlSanitizer _sanitizer, _lengthSanitizer;
        private readonly int _descriptionMinimumLength = ConfigurationManager.AppSettings["Validation:DescriptionMin"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMin"].ToString())
            : 50;
        private readonly int _descriptionMaximumLength = ConfigurationManager.AppSettings["Validation:DescriptionMax"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMax"].ToString())
            : 1000;

        public DataInputServiceController()
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
        public ActionResult Index(Guid? id, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            if (id.HasValue && id.Value != Guid.Empty && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            if(summaryType == DataInputSummaryTypeEnum.SuggestedService && TempData.Peek("newServiceOrgId") != null) 
            {
                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(Guid.Parse(TempData.Peek("newServiceOrgId").ToString()));
            }

            ViewBag.CurrentStep = DataInputStepsEnum.ServiceTestStep;
            if (id.HasValue)
            {
                ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
            }

            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                ViewBag.ServiceExists = true;
            }

            ServiceViewModel model;

            if (TempData.Peek("cancelData") != null)
            {
                model = TempData["cancelData"] as ServiceViewModel;
            }
            else if (id.HasValue && id.Value != Guid.Empty)
            {
                model = _dataInputService.GetServiceForEdit(id.Value);
                model.organisationModel = new OrganisationViewModelNoValidation(_dataInputService.GetOrganisationForEdit(model.OrganisationId));
            }
            else
            {
                Guid newServiceId = Guid.NewGuid();
                while (_serviceService.DoesServiceIdExist(newServiceId))
                {
                    newServiceId = Guid.NewGuid();
                }
                model = _dataInputService.GetEmptyServiceModel(newServiceId, true);
                model.organisationModel = new OrganisationViewModelNoValidation(TempData.Peek("newOrganisation") as OrganisationViewModel);
            }

            if(model.OrganisationId != null && model.OrganisationId != Guid.Empty)
            {
                ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
            }
            else if(model.organisationModel.OrganisationId != null && model.organisationModel.OrganisationId != Guid.Empty)
            {
                ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(model.organisationModel.OrganisationId);
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.organisationModel.OrganisationId);
            }
            else if(TempData.Peek("newServiceOrgId") != null)
            {
                Guid orgId = Guid.Parse(TempData.Peek("newServiceOrgId") as string);
                if(orgId != Guid.Empty)
                {
                    ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(orgId);
                    ViewBag.AddService = _dataInputService.isOrganisationSubmitted(orgId);
                }
            }

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.CurrentService = model;

            model.SummaryType = summaryType;

            ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Service Details");

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ServiceViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.ServiceTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                Guid serviceId = Guid.Empty;
                if(_serviceService.DoesServiceIdExist(model.ServiceId))
                {
                    serviceId = model.ServiceId;
                }
                return RedirectToAction("Index", "DataInputCancel", new { id = serviceId, prevStep = (int)ViewBag.CurrentStep });
            }

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.CurrentService = model;
            ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
            ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
            ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

            OrganisationViewModel organisation;
            string responseMessage = " ";

            if (model.OrganisationId == Guid.Empty)
            {
                organisation = TempData.Peek("newOrganisation") as OrganisationViewModel;
            }
            else
            {
                organisation = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
            }

            if (!model.UseOrganisationDescription)
            {
                int descriptionLength = _lengthSanitizer.Sanitize(model.ServiceDescription).Length;
                if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
                {
                    ModelState.AddModelError("ServiceDescription", $"The service description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.RemoveDuplicateErrorMessages();
                ViewBag.Error = true;
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Service Details");
                return View(model);
            }

            model = _dataInputService.UseOrganisationValues(model, organisation);

            EditServiceViewModel serviceToAdd = new EditServiceViewModel()
            {
                OrganisationId = organisation.OrganisationId,
                ServiceId = model.ServiceId,
                Name = model.ServiceName,
                ServiceRepresentative = model.ServiceRepresentative,
                ServiceRepresentativeName = model.ServiceRepresentativeName,
                ServiceRepresentativePhone = model.ServiceRepresentativePhone,
                ServiceRepresentativeRole = model.ServiceRepresentativeRole,
                Summary = model.ServiceSummary,
                Description = model.ServiceDescription,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Url = model.Url,
                ReferralUrl = model.ReferralUrl,
                Facebook = model.Facebook,
                Twitter = model.Twitter,
                Instagram = model.Instagram,
                Slug = model.Slug,
            };

            if (!_serviceService.DoesServiceIdExist(model.ServiceId))
            {
                if (!_organisationService.DoesOrganisationIdExist(organisation.OrganisationId))
                {
                    AddOrganisationViewModel organisationToAdd = new AddOrganisationViewModel()
                    {
                        OrganisationId = organisation.OrganisationId,
                        Name = organisation.OrganisationName,
                        OrganisationRepresentative = organisation.OrganisationRepresentative,
                        OrganisationRepresentativeName = organisation.OrganisationRepresentativeName,
                        OrganisationRepresentativePhone = organisation.OrganisationRepresentativePhone,
                        OrganisationRepresentativeRole = organisation.OrganisationRepresentativeRole,
                        Description = organisation.OrganisationDescription,
                        PhoneNumber = organisation.PhoneNumber,
                        Email = organisation.Email,
                        Url = organisation.Url,
                        Facebook = organisation.Facebook,
                        Twitter = organisation.Twitter,
                        Instagram = organisation.Instagram,
                        Slug = organisation.Slug,
                    };

                    responseMessage += _dataInputService.AddOrganisation(organisationToAdd, CurrentUser.UserProfileId, UserManager, out Guid organisationId) + " ";
                }

                responseMessage += _dataInputService.AddService(serviceToAdd, CurrentUser.UserProfileId, (int)DataInputStepsEnum.ServiceTestStep, model.SummaryType);
            }
            else
            {
                _dataInputService.EditService(serviceToAdd, CurrentUser.UserProfileId);
            }

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", responseMessage);

                if (!responseMessage.Contains("Claim Error"))
                {
                    ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Service Details");
                    return View(model);
                }
            }

            TempData.Remove("newOrganisation");

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
                return RedirectToAction("Index", "DataInputWhere", new { id = model.ServiceId, claimError = responseMessage.Contains("Claim Error") });
            }
        }

        [HttpGet]
        public ActionResult SuggestService(Guid? id) 
        {
            TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(id.Value);
            TempData["newServiceOrgId"] = id.Value;
            return RedirectToAction("Index", new { id = Guid.Empty, summaryType = DataInputSummaryTypeEnum.SuggestedService });
        }
    }
}