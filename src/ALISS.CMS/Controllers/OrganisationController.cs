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
using ALISS.API.Models.API;
using ALISS.ApiServices.ApiServices;
using ALISS.ApiServices.ViewModels.Organisation;
using ALISS.CMS.Helpers;
using RestSharp;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using System.ComponentModel.DataAnnotations;
using ALISS.Business.Validators;
using System.Web.Http.Results;
using Umbraco.Core.Models.PublishedContent;
using System.Net.Http.Headers;

namespace ALISS.CMS.Controllers
{
    public class OrganisationController : ALISSBaseController
    {
        private string _apiBaseUrl = ConfigurationManager.AppSettings["Settings:AdminApiBaseUrl"];
        private string _adminBaseUrl = ConfigurationManager.AppSettings["Settings:AdminBaseUrl"];

        public OrganisationController() { }
       

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewOrganisation(string slug)
        {
			OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(slug);
            ViewBag.AddServiceLink = model.data != null ? $"{_adminBaseUrl}/Organisation/AddService/{model.data.id}" : "";
            string logo = model.data != null ? GenericHelpers.GetOrganisationLogo(model.data.id) : "";
            ViewBag.Logo = logo;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{Request.Url.Scheme}://{Request.Url.Host}");
            ViewBag.AboutClaimedUrl = client.GetStringAsync("/umbraco/api/navigation/GetAboutClaimedUrl").Result.TrimStart('\\', '"', '/').TrimEnd('"');
            ViewBag.SuggestUrl = $"{_adminBaseUrl}/AddToALISS/SuggestService/{model.data.id}";
            if (CurrentUser.UserProfileId == 0)
            {
                ViewBag.EditOrganisation = false;
            }
            else
            {
                if (model.data != null)
                {
                    ViewBag.EditOrganisation = GenericHelpers.CanUserEditOrganisation(CurrentUser.UserProfileId, model.data.id);
                    string editUrl = $"{_adminBaseUrl}/AddToALISS/OrganisationSummary/{model.data.id}";
                    if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.ToLower().Contains("admin"))
                    {
                        ViewBag.ReturnUrl = $"{editUrl}?returnUrl={Request.UrlReferrer}/Organisation/AllOrganisations";
                    }
                    else
                    {
                        ViewBag.ReturnUrl = $"{editUrl}?returnUrl={Request.Url}";
                    }
                }
            }
            ViewBag.Follow = false;

            return View("organisation", model.data);
        }

        [HttpGet]
        public ActionResult ImproveOrganisation(Guid id)
        {
            DtImproveThisListing improveThisListingNode = new DtImproveThisListing(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtImproveThisListing.ModelTypeAlias));
            OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(id);
            ViewBag.Organisation = model.data;

            return View("ImproveThisListing", improveThisListingNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ImproveOrganisation(Guid id, string name, string email, string message, string notes)
        {
            DtImproveThisListing improveThisListingNode = new DtImproveThisListing(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtImproveThisListing.ModelTypeAlias));
            OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(id);
            ViewBag.Organisation = model.data;

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
                    request.AddParameter("OrganisationId", id);
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
        public ActionResult ShareOrganisation(string slug)
        {
            DtShareThisPage shareThisPageNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisPage.ModelTypeAlias));
            OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(slug);
            ViewBag.Organisation = model.data;

            return View("ShareThisPage", shareThisPageNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ShareOrganisation(string slug, string to, string message, string notes)
        {
            DtShareThisPage shareThisPageNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisPage.ModelTypeAlias));
            OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(slug);
            ViewBag.Organisation = model.data;

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
                        string url = _apiBaseUrl + "organisation/PostShareOrganisation";
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
        public ActionResult ClaimOrganisation(string slug, bool requestToManage)
        {
            DtClaimOrganisation claimOrganisationNode = requestToManage
                ? new DtClaimOrganisation(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtManageOrganisation.ModelTypeAlias))
                : new DtClaimOrganisation(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtClaimOrganisation.ModelTypeAlias));
            OrganisationService orgService = new OrganisationService();
            OrganisationAPIModel model = orgService.GetOrganisation(slug);
            ViewBag.Organisation = model.data;

            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.UrlReferrer.ToString()}");
            }
            else
            {
                ViewBag.UserId = CurrentUser.UserProfileId;
                return View("ClaimOrganisation", claimOrganisationNode);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ClaimOrganisation(string slug, string role, string name, string phone, string requestLead, int userId, string understand, string notes, bool requestToManage)
        {
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.UrlReferrer.ToString()}");
            }
            else
            {
                DtClaimOrganisation claimOrganisationNode = requestToManage
                    ? new DtClaimOrganisation(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtManageOrganisation.ModelTypeAlias))
                    : new DtClaimOrganisation(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtClaimOrganisation.ModelTypeAlias));
                OrganisationService orgService = new OrganisationService();
                OrganisationAPIModel model = orgService.GetOrganisation(slug);
                ViewBag.Organisation = model.data;
                ViewBag.UserId = userId;

                if (!string.IsNullOrWhiteSpace(role) && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone))
                {
                    if (understand == "1")
                    {
                        PhoneNumberValidator phoneNumberValidator = new PhoneNumberValidator();
                        if (!phoneNumberValidator.IsValidPhoneNumber(phone))
                        {
                            ViewBag.Error = "Please enter a valid UK phone number.";
                            ViewBag.IsPhoneInvalid = true;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(notes))
                            {
                                ViewBag.Sent = true;
                            }
                            else
                            {
                                string url = _apiBaseUrl + "claim/PostClaim";
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

                return View("ClaimOrganisation", claimOrganisationNode);
            }
        }

    }
}