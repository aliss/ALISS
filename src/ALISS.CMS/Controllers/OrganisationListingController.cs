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
    public class OrganisationListingController : RenderMvcController
    {
        // GET: Organisation Listing
        public ActionResult Index(ContentModel model, int page = 1)
        {
            SearchService orgService = new SearchService();
            OrganisationSearchAPIModel apiModel = orgService.GetOrganisationSiteMap(page);
            OrganisationSearchInputModel input = new OrganisationSearchInputModel()
            {
                Page = page
            };
            OrganisationSearchViewModel viewModel = new OrganisationSearchViewModel(input);
            viewModel.SearchModel = apiModel;
            ViewBag.SearchViewModel = viewModel;
            ViewBag.Index = false;

            return View(model);
        }
    }
}