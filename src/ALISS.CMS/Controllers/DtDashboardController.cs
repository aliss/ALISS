using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class DtDashboardController : RenderMvcController
    {
        public DtDashboardController() { }

        // GET: DtContent
        public override ActionResult Index(ContentModel model)
        {
            return View(model);
        }
    }
}