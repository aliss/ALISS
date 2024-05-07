using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace ALISS.CMS.Controllers
{
    public class LogoutController : SurfaceController
    {
        // GET: Logout
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"] != null)
            {
                HttpCookie currentUserCookie = System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"];
                System.Web.HttpContext.Current.Response.Cookies.Remove("ALISSAdmin.User");
                string urlHost = System.Web.HttpContext.Current.Request.Url.Host;
                urlHost = urlHost.Substring(urlHost.IndexOf('.') + 1);
                if (!urlHost.Contains("."))
                {
                    urlHost = System.Web.HttpContext.Current.Request.Url.Host;
                }
                currentUserCookie.Domain = $".{urlHost}";
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                System.Web.HttpContext.Current.Response.SetCookie(currentUserCookie);
            }

            return Redirect("/");
        }
    }
}