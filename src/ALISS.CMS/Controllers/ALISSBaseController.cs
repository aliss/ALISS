using ALISS.CMS.Models.User;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Umbraco.Web.Mvc;

namespace ALISS.CMS.Controllers
{
    public class ALISSBaseController : SurfaceController
    {
        // GET: ALISSBase
        private string _apiBaseUrl = ConfigurationManager.AppSettings["Settings:AdminApiBaseUrl"];
        public CurrentUserViewModel CurrentUser { get; set; }
        
        public ALISSBaseController()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            string alissAdminUser = System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"] != null
                ? System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"].Value
                : "";

            CurrentUser = !string.IsNullOrEmpty(alissAdminUser)
				? (CurrentUserViewModel)js.Deserialize(Encoding.UTF8.GetString(Convert.FromBase64String(alissAdminUser)), typeof(CurrentUserViewModel))
				: new CurrentUserViewModel();
		}
    }
}