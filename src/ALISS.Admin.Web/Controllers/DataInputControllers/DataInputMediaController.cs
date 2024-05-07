using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using ALISS.Admin.Web.ControllerAPI;
using ALISS.Admin.Web.Helpers;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.DataInput;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputMediaController : DataInputBaseController
    {
        const string AltTextErrorMessage = " Alt Text is a required field when uploading a Service Gallery Image. Please complete the alt text field for all uploaded images.";

        [HttpGet]
        public ActionResult Index(Guid? id, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.MediaTestStep;
            ViewBag.SummaryType = summaryType;

            ViewBag.UserId = CurrentUser.UserProfileId;
            ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Media");

            if (summaryType == DataInputSummaryTypeEnum.NotSubmitted)
            {
                if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
                {
                    return RedirectToAction("Index", "AddToALISS");
                }

                if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
                {
                    ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                    MediaViewModel model = TempData.Peek("cancelData") != null
                        ? TempData["cancelData"] as MediaViewModel
                        : _dataInputService.GetMediaForEdit(id.Value);
                    ViewBag.ServiceId = model.ServiceId.ToString();
                    ViewBag.OrganisationId = model.OrganisationId;
                    ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                    ViewBag.GalleryCount = _dataInputService.GetGalleryCount(id.Value);
                    ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                    ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                    ViewBag.ShowAccessibility = _dataInputService.ShowAcessibility(model.ServiceId);
                    return View(model);
                }
            }
            else if (summaryType == DataInputSummaryTypeEnum.Service || summaryType == DataInputSummaryTypeEnum.ServiceReview)
            {
                if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
                {
                    return RedirectToAction("Index", "AddToALISS");
                }

                if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
                {
                    ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(id.ToString());
                    MediaViewModel model = _dataInputService.GetServiceMediaForEdit(id.Value);
                    model.SummaryType = summaryType;
                    ViewBag.ServiceId = model.ServiceId.ToString();
                    ViewBag.OrganisationId = model.OrganisationId;
                    ViewBag.Submitted = _dataInputService.isServiceSubmitted(id.Value);
                    ViewBag.GalleryCount = _dataInputService.GetGalleryCount(id.Value);
                    ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                    ViewBag.ShowAccessibility = _dataInputService.ShowAcessibility(model.ServiceId);
                    return View(model);
                }
            }
            else if (summaryType == DataInputSummaryTypeEnum.Organisation)
            {
                if (id.HasValue && !_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id.Value))
                {
                    return RedirectToAction("Index", "AddToALISS");
                }

                if (id.HasValue && _organisationService.DoesOrganisationIdExist(id.Value))
                {
                    ViewBag.CurrentStep = DataInputStepsEnum.MediaTestStep;
                    ViewBag.BeenToSummary = true;
                    MediaViewModel model = _dataInputService.GetOrganisationMediaForEdit(id.Value);
                    ViewBag.OrganisationId = model.OrganisationId;
                    ViewBag.Submitted = _dataInputService.isOrganisationSubmitted(id.Value);
                    ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                    return View(model);
                }
            }

            return RedirectToAction("Index", "AddToALISS");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MediaViewModel model, string submit)
        {
            ViewBag.CurrentStep = DataInputStepsEnum.MediaTestStep;

            if (submit.Equals("Cancel the Form"))
            {
                TempData["cancelData"] = model;
                return RedirectToAction("Index", "DataInputCancel", new { id = model.ServiceId, prevStep = (int)ViewBag.CurrentStep });
            }

            ViewBag.UserId = CurrentUser.UserProfileId;
            ViewBag.IsAdmin = CurrentUser.IsAdmin;

            ViewBag.ServiceId = model.ServiceId.ToString();
            ViewBag.OrganisationId = model.OrganisationId;
            if (model.SummaryType != DataInputSummaryTypeEnum.Organisation)
            {
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId);
                ViewBag.ShowAccessibility = _dataInputService.ShowAcessibility(model.ServiceId);
            }
            else
            {
                ViewBag.Submitted = _dataInputService.isServiceSubmitted(model.ServiceId) || _dataInputService.isOrganisationSubmitted(model.OrganisationId);
                ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(model.OrganisationId);
            }
            ViewBag.BeenToSummary = _dataInputService.hasServiceBeenToSummary(model.ServiceId.ToString());
            ViewBag.SummaryType = model.SummaryType;
            ViewBag.GalleryCount = _dataInputService.GetGalleryCount(model.ServiceId);
            ViewBag.AddService = _dataInputService.isOrganisationSubmitted(model.OrganisationId);

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                model = _dataInputService.UpdateMediaModel(model);
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Media");
                return View(model);
            }

            if (Request.Files.Count > 0)
            {
                if ((model.SummaryType == DataInputSummaryTypeEnum.NotSubmitted && !_dataInputService.isOrganisationSubmitted(model.OrganisationId))
                    || ((model.SummaryType == DataInputSummaryTypeEnum.NotSubmitted || model.SummaryType == DataInputSummaryTypeEnum.Service || model.SummaryType == DataInputSummaryTypeEnum.ServiceReview) && _dataInputService.isOrganisationSubmitted(model.OrganisationId)))
                {
                    _dataInputService.EditServiceVideo(model, CurrentUser.UserProfileId, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
                    SaveServiceGallery(model);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                model = _dataInputService.UpdateMediaModel(model);
                ViewBag.Guidance = GenericHelpers.GetGuidanceContent("Media");
                return View(model);
            }

            if (submit.Equals("Return To Summary") && model.SummaryType == DataInputSummaryTypeEnum.Service)
            {
                return RedirectToAction("Index", "DataInputSummary", new { id = model.ServiceId });
            }
            else if (submit.Equals("Return To Summary") && model.SummaryType == DataInputSummaryTypeEnum.Organisation)
            {
                return RedirectToAction("Index", "DataInputOrganisationSummary", new { id = model.OrganisationId });
            }
            else if (submit.Equals("Return To Review"))
            {
                return RedirectToAction("Index", "ServiceReviewSummary", new { id = model.ServiceId });
            }
            else if (submit.Equals("Save and Exit"))
            {
                return RedirectToAction("Index", "AddToALISS");
            }
            else if (submit.Equals("Next") && model.SummaryType == DataInputSummaryTypeEnum.Organisation)
            {
                return RedirectToAction("Index", "DataInputOrganisationSummary", new { id = model.OrganisationId });
            }
            else
            {
                return RedirectToAction("Index", "DataInputSummary", new { id = model.ServiceId });
            }
        }

        public void SaveOrganisationLogo(MediaViewModel model)
        {
            if (Request.Files["OrganisationLogo"].ContentLength > 0)
            {
                HttpPostedFileBase organisationLogo = Request.Files["OrganisationLogo"];
                _blobStorageService = new BlobStorageService();
                if (!_blobStorageService.IsValidImage(organisationLogo, out string validationError, "Organisation Logo "))
                {
                    ViewBag.Error = true;
                    ModelState.AddModelError("OrganisationLogo", validationError);
                }

                if (ModelState.IsValid)
                {
                    string organisationLogoUrl = _blobStorageService.UploadLogoToBlobStorage(model.OrganisationId, organisationLogo);
                    _dataInputService.EditOrganisationLogo(model, CurrentUser.UserProfileId, organisationLogoUrl);
                }
            }
            else
            {
                _dataInputService.EditOrganisationLogo(model, CurrentUser.UserProfileId, null);
            }
        }

        public void SaveServiceLogo(MediaViewModel model)
        {
            if (Request.Files["ServiceLogo"].ContentLength > 0)
            {
                HttpPostedFileBase serviceLogo = Request.Files["ServiceLogo"];
                _blobStorageService = new BlobStorageService();
                if (!_blobStorageService.IsValidImage(serviceLogo, out string validationError, "Service Logo "))
                {
                    ViewBag.Error = true;
                    ModelState.AddModelError("ServiceLogo", validationError);
                }

                if (ModelState.IsValid)
                {
                     string serviceLogoUrl = _blobStorageService.UploadLogoToBlobStorage(model.ServiceId, serviceLogo);
                    _dataInputService.EditServiceLogo(model, CurrentUser.UserProfileId, serviceLogoUrl);
                }
            }
            else
            {
                _dataInputService.EditServiceLogo(model, CurrentUser.UserProfileId, null);
            }
        }

        public void SaveServiceGallery(MediaViewModel model)
        {
            if (string.IsNullOrEmpty(model.ServiceGallery1AltText) && (model.ServiceGalleryImageId1 != Guid.Empty || Request.Files["Gallery1"].ContentLength > 0))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("ServiceGallery1AltText", "Service Gallery Image 1" + AltTextErrorMessage);
            }

            if (Request.Files["Gallery1"].ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, model.ServiceGalleryImage1, DataInputServiceGalleryEnum.Gallery1, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
                }
            }
            else
            {
                _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, null, DataInputServiceGalleryEnum.Gallery1, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
            }

            //Gallery Image 2
            if (string.IsNullOrEmpty(model.ServiceGallery2AltText) && (model.ServiceGalleryImageId2 != Guid.Empty || Request.Files["Gallery2"].ContentLength > 0))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("ServiceGallery2AltText", "Service Gallery Image 2" + AltTextErrorMessage);
            }

            if (Request.Files["Gallery2"].ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, model.ServiceGalleryImage2, DataInputServiceGalleryEnum.Gallery2, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
                    }
                }
            else
            {
                _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, null, DataInputServiceGalleryEnum.Gallery2, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
            }

            //Gallery Image 3
            if (string.IsNullOrEmpty(model.ServiceGallery3AltText) && (model.ServiceGalleryImageId3 != Guid.Empty || Request.Files["Gallery3"].ContentLength > 0))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("ServiceGallery3AltText", "Service Gallery Image 3" + AltTextErrorMessage);
            }

            if (Request.Files["Gallery3"].ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, model.ServiceGalleryImage3, DataInputServiceGalleryEnum.Gallery3, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
                }
            }
            else
            {
                _dataInputService.EditServiceGallery(model, CurrentUser.UserProfileId, null, DataInputServiceGalleryEnum.Gallery3, _userProfileService.IsAdmin(CurrentUser.UserProfileId, UserManager));
            }
        }
    }
}