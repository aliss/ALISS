using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputSummaryController : DataInputBaseController
    {
        [HttpGet]
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            TempData.Clear();

            SummaryViewModel model;
            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                ViewBag.CurrentStep = DataInputStepsEnum.SummaryTestStep;
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                ViewBag.SummaryType = DataInputSummaryTypeEnum.Service;
                ViewBag.SuggestedService = _dataInputService.IsServiceSuggested(id.Value);
                ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
                ViewBag.IsAdmin = UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());

                using (ALISSContext dc = new ALISSContext())
                {
                    Service service = dc.Services.Find(id);
                    Organisation organisation = dc.Organisations.Find(service.OrganisationId);

                    model = _dataInputService.GetSummary(service.ServiceId);
                    model.SummaryType = _dataInputService.isServiceSubmitted(model.ServiceId) ? DataInputSummaryTypeEnum.Service : DataInputSummaryTypeEnum.NotSubmitted;

                    ViewBag.ServiceId = service.ServiceId.ToString();
                    ViewBag.OrganisationId = service.OrganisationId;
                    ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(service.OrganisationId);
                    ViewBag.OrganisationPublished = _organisationService.IsOrganisationPublished(service.OrganisationId);
                    ViewBag.Published = service.Published;
                    ViewBag.IsLeadClaimant = service.ClaimedUser != null && service.ClaimedUserId == CurrentUser.UserProfileId;
                    ViewBag.IsClaimant = dc.ServiceClaimUsers.Where(x => x.ServiceId == id && x.ClaimedUserId == CurrentUser.UserProfileId).Any();
                    ViewBag.IsLeadClaimantOfParentOrganisation = service.Organisation.ClaimedUser != null && service.Organisation.ClaimedUserId == CurrentUser.UserProfileId;

                    if (service.LastEditedStep < (int)DataInputStepsEnum.SummaryTestStep)
                    {
                        _dataInputService.updateServiceStepToSummary(id.Value);
                    }

                    TempData["serviceId"] = service.ServiceId.ToString();
                }

                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
                TempData["newServiceOrgId"] = model.OrganisationId;
                ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Summary");

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "AddToALISS");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SummaryViewModel model, string submit)
        {
            TempData.Remove("newOrganisation");
            TempData.Remove("newServiceOrgId");

            if (submit.Equals("Save and Exit"))
            {
                return RedirectToAction("Index", "AddToALISS");
            }
            else if (submit.Equals("Submit") || submit.Equals("Submit For Review") || (submit.Equals("Save") && !_serviceService.IsServiceSubmitted(model.ServiceId)))
            {
                if (_dataInputService.isOrganisationSubmitted(model.OrganisationId))
                {
                    if (_dataInputService.IsServiceSuggested(model.ServiceId))
                    {
                        _dataInputService.SubmitSuggestedService(model.ServiceId, CurrentUser);
                    }
                    else
                    {
                        _dataInputService.submitService(model.ServiceId, CurrentUser.Username, UserManager);
                    }
                }
                else
                {
                    _dataInputService.submitServiceAndOrganisation(model.ServiceId, CurrentUser.Username, UserManager);
                }

                return RedirectToAction("Index", "DataInputConfirmation", new { id = model.ServiceId });
            }
            else if (submit.Equals("Save"))
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    List<MediaGallery> serviceGallery = dc.MediaGallery.Where(m => m.ServiceId == model.ServiceId && !m.Approved).ToList();

                    if (serviceGallery.Any())
                    {
                        _dataInputService.submitService(model.ServiceId, CurrentUser.Username, UserManager);
                    }
                }

                return RedirectToAction("Index", "Service");
                
            }
            else if (submit.Equals("Approve & Publish"))
            {
                ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
                _dataInputService.ApproveSuggestedService(model.ServiceId, UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString()));
                return RedirectToAction("Index", "ServiceSuggestions");
            }
            else if (submit.Equals("Cancel the Form"))
            {
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)DataInputStepsEnum.SummaryTestStep });
            }
            else if (submit.Equals("Add Service"))
            {
                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
                TempData["newServiceOrgId"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId).OrganisationId;
                return RedirectToAction("Index", "DataInputService", new { id = Guid.Empty });
            }

            return RedirectToAction("Index", "AddToALISS");
        }
    }
}