using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private UserProfileService _userProfileService;

        public LoginController()
        {
            _userProfileService = new UserProfileService();
        }

        [HttpGet]
        [Route("GetLoggedInUser")]
        public CurrentUserViewModel GetLoggedInUser()
        {
            System.Security.Principal.IPrincipal user = System.Web.HttpContext.Current.User;
            CurrentUserViewModel userModel = _userProfileService.GetCurentLoggedInUser(user.Identity.Name);
            return userModel;
        }
    }
}
