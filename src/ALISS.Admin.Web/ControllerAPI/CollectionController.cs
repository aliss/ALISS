using ALISS.Business.Services;
using ALISS.Business.ViewModels.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/collection")]
    public class CollectionController : ApiController
    {
        private CollectionService _collectionService;
        private Business.Services.EmailService _emailService;

        public CollectionController()
        {
            _collectionService = new CollectionService();
            _emailService = new Business.Services.EmailService();
        }

        [HttpGet]
        [Route("GetCollectionsForUser")]
        public CollectionListingViewModel GetCollectionsForUser(int userProfileId, int page = 1)
        {
            var model = _collectionService.GetCollectionsForUser(userProfileId, page);

            return model;
        }

        [HttpGet]
        [Route("GetCollection")]
        public CollectionViewModel GetCollection(Guid collectionId, int page = 1)
        {
            var model = _collectionService.GetCollection(collectionId, page);
            return model;
        }

        [HttpPost]
        [Route("PostEmailCollection")]
        public string PostEmailCollection(PostEmailCollectionViewModel model)
        {
            EmailCollectionViewModel modelToSend = _collectionService.GetCollectionToEmail(model);
            _emailService.SendCollectionEmail(modelToSend);

            return "Email sent successfully";
        }

        [HttpPost]
        [Route("PostCollection")]
        public Guid PostCollection(EditCollectionViewModel model)
        {
            Guid newCollectionId = _collectionService.AddNewCollectionForUser(model);

            return newCollectionId;
        }

        [HttpPost]
        [Route("PostAddServiceToCollection")]
        public Guid AddServiceToCollection(AddCollectionServiceViewModel model)
        {
            Guid collectionId = _collectionService.AddServiceToCollection(model);
            return collectionId;
        }

        [HttpPost]
        [Route("PostDeleteCollection")]
        public string DeleteCollection(AddCollectionServiceViewModel model)
        {
            _collectionService.DeleteCollection(model.CollectionId.Value);
            return "Collection was successfully deleted.";
        }

        [HttpPost]
        [Route("PostRemoveServiceFromCollection")]
        public string RemoveServiceFromCollection(AddCollectionServiceViewModel model)
        {
            _collectionService.RemoveServiceFromCollection(model);
            return "Service was successfully removed from the collection.";
        }
    }
}
