using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Service;
using ALISS.CMS.Helpers;
using ALISS.CMS.Models.Collection;
using RestSharp;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class SitemapController : ALISSBaseController
    {
        public SitemapController() { }

        public ActionResult Index()
        {
            return View(Umbraco.ContentAtRoot().First());
        }
    }
}