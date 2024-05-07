using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ALISS.API.Models.Elasticsearch;
using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Search;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class DtServicesController : RenderMvcController
    {
        public DtServicesController() {}

		public ActionResult Index(ContentModel model, SearchInputModel input)
		{
            if (string.IsNullOrWhiteSpace(input.Postcode) && !string.IsNullOrWhiteSpace(input.Sort) && input.Sort.Equals("sort-distance"))
            {
                input.Sort = string.IsNullOrEmpty(input.Query) ? "sort-last-reviewed" : "sort-relevance";
            }

			string fullUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{Request.Url.AbsolutePath}?postcode={input.Postcode}";
			if (!string.IsNullOrWhiteSpace(input.Categories))
			{
                fullUrl += "&categories=" + input.Categories;
			}
            if (!string.IsNullOrWhiteSpace(input.Community_Groups))
            {
                fullUrl += "&communitygroups=" + input.Community_Groups;
            }
			if (!string.IsNullOrWhiteSpace(input.Query))
			{
				fullUrl += "&query=" + input.Query;
			}
            if (!string.IsNullOrWhiteSpace(input.Accessibility_Features))
            {
                fullUrl += "&accessibilityfeatures=" + input.Accessibility_Features;
            }
			if (!string.IsNullOrWhiteSpace(input.LocationType))
			{
				fullUrl += "&locationtype=" + input.LocationType;
			}
			if (input.Radius != null && (input.Radius.Value > 0 || input.CustomRadius.Value > 0))
			{
				fullUrl += "&radius=" + input.Radius;
			}
			if (input.CustomRadius != null && input.CustomRadius.Value > 0 && (input.Radius == null || input.CustomRadius.Value != input.Radius.Value))
			{
				fullUrl += "&customradius=" + input.CustomRadius;
			}
			if (input.Page != null && input.Page.Value > 1)
			{
				fullUrl += "&page=" + input.Page;
			}
			if (!string.IsNullOrWhiteSpace(input.Sort))
			{
				fullUrl += "&sort=" + input.Sort;
			}

			HttpCookie serviceSearchUrlCookie = new HttpCookie("serviceSearchUrl");
			serviceSearchUrlCookie.Value = fullUrl;
			serviceSearchUrlCookie.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Remove("serviceSearchUrl");
			Response.Cookies.Add(serviceSearchUrlCookie);

			input.PageSize = (model.Content as DtServices).PageSize;
			if (input.Radius == null && input.CustomRadius == null)
			{
				input.Radius = 10000;
			}
			else
			{
				if (input.Radius == null && input.CustomRadius != null)
				{
					input.Radius = input.CustomRadius;
				}
			}

			SearchService orgService = new SearchService();
			SearchAPIModel apiModel = orgService.GetSearch(input);
			SearchViewModel viewModel = new SearchViewModel(input);
			viewModel.SearchModel = apiModel;
			ViewBag.SearchViewModel = viewModel;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{Request.Url.Scheme}://{Request.Url.Host}");
            ViewBag.AboutClaimedUrl = client.GetStringAsync("/umbraco/api/navigation/GetAboutClaimedUrl").Result.TrimStart('\\', '"', '/').TrimEnd('"');
			ViewBag.CategoryList = orgService.GetCategories(apiModel.aggregations);
            ViewBag.AccessibilityFeaturesList = orgService.GetAccessibilityFeatures(apiModel.aggregations);
            ViewBag.CommunityGroupsList = orgService.GetCommunityGroups(apiModel.aggregations);
            
            DtConfiguration config = new DtConfiguration(model.Content.Root().DescendantOfType(DtConfiguration.ModelTypeAlias));
            ViewBag.DataGovernanceDisclaimer = config.DataDisclaimer;
            ViewBag.WhereLabelText = config.WhereLabelText;
            ViewBag.WherePlaceholderText = config.WherePlaceholderText;
            ViewBag.WhatLabelText = config.WhatLabelText;
            ViewBag.WhatPlaceholderText = config.WhatPlaceholderText;

            if (!String.IsNullOrEmpty(apiModel?.error))
            {
                ViewBag.Error = apiModel.error;
                return View("PostcodeError", model);
            }

            return View(model);
		}
    }
}