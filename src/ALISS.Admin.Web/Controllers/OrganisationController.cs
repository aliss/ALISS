using ALISS.Business;
using ALISS.Business.PresentationTransferObjects.Location;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.Location;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Ganss.Xss;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication.Common;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class OrganisationController : ALISSBaseController
    {
        private readonly OrganisationService _organisationService;
        private readonly ServiceService _serviceService;
        private readonly LocationService _locationService;
        private readonly ElasticSearchService _elasticsearchService;
        private readonly BlobStorageService _blobStorageService;
        private readonly UserProfileService _userProfileService;
        private readonly Business.Services.EmailService _emailService;
        private readonly HtmlSanitizer _sanitizer, _lengthSanitizer;
        private readonly int _descriptionMinimumLength = ConfigurationManager.AppSettings["Validation:DescriptionMin"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMin"].ToString())
            : 50;
        private readonly int _descriptionMaximumLength = ConfigurationManager.AppSettings["Validation:DescriptionMax"] != null
            ? int.Parse(ConfigurationManager.AppSettings["Validation:DescriptionMax"].ToString())
            : 1000;
        private readonly ClaimService _claimService;

        public OrganisationController()
        {
            _organisationService = new OrganisationService();
            _locationService = new LocationService();
            _serviceService = new ServiceService();
            _elasticsearchService = new ElasticSearchService();
            _blobStorageService = new BlobStorageService();
            _userProfileService = new UserProfileService();
            _emailService = new Business.Services.EmailService();

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
            _claimService = new ClaimService();
        }


        // GET: Organisation
        public ActionResult Index(string searchTerm = "", int page = 1, bool unpublished = false, string orderBy = "createdon", int descending = 1)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;

            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            OrganisationListingViewModel model = _organisationService.GetOrganisations(UserManager, CurrentUser.Username, searchTerm, page, unpublished, orderBy, descending);
            model.Unpublished = unpublished;
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            return View(model);
        }

        public ActionResult AllOrganisations(string searchTerm = "", int page = 1, string orderBy = "createdon", int descending = 1)
        {
            EditUserViewModel userProfile = _userProfileService.GetUserForEdit(CurrentUser.UserProfileId, UserManager);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Page = page;
            ViewBag.OrderBy = orderBy;
            ViewBag.Descending = descending;
            ViewBag.isEditor = userProfile.IsEditor;

            OrganisationListingViewModel model = _organisationService.GetAllOrganisations(UserManager, CurrentUser.Username, searchTerm, page, orderBy, descending, userProfile.IsAdmin);
            
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            
            return View(model);
        }

        public ActionResult AddOrganisation()
        {
            AddOrganisationViewModel model = new AddOrganisationViewModel();
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrganisation(AddOrganisationViewModel model)
        {
            if (model.OrganisationRepresentative == true && model.OrganisationAcceptDataStandards == false)
            {
                ModelState.AddModelError("AcceptDataStandards", "As a representative of this organisation, you must accept the ALISS Data Standards to Add to ALISS.");
            }

            var descriptionLength = _lengthSanitizer.Sanitize(model.Description).Length;
            if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
            {
                ModelState.AddModelError("Description", $"The description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }

            model.Description = _sanitizer.Sanitize(model.Description);

            Guid organisationId = new Guid();

            string responseMessage = _organisationService.AddOrganisation(model, CurrentUser.UserProfileId, UserManager, out organisationId);

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (!_blobStorageService.IsValidImage(file, out string validationError))
                {
                    ViewBag.Error = true;
                    ModelState.AddModelError("InvalidLogo", validationError);
                    return View(model);
                }
                string logoUrl = _blobStorageService.UploadLogoToBlobStorage(organisationId, file);
                _organisationService.AddLogoToOrganisation(organisationId, logoUrl);
            }

            return RedirectToAction("AddService", new { id = organisationId });
        }

        public ActionResult EditOrganisation(Guid id, string returnUrl, bool claimError = false)
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
                return Redirect($"{publicBaseURL}organisations/{id}");
            }
            ViewBag.OrganisationId = id;
            EditOrganisationViewModel model = _organisationService.GetOrganisationForEdit(id);
            ViewBag.Published = model.Published;
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(id);
                ViewBag.IsLeadClaimant = organisation.ClaimedUser != null && organisation.ClaimedUserId == CurrentUser.UserProfileId;
                ViewBag.IsClaimant = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == id && x.ClaimedUserId == CurrentUser.UserProfileId).Any();
            }
            model.ReturnUrl = returnUrl;

            if (claimError)
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", "Claim Error: Your data has been saved but there was a problem when trying to set up your claim.  Once your data is published, you can submit a new claim.");
            }

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrganisation(EditOrganisationViewModel model)
        {
            ViewBag.OrganisationId = model.OrganisationId;

            var descriptionLength = _lengthSanitizer.Sanitize(model.Description).Length;
            if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
            {
                ModelState.AddModelError("Description", $"The description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.RemoveDuplicateErrorMessages();
                ViewBag.Error = true;
                ViewBag.Published = model.Published;
                return View(model);
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(model.OrganisationId);
                ViewBag.IsLeadClaimant = organisation.ClaimedUser != null && organisation.ClaimedUserId == CurrentUser.UserProfileId;
            }

            model.Description = _sanitizer.Sanitize(model.Description);

            string responseMessage = _organisationService.EditOrganisation(model, CurrentUser.UserProfileId);

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ViewBag.Published = model.Published;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (!_blobStorageService.IsValidImage(file, out string validationError))
                {
                    ViewBag.Error = true;
                    ViewBag.Published = model.Published;
                    ModelState.AddModelError("InvalidLogo", validationError);
                    return View(model);
                }
                string logoUrl = _blobStorageService.UploadLogoToBlobStorage(model.OrganisationId, file);
                _organisationService.AddLogoToOrganisation(model.OrganisationId, logoUrl);
            }

            if (_organisationService.GetNumberOfServicesForOrganisation(model.OrganisationId) == 0)
            {
                return RedirectToAction("AddService", new { id = model.OrganisationId });
            }

            if (String.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToAction("Index");
            }
            else
            {
                Thread.Sleep(1500);
                return Redirect(model.ReturnUrl);
            }
        }

        public ActionResult PublishOrganisation(Guid id, bool dataInput = false)
        {
            _organisationService.PublishOrganisation(id, CurrentUser.UserProfileId);
            return dataInput ? RedirectToAction("Index", "DataInputOrganisationSummary", new { id = id }) : RedirectToAction("EditOrganisation", new { id = id });

        }

        public ActionResult PublishService(Guid id, bool dataInput = false)
        {
            _serviceService.PublishService(id, CurrentUser.UserProfileId);
            return dataInput ? RedirectToAction("Index", "DataInputSummary", new { id = id }) : RedirectToAction("EditService", new { id = id });
        }

        public ActionResult DeleteOrganisation(Guid id)
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }
            DeleteOrganisationViewModel model = _organisationService.GetOrganisationForDelete(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrganisationConfirm(Guid id)
        {
            _organisationService.DeleteOrganisation(id, out string errorMessage);

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", errorMessage);

                DeleteOrganisationViewModel model = _organisationService.GetOrganisationForDelete(id);

                return View("DeleteOrganisation", model);
            }

        }

        public ActionResult ListServices(Guid id, string searchTerm = "", int page = 1)
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }

            ViewBag.publicURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            ViewBag.OrganisationId = id;
            var model = _serviceService.GetServices(id, searchTerm, page);
            return View(model);
        }

        public ActionResult AddService(Guid? id)
        {
            if (id.HasValue && !_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index");
            }

            if (id.HasValue)
            {
                ViewBag.OrganisationId = id.Value;
                EditServiceViewModel model = _serviceService.GetEmptyModelForAdd(id.Value);
                return View(model);
            }
            else
            {
                Guid newOrganisationId = Guid.NewGuid();
                while (_organisationService.DoesOrganisationIdExist(newOrganisationId))
                {
                    newOrganisationId = Guid.NewGuid();
                }
                EditServiceViewModel model = _serviceService.GetEmptyModelForAdd(newOrganisationId, true);
                return View("AddServiceOrganisation", model);
            }
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddService(EditServiceViewModel model)
        {
            string orgResponseMessage = "";

            ViewBag.OrganisationId = model.OrganisationId;

            var descriptionLength = _lengthSanitizer.Sanitize(model.Description).Length;
            if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
            {
                ModelState.AddModelError("Description", $"The description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.RemoveDuplicateErrorMessages();
                model = _serviceService.RepopulateModelForError(model);
                ViewBag.Error = true;
                return model.NewOrganisation ? View("AddServiceOrganisation", model) : View(model);
            }

            model.Description = _sanitizer.Sanitize(model.Description);

            if (model.NewOrganisation)
            {
                if (!_organisationService.DoesOrganisationIdExist(model.OrganisationId))
                {
                    AddOrganisationViewModel organisationToAdd = new AddOrganisationViewModel()
                    {
                        OrganisationId = model.OrganisationId,
                        Name = model.NewOrganisationName,
                        OrganisationRepresentative = model.OrganisationRepresentative,
                        OrganisationRepresentativeName = model.OrganisationRepresentativeName,
                        OrganisationRepresentativePhone = model.OrganisationRepresentativePhone,
                        OrganisationRepresentativeRole = model.OrganisationRepresentativeRole
                    };

                    orgResponseMessage = _organisationService.AddOrganisation(organisationToAdd, CurrentUser.UserProfileId, UserManager, out Guid organisationId);
                }
                else
                {
                    AddOrganisationViewModel organisationToAdd = new AddOrganisationViewModel()
                    {
                        OrganisationId = model.OrganisationId,
                        Name = model.NewOrganisationName
                    };

                    orgResponseMessage = _organisationService.EditTemporaryOrganisation(organisationToAdd, CurrentUser.UserProfileId, UserManager);
                }

                if (orgResponseMessage.Contains("Error"))
                {
                    model = _serviceService.RepopulateModelForError(model);
                    ViewBag.Error = true;

                    if (!orgResponseMessage.Contains("Claim Error"))
                    {
                        ModelState.AddModelError("DuplicateName", orgResponseMessage);
                        return View("AddServiceOrganisation", model);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", orgResponseMessage);
                    }
                }
            }

            string responseMessage = _serviceService.AddService(model, CurrentUser.UserProfileId, 100);

            if (model.NewOrganisation)
            {
                _emailService.SendOrganisationAddedEmail(model.OrganisationId, CurrentUser.UserProfileId, UserManager);
                return RedirectToAction("EditOrganisation", new { id = model.OrganisationId, claimError = orgResponseMessage.Contains("Claim Error") });
            }

            return RedirectToAction("ListServices", new { id = model.OrganisationId });
        }

        public ActionResult EditService(Guid id, string returnUrl)
        {
            EditServiceViewModel model = _serviceService.GetServiceToEdit(id);
            ViewBag.OrganisationId = model.OrganisationId;
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, model.OrganisationId))
            {
                if (!_serviceService.CanUserEditService(UserManager, CurrentUser.Username, model.ServiceId))
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ServiceId = model.ServiceId;
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(id);
                ViewBag.IsLeadClaimant = service.ClaimedUser != null && service.ClaimedUserId == CurrentUser.UserProfileId;
                ViewBag.IsClaimant = dc.ServiceClaimUsers.Where(x => x.ServiceId == id && x.ClaimedUserId == CurrentUser.UserProfileId).Any();
                ViewBag.IsLeadClaimantOfParentOrganisation = service.Organisation.ClaimedUser != null && service.Organisation.ClaimedUserId == CurrentUser.UserProfileId;
                ViewBag.OrganisationPublished = service.Organisation.Published;
            }
            ViewBag.Published = model.Published;
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(EditServiceViewModel model)
        {
            ViewBag.OrganisationId = model.OrganisationId;

            var descriptionLength = _lengthSanitizer.Sanitize(model.Description).Length;
            if (descriptionLength < _descriptionMinimumLength || descriptionLength > _descriptionMaximumLength)
            {
                ModelState.AddModelError("Description", $"The description field must be a minimum of {_descriptionMinimumLength} characters long and a maximum of {_descriptionMaximumLength} characters long, including spaces and punctuation.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.RemoveDuplicateErrorMessages();
                model = _serviceService.RepopulateModelForError(model);
                ViewBag.Error = true;
                ViewBag.Published = model.Published;
                ViewBag.OrganisationPublished = _organisationService.IsOrganisationPublished(model.OrganisationId);
                return View(model);
            }

            model.Description = _sanitizer.Sanitize(model.Description);

            string responseMessage = _serviceService.EditService(model, CurrentUser.UserProfileId);

            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(model.ServiceId);
                ViewBag.IsLeadClaimant = service.ClaimedUser != null && service.ClaimedUserId == CurrentUser.UserProfileId;
            }

            if (String.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToAction("ListServices", new { id = model.OrganisationId });
            }
            else
            {
                Thread.Sleep(1500);
                return Redirect(model.ReturnUrl);
            }
        }

        public ActionResult DeleteService(Guid id)
        {
            var model = _serviceService.GetServiceNameForDeleteConfirmation(id);
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, model.OrganisationId))
            {
                if (!_serviceService.CanUserEditService(UserManager, CurrentUser.Username, model.ServiceId))
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteServiceConfirm(Guid id)
        {
            Guid organisationId = _serviceService.DeleteService(id, out string errorMessage);

            if (organisationId != Guid.Empty && string.IsNullOrWhiteSpace(errorMessage))
            {
                return RedirectToAction("ListServices", new { id = organisationId });
            }
            else
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Error", errorMessage);

                var model = _serviceService.GetServiceNameForDeleteConfirmation(id);

                return View("DeleteService", model);
            }
        }

        // GET: Organisation Claimants
        public ActionResult ListOrganisationClaimants(Guid id, string searchTerm = "", int page = 1, string responseMessage = "")
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }
            ViewBag.OrganisationId = id;
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            OrganisationClaimantListingViewModel model = _organisationService.ListClaimants(searchTerm, id, page);
            ViewBag.ResponseMessage = responseMessage;

            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(id);
                ViewBag.IsLeadClaimant = organisation.ClaimedUser != null && organisation.ClaimedUserId == CurrentUser.UserProfileId;
            }

            return View(model);
        }

        // GET: Service Claimants
        public ActionResult ListServiceClaimants(Guid id, string searchTerm = "", int page = 1, string responseMessage = "")
        {
            if (!_serviceService.CanUserEditService(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }
            ViewBag.ServiceId = id;
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";

            ServiceClaimantListingViewModel model;

            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(s => s.ServiceId == id);
                ViewBag.IsLeadClaimant = service.ClaimedUserId.HasValue && service.ClaimedUserId == CurrentUser.UserProfileId;
                ViewBag.OrganisationId = service.OrganisationId;
                ViewBag.IsLeadClaimantOfParentOrganisation = service.Organisation.ClaimedUser != null && service.Organisation.ClaimedUserId == CurrentUser.UserProfileId;
                model = _serviceService.ListClaimants(searchTerm, service.OrganisationId, id, page);
            }
            
            ViewBag.ResponseMessage = responseMessage;

            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(id);
                ViewBag.IsLeadClaimant = service.ClaimedUser != null && service.ClaimedUserId == CurrentUser.UserProfileId;
            }

            return View(model);
        }

        public ActionResult MakeLeadOrganisationClaimant(Guid id)
        {
            OrganisationClaimUser organisationClaimUser = _organisationService.GetOrganisationClaimant(id);

            MakeLeadOrganisationClaimantViewModel model = new MakeLeadOrganisationClaimantViewModel
            {
                Claimant = new OrganisationClaimantPTO
                {
                    OrganisationClaimUserId = organisationClaimUser.OrganisationClaimUserId,
                    OrganisationId = organisationClaimUser.OrganisationId.Value,
                    ClaimantUserId = organisationClaimUser.ClaimedUserId.Value,
                    ClaimantName = organisationClaimUser.ClaimedUser.Name,
                    ClaimantEmail = organisationClaimUser.ClaimedUser.Email,
                    IsLeadClaimant = organisationClaimUser.IsLeadClaimant,
                    ApprovedOn = organisationClaimUser.ApprovedOn
                }
            };

            ViewBag.OrganisationId = model.Claimant.OrganisationId;
            ViewBag.isLeadClaimant = model.Claimant.IsLeadClaimant;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeLeadOrganisationClaimant(MakeLeadOrganisationClaimantViewModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }
            if (!model.Confirmation)
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Confirmation", "You must check the Confirmation checkbox.");
                return View(model);
            }

            string responseMessage = _organisationService.ChangeLeadOrganisationClaimant(model);

            return RedirectToAction("ListOrganisationClaimants", new { id = model.Claimant.OrganisationId });
        }

        public ActionResult MakeLeadServiceClaimant(Guid id)
        {
            ServiceClaimUser serviceClaimUser = _serviceService.GetServiceClaimant(id);

            MakeLeadServiceClaimantViewModel model = new MakeLeadServiceClaimantViewModel
            {
                Claimant = new ServiceClaimantPTO
                {
                    ServiceClaimUserId = serviceClaimUser.ServiceClaimUserId,
                    ServiceId = serviceClaimUser.ServiceId.Value,
                    ClaimantUserId = serviceClaimUser.ClaimedUserId.Value,
                    ClaimantName = serviceClaimUser.ClaimedUser.Name,
                    ClaimantEmail = serviceClaimUser.ClaimedUser.Email,
                    IsLeadClaimant = serviceClaimUser.IsLeadClaimant,
                    ApprovedOn = serviceClaimUser.ApprovedOn
                }
            };

            using(ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(model.Claimant.ServiceId);
                ViewBag.OrganisationId = service.OrganisationId.ToString();
                ViewBag.isLeadClaimantOfParentOrganisation = service.Organisation.ClaimedUser != null && service.Organisation.ClaimedUserId == CurrentUser.UserProfileId;
            }

            ViewBag.isLeadClaimant = model.Claimant.IsLeadClaimant;
            ViewBag.ServiceId = model.Claimant.ServiceId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeLeadServiceClaimant(MakeLeadServiceClaimantViewModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }
            if (!model.Confirmation)
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Confirmation", "You must check the Confirmation checkbox.");
                return View(model);
            }

            string responseMessage = _serviceService.ChangeLeadServiceClaimant(model);
            
            return RedirectToAction("ListServiceClaimants", new { id = model.Claimant.ServiceId });
        }

        // GET: Location
        public ActionResult ListLocations(Guid id, string searchTerm = "", int page = 1, string responseMessage = "")
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }
            ViewBag.OrganisationId = id;
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            LocationListingViewModel model = _locationService.ListLocation(searchTerm, id, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        //GET: New Location
        public ActionResult AddLocation(Guid id)
        {
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id))
            {
                return RedirectToAction("Index");
            }
            ViewBag.OrganisationId = id;
            EditLocationViewModel model = new EditLocationViewModel()
            {
                OrganisationId = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLocation(EditLocationViewModel model)
        {
            ViewBag.OrganisationId = model.OrganisationId;
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }

            string responseMessage = _locationService.AddLocation(model);

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }
            //Session["ResposneMessage"] = responseMessage;
            return RedirectToAction("ListLocations", new { id = model.OrganisationId });
        }

        public ActionResult EditLocation(Guid id)
        {
            EditLocationViewModel model = _locationService.GetLocationForEdit(id);
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, model.OrganisationId))
            {
                return RedirectToAction("Index");
            }
            ViewBag.OrganisationId = model.OrganisationId;
            ViewBag.LocationId = model.LocationId;
            return View("EditLocation", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLocation(EditLocationViewModel model)
        {
            ViewBag.OrganisationId = model.OrganisationId;
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }

            string responseMessage = _locationService.EditLocation(model);

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }
            //Session["ResposneMessage"] = responseMessage;
            return RedirectToAction("ListLocations", new { id = model.OrganisationId });
        }


        public ActionResult DeleteLocation(Guid id)
        {
            var model = _locationService.GetLocationForDelete(id);
            if (!_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, model.OrganisationId))
            {
                return RedirectToAction("Index");
            }
            if (!model.CanDelete)
            {
                ViewBag.ResponseMessage = "You cannot delete this address as it is the primary location for 1 or more services.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLocationConfirm(Guid id)
        {
            Guid organisationId = _locationService.DeleteLocation(id);
            return RedirectToAction("ListLocations", new { id = organisationId });
        }

        [HttpPost]
        public JsonResult AddServiceLocation(EditLocationViewModel model)
        {
            EditLocationViewModel locationToAdd = _locationService.AddServiceLocation(model);

            return Json(locationToAdd);
        }

        [HttpPost]
        public JsonResult EditServiceLocation(EditLocationViewModel model)
        {
            EditLocationViewModel location = _locationService.EditServiceLocation(model);

            return Json(location);
        }

        [HttpPost]
        public bool CheckOrganisationName(string name)
        {
            return _organisationService.DoesOrganisationExist(name);
        }

        public ActionResult UpdateElasticOrganisation(Guid id)
        {
            _elasticsearchService.AddOrganisationToElasticSearch(id);
            return RedirectToAction("Index");
        }

        public ActionResult UpdateElasticService(Guid id)
        {
            _elasticsearchService.AddServiceToElasticSearch(id);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteElasticOrganisation(Guid id)
        {
            _elasticsearchService.DeleteOrganisation(id);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteElasticService(Guid id)
        {
            _elasticsearchService.DeleteService(id);
            return RedirectToAction("Index");
        }
    }
}

namespace WebApplication.Common
{
    public static class ModelStateExtension
    {
        public static void RemoveDuplicateErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            //Stores the error messages we have seen
            var knownValues = new HashSet<string>();

            //Create a copy of the modelstatedictionary so we can modify the original.
            var modelStateDictionaryCopy = modelStateDictionary.ToDictionary(
                element => element.Key,
                element => element.Value);

            foreach (var modelState in modelStateDictionaryCopy)
            {
                var modelErrorCollection = modelState.Value.Errors;
                for (var i = 0; i < modelErrorCollection.Count; i++)
                {
                    //Check if we have seen the error message before by trying to add it to the HashSet
                    if (!knownValues.Add(modelErrorCollection[i].ErrorMessage))
                    {
                        modelStateDictionary[modelState.Key].Errors.RemoveAt(i);
                        modelStateDictionary[modelState.Key].Errors.Add("");
                    }
                }
            }
        }
    }
}