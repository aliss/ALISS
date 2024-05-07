using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.CommunityGroup;
using System.Linq;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class CommunityGroupController : ALISSBaseController
    {
        private readonly CommunityGroupService _communityGroupService;

        public CommunityGroupController()
        {
            _communityGroupService = new CommunityGroupService();
        }

		// GET: CommunityGroup
		public ActionResult Index(string searchTerm = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
			CommunityGroupListingViewModel model = _communityGroupService.ListCommunityGroups(searchTerm);
            return View(model);
        }

        public ActionResult AddCommunityGroup(int parentCommunityGroupId = 0)
        {
            if (parentCommunityGroupId == 0)
            {
                return View("AddTopLevelCommunityGroup", new EditTopLevelCommunityGroupViewModel());
            }
            else
            {
				EditLowerLevelCommunityGroupViewModel viewModel = new EditLowerLevelCommunityGroupViewModel()
                {
					ParentCommunityGroupId = parentCommunityGroupId,
					CommunityGroups = _communityGroupService.GetCommunityGroupDropdown(parentCommunityGroupId)
                };
                return View("AddLowerLevelCommunityGroup", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTopLevelCommunityGroup(EditTopLevelCommunityGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }
            string responseMessage = "";
            if (model.CommunityGroupId == 0)
            {
                responseMessage = _communityGroupService.AddTopLevelCommunityGroup(model);
            }
            else
            {
                responseMessage = _communityGroupService.EditTopLevelCommunityGroup(model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLowerLevelCommunityGroup(EditLowerLevelCommunityGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                model.CommunityGroups = _communityGroupService.GetCommunityGroupDropdown(model.ParentCommunityGroupId, model.CommunityGroupId);
                return View(model);
            }

            string responseMessage = "";

            if (model.CommunityGroupId == 0)
            {
                responseMessage = _communityGroupService.AddLowerLevelCommunityGroup(model);
            }
            else
            {
                responseMessage = _communityGroupService.EditLowerLevelCommunityGroup(model);
            }

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                model.CommunityGroups = _communityGroupService.GetCommunityGroupDropdown(model.ParentCommunityGroupId, model.CommunityGroupId);
                return View(model);
            }
            //Session["ResponseMessage"] = responseMessage;
            return RedirectToAction("Index");
        }

        public ActionResult EditCommunityGroup(int id, bool topLevelCommunityGroup)
        {
            if (topLevelCommunityGroup)
            {
				EditTopLevelCommunityGroupViewModel editViewModel = _communityGroupService.GetTopLevelCommunityGroupForEdit(id);
                using (ALISSContext dc = new ALISSContext())
                {
                    ViewBag.ContainsLowerLevel = dc.CommunityGroups.Where(c => c.ParentCommunityGroupId == id).ToList().Count > 0;
                }
                return View("AddTopLevelCommunityGroup", editViewModel);
            }
            else
            {
				EditLowerLevelCommunityGroupViewModel editViewModel = _communityGroupService.GetLowerLevelCommunityGroupForEdit(id);
                return View("AddLowerLevelCommunityGroup", editViewModel);
            }
        }

        public ActionResult DeleteCommunityGroup(int id)
        {
			DeleteCommunityGroupViewModel model = _communityGroupService.GetCommunityGroupForDelete(id);
            if (!model.CanDelete)
            {
                ViewBag.ResponseMessage = "You cannot delete this who category as it has a number of who categories below it.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommunityGroupConfirm(int id)
        {
            _communityGroupService.DeleteCommunityGroup(id);
            return RedirectToAction("Index");
        }
    }
}