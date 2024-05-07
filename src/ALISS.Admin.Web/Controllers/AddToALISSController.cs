using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Admin.Web.Helpers;

namespace ALISS.Admin.Web.Controllers
{
    public class AddToALISSController : DataInputBaseController
    {
        // GET: AddToALISS
        public ActionResult Index()
        {
            ViewBag.ContinueAdding = _dataInputService.getUsersUnfinishedServices(UserManager, CurrentUser.Username);
            var pickUp = Request["w"];
            ViewBag.ShowWelcome = pickUp != "f";
            TempData.Clear();

            ViewBag.Guidance = pickUp != "f"
                ? GenericHelpers.GetGuidanceContent("Add to ALISS")
                : GenericHelpers.GetGuidanceContent("Unpublished Forms");

            return View();
        }
    }
}