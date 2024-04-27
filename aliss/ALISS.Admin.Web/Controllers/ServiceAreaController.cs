using ALISS.Business.Services;
using ALISS.Business.ViewModels.ServiceArea;
using System;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class ServiceAreaController : ALISSBaseController
    {
        private readonly ServiceAreaService _serviceAreaService;

        public ServiceAreaController()
        {
            _serviceAreaService = new ServiceAreaService();
        }

        // GET: ServiceAreList
        public ActionResult Index()
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            ServiceAreaListingViewModel model = _serviceAreaService.ListServiceAreas();
            return View(model);

        }

        //GET: New ServiiceArea    
        public ActionResult AddServiceArea()
        {
            EditServiceAreaViewModel editViewModel = new EditServiceAreaViewModel()
            {
                ServiceAreaType = _serviceAreaService.GetServiceAreaDropdownList(),
            };
            return View(editViewModel);
        }

        //POST: New ServiceArea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddServiceArea(EditServiceAreaViewModel model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                model.ServiceAreaType = _serviceAreaService.GetServiceAreaDropdownList(Convert.ToInt32(model.Type));
                return View(model);
            }

            string responseMessage = "";
            if (model.ServiceAreaId == 0)
            {
                responseMessage = _serviceAreaService.AddServiceArea(model);
            }

            else
            {
                responseMessage = _serviceAreaService.EditServiceArea(model);
            }

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                model.ServiceAreaType = _serviceAreaService.GetServiceAreaDropdownList(Convert.ToInt32(model.Type));
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }
            //Session["ResponseMessage"] = responseMessage;
            return RedirectToAction("Index");
        }

        public ActionResult DeleteServiceArea(int id)
        {
            DeleteServiceAreaModel model = _serviceAreaService.GetServiceAreaForDelete(id);
            if (!model.CanDelete)
            {
                ViewBag.ResponseMessage = "You cannot delete this service area as it is the primary location for 1 or more services.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteServiceAreaConfirm(int id)
        {
            _serviceAreaService.DeleteServiceArea(id);

            return RedirectToAction("Index");
        }

        // GET: Edit ServiceArea
        public ActionResult EditServiceArea(int id)
        {
            EditServiceAreaViewModel editViewModel = _serviceAreaService.GetServiceAreaForEdit(id);
            return View("EditServiceArea", editViewModel);
        }
    }
}