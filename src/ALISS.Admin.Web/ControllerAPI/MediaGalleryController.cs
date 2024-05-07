using ALISS.Admin.Web.Controllers.DataInputControllers;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using System;
using System.Drawing;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/mediagallery")]
    public class MediaGalleryController : ApiController
    {
        private readonly DataInputService _dataInputService;
        private readonly Business.Services.EmailService _emailService;
        private BlobStorageService _blobStorageService;

        public MediaGalleryController()
        {
            _dataInputService = new DataInputService();
            _emailService = new Business.Services.EmailService();
        }

        [HttpGet]
        [Route("ApproveMedia")]
        public string ApproveMedia(Guid mediaId)
        {
            return _dataInputService.ApproveServiceGallery(mediaId);
        }

        [Authorize]
        [HttpPost]
        [Route("RemoveImage")]
        public bool RemoveImage(UploadImageRequest model)
        {
            if (model.MediaGalleryId != Guid.Empty)
            {
                _dataInputService.RemoveMediaGallery(model.MediaGalleryId);
            }
            else if (model.ServiceId != Guid.Empty)
            {
                _dataInputService.RemoveServiceLogo(model.ServiceId);
            }
            else if (model.OrganisationId != Guid.Empty)
            {
                _dataInputService.RemoveOrganisationLogo(model.OrganisationId);
            }
            else if (!string.IsNullOrEmpty(model.MediaGalleryUrl))
            {
                _dataInputService.RemoveMediaGallery(_dataInputService.GetGuidFromBlobUrl(model.MediaGalleryUrl));
            }
            return true;
        }

        [Authorize]
        [HttpPost]
        [Route("UploadImage")]
        public dynamic UploadImage(UploadImageRequest model)
        {
            _blobStorageService = new BlobStorageService();

            var valid = _blobStorageService.IsValidImage(model.ImageData, out string validationError);
            if (!valid)
            {
                return validationError;
            }

            var imageGuid = model.ImageType == "OrganisationLogo"
                ? model.OrganisationId
                : model.ImageType == "ServiceLogo"
                    ? model.ServiceId
                    : model.MediaGalleryId != Guid.Empty
                        ? model.MediaGalleryId
                        : String.IsNullOrEmpty(model.MediaGalleryUrl)
                            ? Guid.NewGuid()
                            : _dataInputService.GetGuidFromBlobUrl(model.MediaGalleryUrl);

            string url = _blobStorageService.UploadLogoToBlobStorage(imageGuid, model.ImageName);
            string galleryId = "";

            if (model.ImageType == "OrganisationLogo")
            {
                _dataInputService.EditOrganisationLogo(model.OrganisationId, url, model.UserId);
            }
            else if (model.ImageType == "ServiceLogo")
            {
                _dataInputService.EditServiceLogo(model.ServiceId, url, model.UserId);
            }
            else
            {
                galleryId = _dataInputService.EditServiceGallery(model.ServiceId, imageGuid, url, model.UserId, model.IsAdmin, model.GalleryRef);
            }
            return new { url, galleryId };
        }
    }

    public class UploadImageRequest
    {
        public Guid OrganisationId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid MediaGalleryId { get; set; }
        public string MediaGalleryUrl { get; set; }
        public string ImageType { get; set; }
        public string ImageName { get; set; }
        public string ImageData { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public int GalleryRef { get; set; }
    }
}
