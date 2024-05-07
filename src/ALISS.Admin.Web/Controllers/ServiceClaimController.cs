using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.ServiceClaim;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{

    [Authorize]
    public class ServiceClaimController : ALISSBaseController
    {
        private readonly ServiceClaimService _claimService;

        public ServiceClaimController()
        {
            _claimService = new ServiceClaimService();
        }

        // GET: Claim
        public ActionResult Index(string searchTerm = "", int page = 1, string responseMessage = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
            bool isAdmin = UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());

            ServiceClaimListingViewModel model = _claimService.ListClaims(searchTerm, CurrentUser.Username, UserManager, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        // GET: My Claims
        public ActionResult MyClaims(string searchTerm = "", int page = 1, string responseMessage = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";

            ServiceClaimListingViewModel model = _claimService.ListMyClaims(searchTerm, CurrentUser.UserProfileId, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        // GET: Claim
        public ActionResult EditClaim(Guid id)
        {
            EditServiceClaimViewModel model = _claimService.GetClaimForEdit(id);
            ViewBag.ClaimedUserId = model.ClaimedUserId;

            using (ALISSContext dc = new ALISSContext())
            {
                ServiceClaim claim = dc.ServiceClaims.Find(id);
                ViewBag.IsClaimed = claim.Service.ClaimedUser != null;
                ViewBag.IsLeadClaimant = claim.Service.ClaimedUser != null && claim.Service.ClaimedUserId == CurrentUser.UserProfileId;
            }
            return View(model);
        }

        //POST: Approve Claim
        public ActionResult Approve(Guid id)
        {
            string responseMessage = _claimService.ApproveClaim(id, CurrentUser.UserProfileId, false, UserManager);
            return RedirectToAction("Index", new { tab = "services" });
        }

        //POST: Approve Claim and set as lead
        public ActionResult ApproveWithLead(Guid id)
        {
            string responseMessage = _claimService.ApproveClaim(id, CurrentUser.UserProfileId, true, UserManager);
            return RedirectToAction("Index");
        }

        //POST: Revoke Claim
        public ActionResult Revoke(Guid id)
        {
            string responseMessage = _claimService.RevokeClaim(id, CurrentUser.UserProfileId, UserManager);
            return RedirectToAction("Index", new { tab = "services" });
        }

        //POST: Deny Claim
        public ActionResult Deny(Guid id)
        {
            string responseMessage = _claimService.DenyClaim(id, CurrentUser.UserProfileId, UserManager);
            return RedirectToAction("Index", new { tab = "services" });
        }

        //POST: Un-reviewed Claim
        public ActionResult Unreviewed(Guid id)
        {
            string responseMessage = _claimService.UnreviewedClaim(id, CurrentUser.UserProfileId);
            return RedirectToAction("Index", new { tab = "services" });
        }
    }
}


