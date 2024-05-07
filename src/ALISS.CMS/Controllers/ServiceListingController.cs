using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace ALISS.CMS.Controllers
{
    public class ServiceListingController : RenderMvcController
    {
        // GET: Service Listing
        public ActionResult Index(ContentModel model, int page = 1)
        {
            SearchService orgService = new SearchService();
            SearchAPIModel apiModel = orgService.GetSiteMap(page);
            SearchInputModel input = new SearchInputModel()
            {
                Page = page
            };
            SearchViewModel viewModel = new SearchViewModel(input);
            viewModel.SearchModel = apiModel;
            ViewBag.SearchViewModel = viewModel;
            ViewBag.Index = false;

            return View(model);
        }
    }
}