using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.Validators;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using Ganss.Xss;
using Microsoft.AspNet.Identity;
using WebApplication.Common;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class ServiceReviewSummaryController : DataInputBaseController
    {
        ElasticSearchService _elasticSearchService = new ElasticSearchService();
        private readonly HtmlSanitizer _sanitizer, _lengthSanitizer;
        private readonly int _descriptionMinimumLength = ConfigurationManager.AppSettings["Validation:DescriptionMin"] != null 
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMin"].ToString()) 
            : 50;
        private readonly int _descriptionMaximumLength = ConfigurationManager.AppSettings["Validation:DescriptionMax"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMax"].ToString())
            : 1000;
        private readonly LocationValidator LocationValidator = new LocationValidator();

        public ServiceReviewSummaryController()
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

        // GET: ServiceReviewSummary
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue && !_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "ServiceReviews");
            }

            SummaryViewModel model;
            if (id.HasValue && _serviceService.DoesServiceIdExist(id.Value))
            {
                model = _dataInputService.GetSummary(id.Value);
                model.SummaryType = DataInputSummaryTypeEnum.ServiceReview;
                ViewBag.SummaryType = model.SummaryType;
                ViewBag.ReviewSummary = true;

                int descriptionLength = _lengthSanitizer.Sanitize(model.ServiceDescription).Length;
                if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
                {
                    ModelState.AddModelError("ServiceDescription", $"The service description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
                }

                string locationResponse = LocationValidator.ValidateSummaryModel(model.HowServiceAccessed.ToLower(), model.SelectedServiceAreas, model.SelectedLocations);
                if (locationResponse.Contains("service"))
                {
                    if (locationResponse.Contains("location"))
                    {
                        ModelState.AddModelError("ServiceLocation", locationResponse);
                    }
                    
                    ModelState.AddModelError("ServiceRegion", locationResponse);
                }

                foreach (ModelValidationResult item in ModelValidator.GetModelValidator(ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()), base.ControllerContext).Validate(null))
                {
                    ModelState.AddModelError(item.MemberName, item.Message);
                }

                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    ViewBag.Error = true;
                }

                return View(model);
            }

            return RedirectToAction("Index", "ServiceReviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SummaryViewModel model) 
        {
            TempData.Clear();

            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(model.ServiceId);
                ServiceReview serviceReview = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == model.ServiceId);
                serviceReview.LastReviewedUserId = CurrentUser.UserProfileId;
                serviceReview.LastReviewedDate = DateTime.UtcNow.Date;
                serviceReview.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                service.Deprioritised = false;

                ApplicationUser user = UserManager.FindByName(CurrentUser.Username);
                UserProfile userProfile = dc.UserProfiles.FirstOrDefault(u => u.Username == CurrentUser.Username);
                bool isEditor = UserManager.IsInRole(user.Id, RolesEnum.Editor.ToString());
                bool isBasic = UserManager.IsInRole(user.Id, RolesEnum.BaseUser.ToString()) || UserManager.IsInRole(user.Id, RolesEnum.ClaimedUser.ToString());
                bool isAdmin = !isEditor && !isBasic;

                if (!isAdmin)
                {
                    List<MediaGallery> MediaGallery = dc.MediaGallery.Where(s => s.ServiceId == model.ServiceId && !s.Approved).ToList();
                    if (MediaGallery.Count > 0)
                    {
                        foreach (MediaGallery media in MediaGallery)
                        {
                            _emailService.SendMediaForApproval(userProfile.Username, media.UploadUserId.Value, service.OrganisationId, service.Organisation.Name, service.Organisation.Published, service.ServiceId, service.Name, service.Published, media.MediaGalleryId, media.Type, media.MediaUrl, media.AltText, media.Caption);
                        }
                    }
                }

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = model.ServiceId,
                    UserProfileId = CurrentUser.UserProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);
                dc.SaveChanges();

                _elasticSearchService.AddServiceToElasticSearch(model.ServiceId);
            }

            ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
            return UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString())
                ? RedirectToAction("Deprioritised", "ServiceReviews", new { reviewedService = model.ServiceName })
                : (ActionResult)RedirectToAction("Index", "ServiceReviews", new { reviewedService = model.ServiceName });
        }
    }
}