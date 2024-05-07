using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System;
using System.Net;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class UserController : ALISSBaseController
    {
        private readonly UserProfileService _userProfileService;

        public UserController()
        {
            _userProfileService = new UserProfileService();
        }


        // GET: User
        public ActionResult Index(string searchTerm = "", int page = 1, string responseMessage = "")
        {

            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            UserListingViewModel model = _userProfileService.ListUsers(searchTerm, UserManager, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        // GET: User
        public ActionResult EditUser(int id, int servicePage = 1, int organisationPage = 1)
        {
            EditUserViewModel editViewModel = _userProfileService.GetUserForEdit(id, UserManager, organisationPage, servicePage);
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            return View("EditUser", editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserViewModel model)
        {
            String publicBaseURL = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            ViewBag.publicURL = publicBaseURL;
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                if (model.UserOrganisations == null)
                {
                    model.UserOrganisations = new Business.ViewModels.Organisation.OrganisationListingViewModel();
                }
                if (model.UserServices == null)
                {
                    model.UserServices = new Business.ViewModels.Service.ServiceListingViewModel();
                }
                return View("EditUser", model);
            }

            bool forceLogout = false;
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userToEdit = dc.UserProfiles.Find(model.UserProfileId);
                forceLogout = ViewBag.ProfileId == model.UserProfileId && userToEdit.Username != model.Username;
            }

            string editUserResult = _userProfileService.EditUser(model, UserManager);
            if (editUserResult.StartsWith("Error:"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Email", editUserResult.Replace("Error:", "").Trim());
                return View("EditUser", model);
            }

            if (forceLogout)
            {
                AccountController controller = DependencyResolver.Current.GetService<AccountController>();
                controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
                controller.LogOff();
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteUser(int id)
        {
            DeleteUserViewModel model = _userProfileService.GetUserToDelete(id);
            if (!model.IsDeleteValid)
            {
                ViewBag.Error = true;
                ModelState.AddModelError("Delete", "Please ensure that any of this users claims on organisations / services have been transfered to a manager");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirm(int id)
        {
            _userProfileService.DeleteUser(id, UserManager);

            return RedirectToAction("Index");
        }
    }
}


