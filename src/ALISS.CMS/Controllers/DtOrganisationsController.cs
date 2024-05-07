using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Search;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class DtOrganisationsController : RenderMvcController
    {
        public DtOrganisationsController() { }

		public ActionResult Index(ContentModel model, OrganisationSearchInputModel input)
		{
			string fullUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{Request.Url.AbsolutePath}?";
			if (!string.IsNullOrEmpty(input.SearchTerm))
			{
				fullUrl += "searchterm=" + input.SearchTerm + "&";
			}
			if (input.Page != null && input.Page.Value > 1)
			{
				fullUrl += "page=" + input.Page;
			}
			fullUrl = fullUrl.TrimEnd('?').TrimEnd('&');

			HttpCookie orgSearchUrlCookie = new HttpCookie("orgSearchUrl");
			orgSearchUrlCookie.Value = fullUrl;
			orgSearchUrlCookie.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Remove("orgSearchUrl");
			Response.Cookies.Add(orgSearchUrlCookie);

			//input.PageSize = (model.Content as DtOrganisations).PageSize;

			SearchService orgService = new SearchService();
			OrganisationSearchAPIModel apiModel = orgService.GetOrganisationSearch(input);
			OrganisationSearchViewModel viewModel = new OrganisationSearchViewModel(input);
			viewModel.SearchModel = apiModel;
			ViewBag.OrganisationSearchViewModel = viewModel;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{Request.Url.Scheme}://{Request.Url.Host}");
            ViewBag.AboutClaimedUrl = client.GetStringAsync("/umbraco/api/navigation/GetAboutClaimedUrl").Result.TrimStart('\\', '"', '/').TrimEnd('"');

            return View(model);
		}
	}
}