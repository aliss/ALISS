using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Organisation;
using ALISS.ApiServices.ViewModels.Service;
using ALISS.CMS.Helpers;
using ALISS.CMS.Models.Collection;
using RestSharp;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class ServiceController : ALISSBaseController
    {
        private string _apiBaseUrl = ConfigurationManager.AppSettings["Settings:AdminApiBaseUrl"];
        private string _adminBaseUrl = ConfigurationManager.AppSettings["Settings:AdminBaseUrl"];

        public ServiceController() { }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewService(string slug)
        {
			ServicesService serviceService = new ServicesService();
			ServiceAPIModel model = serviceService.GetService(slug);
            string logo = model.data != null ? GenericHelpers.GetServiceLogo(model.data.id) : "";
            ViewBag.Logo = logo;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{Request.Url.Scheme}://{Request.Url.Host}");
            ViewBag.AboutClaimedUrl = client.GetStringAsync("/umbraco/api/navigation/GetAboutClaimedUrl").Result.TrimStart('\\', '"', '/').TrimEnd('"');

            if (CurrentUser.UserProfileId == 0)
            {
                ViewBag.EditOrganisation = false;
            }
            else
            {
                if (model.data != null)
                {
                    ViewBag.EditService = GenericHelpers.CanUserEditService(CurrentUser.UserProfileId, model.data.id);
                    string editUrl = $"{_adminBaseUrl}/AddToALISS/Summary/{model.data.id}";
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.ToLower().Contains("admin"))
                    {
                        ViewBag.ReturnUrl = $"{editUrl}?returnUrl={Request.UrlReferrer}/Service/AllServices";
                    }
                    else
                    {
                        ViewBag.ReturnUrl = $"{editUrl}?returnUrl={Request.Url}";
                    }

                    
                    model.data.accessibility_features = model.data.accessibility_features?.OrderBy(af => af.location_id).ThenBy(af => af.name);
                    model.data.categories = model.data.categories?.OrderBy(af => af.name);
                    model.data.community_groups = model.data.community_groups?.OrderBy(af => af.name);
                }
            }
            ViewBag.Follow = false;

            return View("service", model.data);
        }

        [HttpGet]
        public ActionResult SaveService(string slug)
        {
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.Url.ToString()}");
            }
            else
            {
                DtSaveThisService saveThisServiceNode = new DtSaveThisService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtSaveThisService.ModelTypeAlias));
                ServicesService serviceService = new ServicesService();
                ServiceAPIModel model = serviceService.GetService(slug);
            
                ViewBag.Service = model.data;
                ViewBag.CollectionListing = GenericHelpers.GetCollections(CurrentUser.UserProfileId);

                return View("SaveThisService", saveThisServiceNode);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult SaveService(string slug, string collection, string newCollectionName, string notes)
		{
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.Url.ToString()}");
            }
            else
            {
                DtSaveThisService saveThisServiceNode = new DtSaveThisService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtSaveThisService.ModelTypeAlias));
                ServicesService serviceService = new ServicesService();
                ServiceAPIModel model = serviceService.GetService(slug);

                ViewBag.Service = model.data;
                ViewBag.CollectionListing = GenericHelpers.GetCollections(CurrentUser.UserProfileId);

                if (!string.IsNullOrWhiteSpace(collection))
                {
                    if (collection == "new" && string.IsNullOrWhiteSpace(newCollectionName))
                    {
                        ViewBag.sent = false;
                        ViewBag.Error = "You must enter a new collection name!";
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(notes))
                        {
                            ViewBag.Sent = true;
                        }
                        else
                        {
                            string url = _apiBaseUrl + "collection/PostAddServiceToCollection";
                            RestClient client = new RestClient(url);
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("content-type", "application/x-www-form-urlencoded");
                            if (collection != "new")
                            {
                                request.AddParameter("CollectionId", collection);
                                ViewBag.CollectionId = collection;
                            }
                            else
                            {
                                request.AddParameter("CollectionName", newCollectionName);
                            }
                            request.AddParameter("ServiceId", model.data.id);
                            request.AddParameter("UserProfileId", CurrentUser.UserProfileId);
                            IRestResponse response = client.Execute(request);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                Guid collectionId = (Guid)js.Deserialize(response.Content, typeof(Guid));
                                if (collectionId.ToString() == Guid.Empty.ToString())
                                {
                                    ViewBag.Sent = false;
                                    ViewBag.Error = "You already have a collection with this name!";
                                }
                                else
                                {
                                    ViewBag.Sent = true;
                                    ViewBag.CollectionId = collectionId.ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "'You must specify a collection!";
                }

                return View("SaveThisService", saveThisServiceNode);
            }
        }

        [HttpGet]
        public ActionResult ImproveService(Guid id)
		{
            DtImproveThisListing improveThisListingNode = new DtImproveThisListing(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtImproveThisListing.ModelTypeAlias));
            ServicesService serviceService = new ServicesService();
            ServiceAPIModel model = serviceService.GetService(id);

            ViewBag.Service = model.data;

            return View("ImproveThisListing", improveThisListingNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ImproveService(Guid id, string name, string email, string message, string notes)
        {
            DtImproveThisListing improveThisListingNode = new DtImproveThisListing(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtImproveThisListing.ModelTypeAlias));
            ServicesService serviceService = new ServicesService();
            ServiceAPIModel model = serviceService.GetService(id);

            ViewBag.Service = model.data;

            if (!string.IsNullOrWhiteSpace(message))
            {
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    ViewBag.Sent = true;
                }
                else
                {
                    string url = _apiBaseUrl + "improvements/PostImprovement";
                    RestClient client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("ServiceId", id);
                    request.AddParameter("SuggestedImprovement", message);
                    request.AddParameter("Name", name);
                    request.AddParameter("Email", email);
                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.Sent = true;
                    }
                }
            }
            else
            {
                ViewBag.Error = "Please complete all required fields.";
            }

            return View("ImproveThisListing", improveThisListingNode);
        }

        [HttpGet]
        public ActionResult ShareService(string slug)
        {
            DtShareThisPage shareThisPageNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisPage.ModelTypeAlias));
            ServicesService serviceService = new ServicesService();
            ServiceAPIModel model = serviceService.GetService(slug);

            ViewBag.Service = model.data;

            return View("ShareThisPage", shareThisPageNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ShareService(string slug, string to, string message, string notes)
        {
            DtShareThisPage shareThisPageNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisPage.ModelTypeAlias));
            ServicesService serviceService = new ServicesService();
            ServiceAPIModel model = serviceService.GetService(slug);

            ViewBag.Service = model.data;

            if (!string.IsNullOrWhiteSpace(to))
            {
				try
				{
                    MailAddress mailAddress = new MailAddress(to);

                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        ViewBag.Sent = true;
                    }
                    else
                    {
                        string url = _apiBaseUrl + "service/PostShareService";
                        RestClient client = new RestClient(url);
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddParameter("ServiceSlug", slug);
                        request.AddParameter("Message", message);
                        request.AddParameter("Email", to);
                        IRestResponse response = client.Execute(request);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Sent = true;
                        }
                    }
                }
                catch (FormatException)
				{
                    ViewBag.Error = "You must provide a valid email address!";
                }
            }
            else
            {
                ViewBag.Error = "You must provide an email address to send to!";
            }

            return View("ShareThisPage", shareThisPageNode);
        }

        [HttpGet]
        public ActionResult ClaimService(string slug, bool requestToManage)
        {
            DtClaimService claimServiceNode = requestToManage
                ? new DtClaimService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtManageService.ModelTypeAlias))
                : new DtClaimService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtClaimService.ModelTypeAlias));
            ServicesService orgService = new ServicesService();
            ServiceAPIModel model = orgService.GetService(slug);
            ViewBag.Service = model.data;

            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.UrlReferrer.ToString()}");
            }
            else
            {
                ViewBag.UserId = CurrentUser.UserProfileId;
                return View("ClaimService", claimServiceNode);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ClaimService(string slug, string role, string name, string phone, string requestLead, int userId, string understand, string notes, bool requestToManage)
        {
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.UrlReferrer.ToString()}");
            }
            else
            {
                DtClaimService claimServiceNode = requestToManage
                    ? new DtClaimService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtManageService.ModelTypeAlias))
                    : new DtClaimService(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtClaimService.ModelTypeAlias));
                ServicesService orgService = new ServicesService();
                ServiceAPIModel model = orgService.GetService(slug);
                ViewBag.Service = model.data;
                ViewBag.UserId = userId;

                if (!string.IsNullOrWhiteSpace(role) && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone))
                {
                    if (understand == "1")
                    {
                        phone = phone.Replace(" ", "");
                        if (phone.All(char.IsDigit))
                        {
                            string phoneRegex = @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$";
                            if (Regex.IsMatch(phone, phoneRegex))
                            {
                                if (!string.IsNullOrWhiteSpace(notes))
                                {
                                    ViewBag.Sent = true;
                                }
                                else
                                {
                                    string url = _apiBaseUrl + "claim/PostServiceClaim";
                                    RestClient client = new RestClient(url);
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                                    request.AddParameter("Slug", slug);
                                    request.AddParameter("RepresentativeRole", role);
                                    request.AddParameter("RepresentativeName", name);
                                    request.AddParameter("RepresentativePhone", phone);
                                    request.AddParameter("RequestLeadClaimant", requestLead == "1");
                                    request.AddParameter("ClaimedUserId", userId);
                                    IRestResponse response = client.Execute(request);

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        ViewBag.Sent = true;
                                    }
                                }
                            }
                            else
                            {
                                // Invalid phone number
                                ViewBag.Error = "You must provide a valid UK phone number!";
                                ViewBag.IsPhoneInvalid = true;
                            }
                        }
                        else
                        {
                            // Invalid phone number
                            ViewBag.Error = "You must only enter numeric digits as a phone number";
                            ViewBag.IsPhoneInvalid = true;
                        }
                    }
                    else
                    {
                        ViewBag.Error = "You must tick the box to indicate you understand and accept the responsibility of being the claimed user of an organisation";
                    }
                }
                else
                {
                    ViewBag.Error = "Please complete all required fields.";
                }

                return View("ClaimService", claimServiceNode);
            }
        }
    }
}