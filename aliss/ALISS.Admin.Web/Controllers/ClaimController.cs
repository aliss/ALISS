using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.ServiceClaim;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{

    [Authorize]
    public class ClaimController : ALISSBaseController
    {
        private readonly ClaimService _claimService;

        public ClaimController()
        {
            _claimService = new ClaimService();
        }


        // GET: Claim
        public ActionResult Index(string searchTerm = "", int page = 1, string responseMessage = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
            bool isAdmin = UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());

            ClaimListingViewModel model = _claimService.ListClaims(searchTerm, CurrentUser.Username, UserManager, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        // GET: My Claims
        public ActionResult MyClaims(string searchTerm = "", int page = 1, string responseMessage = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";

            ClaimListingViewModel model = _claimService.ListMyClaims(searchTerm, CurrentUser.UserProfileId, page);
            ViewBag.ResponseMessage = responseMessage;

            return View(model);
        }

        // GET: Claim
        public ActionResult EditClaim(Guid id)
        {
            EditClaimViewModel model = _claimService.GetClaimForEdit(id);
            ViewBag.ClaimedUserId = model.ClaimedUserId;

            using (ALISSContext dc = new ALISSContext())
            {
                Claim claim = dc.Claims.Find(id);
                ViewBag.IsClaimed = claim.Organisation.ClaimedUser != null;
                ViewBag.IsLeadClaimant = claim.Organisation.ClaimedUser != null && claim.Organisation.ClaimedUserId == CurrentUser.UserProfileId;
            }
            return View(model);
        }

        //POST: Approve Claim
        public ActionResult Approve(Guid id)
        {
            string responseMessage = _claimService.ApproveClaims(id, CurrentUser.UserProfileId, false, UserManager);
            return RedirectToAction("Index");
        }

        //POST: Approve Claim and set as lead
        public ActionResult ApproveWithLead(Guid id)
        {
            string responseMessage = _claimService.ApproveClaims(id, CurrentUser.UserProfileId, true, UserManager);
            return RedirectToAction("Index");
        }

        //POST: Revoke Claim
        public ActionResult Revoke(Guid id)
        {
            string responseMessage = _claimService.RevokeClaims(id, CurrentUser.UserProfileId, UserManager);
            return RedirectToAction("Index");
        }

        //POST: Deny Claim
        public ActionResult Deny(Guid id)
        {
            string responseMessage = _claimService.DenyClaims(id, CurrentUser.UserProfileId, UserManager);
            return RedirectToAction("Index");
        }

        //POST: Un-reviewed Claim
        public ActionResult Unreviewed(Guid id)
        {
            string responseMessage = _claimService.Unreviewed(id, CurrentUser.UserProfileId);
            return RedirectToAction("Index");
        }
    }
}


