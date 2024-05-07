using ALISS.Business.Services;
using ALISS.Business.ViewModels.Collection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class CollectionController : ALISSBaseController
    {
        private readonly CollectionService _collectionService;
        private readonly Business.Services.EmailService _emailService;

        public CollectionController()
        {
            _collectionService = new CollectionService();
            _emailService = new Business.Services.EmailService();
        }

        // GET: Collections
        public ActionResult Index(int page = 1)
        {
            CollectionListingViewModel model = _collectionService.GetCollectionsForUser(CurrentUser.UserProfileId, page);
            return View(model);
        }
        public ActionResult ViewCollection(Guid id, int page = 1)
        {
            ViewBag.PublicUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();
            CollectionViewModel model = _collectionService.GetCollection(id, page, UserManager, CurrentUser.Username);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCollection(EditCollectionViewModel model)
        {
            if(String.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("NoName", "You must provide a collection name");
                return RedirectToAction("Index");
            }
            model.UserProfileId = CurrentUser.UserProfileId;
            Guid newCollectionId = _collectionService.AddNewCollectionForUser(model);
            return RedirectToAction("Index");
        }

        public ActionResult EmailCollection(Guid id)
        {
            CollectionViewModel collection = _collectionService.GetCollection(id, 1);
            ViewBag.CollectionName = collection.Name;
            PostEmailCollectionViewModel model = new PostEmailCollectionViewModel()
            {
                CollectionId = id
            };
            return View(model);    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailCollection(PostEmailCollectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CollectionViewModel collection = _collectionService.GetCollection(model.CollectionId, 1);
                ViewBag.CollectionName = collection.Name;
                ViewBag.Error = true;
                return View(model);
            }

            EmailCollectionViewModel modelToSend = _collectionService.GetCollectionToEmail(model);
            _emailService.SendCollectionEmail(modelToSend);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteCollection(Guid id)
        {
            _collectionService.DeleteCollection(id);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteService(Guid collectionId, Guid serviceId)
        {
            var model = new AddCollectionServiceViewModel()
            {
                ServiceId = serviceId,
                CollectionId = collectionId
            };
            _collectionService.RemoveServiceFromCollection(model);

            return RedirectToAction("ViewCollection", new { id = collectionId });
        }
    }
}