using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.BulkEmail;

namespace ALISS.Admin.Web.Controllers
{
    public class BulkEmailController : ALISSBaseController
    {
        private readonly Business.Services.EmailService _emailService = new Business.Services.EmailService();
        private readonly UserProfileService _userProfileService = new UserProfileService();

        [HttpGet]
        public ActionResult EmailAllUsers()
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            CustomEmailViewModel model = new CustomEmailViewModel
            {
                Subject = string.Empty,
                Body = string.Empty
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EmailAllUsers(CustomEmailViewModel model)
        {
            if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
            {
                if(Request.Files["EmailAttachment"].ContentLength > 2621440)
                {
                    ModelState.AddModelError("emailInput-File", "The file uploaded is larger than the allowed size (2.5Mb)");
                }

                model.Attachment = Request.Files["EmailAttachment"];
            }

            if (!ModelState.IsValid)
            {
                if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
                {
                    ModelState.AddModelError("emailInput-File", "Your file needs to be re-uploaded as an error has occured");
                }

                ViewBag.Error = true;
                return View(model);
            }

            _emailService.SendBulkCustomEmail(_userProfileService.GetAllUsersForBulkEmail(), model);
            return RedirectToAction("Index", "Home", null);
        }

        [HttpGet]
        public ActionResult EmailAllEditors()
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            CustomEmailViewModel model = new CustomEmailViewModel
            {
                Subject = string.Empty,
                Body = string.Empty
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EmailAllEditors(CustomEmailViewModel model)
        {
            if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
            {
                if (Request.Files["EmailAttachment"].ContentLength > 2621440)
                {
                    ModelState.AddModelError("emailInput-File", "The file uploaded is larger than the allowed size (2.5Mb)");
                }

                model.Attachment = Request.Files["EmailAttachment"];
            }

            if (!ModelState.IsValid)
            {
                if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
                {
                    ModelState.AddModelError("emailInput-File", "Your file needs to be re-uploaded as an error has occured");
                }

                ViewBag.Error = true;
                return View(model);
            }

            _emailService.SendBulkCustomEmail(_userProfileService.GetAllEditorsForBulkEmail(), model);
            return RedirectToAction("Index", "Home", null);
        }

        [HttpGet]
        public ActionResult EmailAllClaimants()
        {
            if (!ViewBag.IsAdmin)
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            CustomEmailViewModel model = new CustomEmailViewModel
            {
                Subject = string.Empty,
                Body = string.Empty
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EmailAllClaimants(CustomEmailViewModel model)
        {
            if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
            {
                if (Request.Files["EmailAttachment"].ContentLength > 2621440)
                {
                    ModelState.AddModelError("emailInput-File", "The file uploaded is larger than the allowed size (2.5Mb)");
                }

                model.Attachment = Request.Files["EmailAttachment"];
            }

            if (!ModelState.IsValid)
            {
                if (Request.Files.Count > 0 && Request.Files["EmailAttachment"].ContentLength > 0)
                {
                    ModelState.AddModelError("emailInput-File", "Your file needs to be re-uploaded as an error has occured");
                }

                ViewBag.Error = true;
                return View(model);
            }

            _emailService.SendBulkCustomEmail(_userProfileService.GetAllClaimantsForBulkEmail(), model);
            return RedirectToAction("Index", "Home", null);
        }
    }
}