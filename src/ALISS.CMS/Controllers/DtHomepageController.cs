using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ALISS.CMS.Models;
using Newtonsoft.Json;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class DtHomepageController : RenderMvcController
    {
        private readonly string AlissApiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];

        public DtHomepageController() { }

		public override ActionResult Index(ContentModel model)
		{
            DtHomepage homepage = model.Content as DtHomepage;
            IEnumerable<CdtSearchSuggestion> searchSuggestions = homepage.SearchSuggestions.Where(x => !x.Disabled);
            List<SearchSuggestion> activeSearchSuggestions = new List<SearchSuggestion>();

            if (searchSuggestions.Any())
            {
                HttpClient client = new HttpClient();
                UriBuilder ub = new UriBuilder($"{AlissApiBaseUrl}categories");
                HttpResponseMessage response = client.GetAsync(ub.Uri).Result;
                response.EnsureSuccessStatusCode();
                string jsonResult = response.Content.ReadAsStringAsync().Result;
                dynamic allCategories = JsonConvert.DeserializeObject(jsonResult);

                string categoryName = "";
                string categorySlug = "";

                foreach (CdtSearchSuggestion searchSuggestion in searchSuggestions)
                {
                    dynamic category = searchSuggestion.Category[0];
                    foreach (var c in allCategories.data)
                    {
                        if (c.id == category.id)
                        {
                            categoryName = c.name;
                            categorySlug = c.slug;
                            break;
                        }
                        foreach (var c2 in c.sub_categories)
                        {
                            if (c2.id == category.id)
                            {
                                categoryName = c2.name;
                                categorySlug = c2.slug;
                                break;
                            }
                            foreach (var c3 in c2.sub_categories)
                            {
                                if (c3.id == category.id)
                                {
                                    categoryName = c3.name;
                                    categorySlug = c3.slug;
                                    break;
                                }
                            }
                        }
                    }

                    activeSearchSuggestions.Add(new SearchSuggestion
                    {
                        Title = searchSuggestion.Title,
                        Image = searchSuggestion.Image.GetCropUrl("Search_Suggestion_Image"),
                        ImageAltText = searchSuggestion.ImageAltText,
                        Summary = searchSuggestion.Summary,
                        CategoryId = category.id,
                        CategoryName = categoryName,
                        CategorySlug = categorySlug
                    });
                }
            }

            ViewBag.SearchSuggestions = activeSearchSuggestions;

            return View(model);
		}
    }
}