using ALISS.Business.Services;
using ALISS.Business.ViewModels.AccessibilityFeature;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class AccessibilityFeatureController : ALISSBaseController
    {
        private readonly AccessibilityFeatureService _accessibilityFeatureService;

        public AccessibilityFeatureController()
        {
            _accessibilityFeatureService = new AccessibilityFeatureService();
        }

		// GET: AccessibilityFeature
		public ActionResult Index(string searchTerm = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
			AccessibilityFeatureListingViewModel model = _accessibilityFeatureService.ListAccessibilityFeatures(searchTerm);
            return View(model);
        }

        public ActionResult AddAccessibilityFeature()
        {
            return View("AddTopLevelAccessibilityFeature", new EditTopLevelAccessibilityFeatureViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTopLevelAccessibilityFeature(EditTopLevelAccessibilityFeatureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }
            string responseMessage = "";
            if (model.AccessibilityFeatureId == 0)
            {
                responseMessage = _accessibilityFeatureService.AddTopLevelAccessibilityFeature(model);
            }
            else
            {
                responseMessage = _accessibilityFeatureService.EditTopLevelAccessibilityFeature(model);
            }
            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }
            //Session["ResponseMessage"] = responseMessage;
            return RedirectToAction("Index");
        }

        public ActionResult EditAccessibilityFeature(int id)
        {
            EditTopLevelAccessibilityFeatureViewModel editViewModel = _accessibilityFeatureService.GetTopLevelAccessibilityFeatureForEdit(id);
            return View("AddTopLevelAccessibilityFeature", editViewModel);
        }

        public ActionResult DeleteAccessibilityFeature(int id)
        {
			DeleteAccessibilityFeatureViewModel model = _accessibilityFeatureService.GetAccessibilityFeatureForDelete(id);
            if (!model.CanDelete)
            {
                ViewBag.ResponseMessage = "You cannot delete this accessibility feature as it has a number of accessibility features below it.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccessibilityFeatureConfirm(int id)
        {
            _accessibilityFeatureService.DeleteAccessibilityFeature(id);
            return RedirectToAction("Index");
        }
    }
}