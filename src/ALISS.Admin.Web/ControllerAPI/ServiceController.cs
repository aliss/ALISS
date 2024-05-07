using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Web;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/service")]
    public class ServiceController : ApiController
    {
        private readonly ServiceService _serviceService;
        private readonly Business.Services.EmailService _emailService;
        private readonly UserProfileService _userProfileService;

        public ServiceController()
        {
            _serviceService = new ServiceService();
            _emailService = new Business.Services.EmailService();
            _userProfileService = new UserProfileService();
        }

        [HttpGet]
        [Route("GetServiceLogo")]
        public string GetServiceLogo(Guid serviceId)
        {
            string logoUrl = _serviceService.GetServiceToEdit(serviceId).Logo;
            return logoUrl;
        }

        [HttpPost]
        [Route("PostShareService")]
        public string ShareService(ShareServiceViewModel model)
        {
            model = _serviceService.GetServiceToShare(model);
            _emailService.SendShareServiceEmail(model);
            return "Service was successfully shared with the intended recipient.";
        }

        [HttpGet]
        [Route("GetCanUserEditService")]
        public bool GetCanUserEditService(int userProfileId, Guid serviceId)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            EditUserViewModel userProfile = _userProfileService.GetUserForEdit(userProfileId, userManager);
            bool canUserEdit = _serviceService.CanUserEditService(userManager, userProfile.Username, serviceId);
            return canUserEdit;
        }
    }
}
