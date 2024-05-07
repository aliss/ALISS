using ALISS.CMS.Helpers;
using ALISS.CMS.Models.Collection;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace ALISS.CMS.Controllers
{
    public class CollectionController : ALISSBaseController
    {
        public CollectionController() { }

        private string _apiBaseUrl = ConfigurationManager.AppSettings["Settings:AdminApiBaseUrl"];

        [HttpGet]
        public ActionResult Collections()
        {
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.Url.ToString()}");
            }
            else
            {
                int page = int.Parse(Request.Params["page"] ?? "1");
                IPublishedContent collectionsNode = UmbracoContext.Content.GetAtRoot().First().DescendantOfType(DtCollections.ModelTypeAlias);

                ViewBag.CollectionListing = GenericHelpers.GetCollections(CurrentUser.UserProfileId, page);
                ViewBag.AdminUrl = ConfigurationManager.AppSettings["Settings:AdminBaseUrl"].ToString();

                return View("~/Views/dtCollections/DtCollections.cshtml", collectionsNode);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Collections(string action, string name, Guid? id, string notes)
        {
            if (CurrentUser.UserProfileId == 0)
            {
                return Redirect($"{ConfigurationManager.AppSettings["Settings:AdminBaseUrl"]}{ConfigurationManager.AppSettings["Settings:AdminLoginUrl"]}{Request.Url.ToString()}");
            }
            else
            {
                IPublishedContent collectionsNode = UmbracoContext.Content.GetAtRoot().First().DescendantOfType(DtCollections.ModelTypeAlias);

                switch (action)
                {
                    case "delete":
                        if (id != null && id.HasValue)
                        {
                            ViewBag.Sent = DeleteCollection(id.Value);
                        }
                        else
                        {
                            ViewBag.Error = "'You must provide a collection to delete!";
                        }
                        break;
                    case "create":
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            ViewBag.Status = CreateCollection(name, notes);
                        }
                        else
                        {
                            ViewBag.Status = "You must provide a collection name";
                        }
                        break;
                }

                ViewBag.CollectionListing = GenericHelpers.GetCollections(CurrentUser.UserProfileId);

                return View("~/Views/dtCollections/DtCollections.cshtml", collectionsNode);
            }
        }

        private string CreateCollection(string name, string notes)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    return "";
                }
                else
                {
                    string url = _apiBaseUrl + "collection/PostCollection";
                    RestClient client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("Name", name);
                    request.AddParameter("UserProfileId", CurrentUser.UserProfileId);
                    IRestResponse response = client.Execute(request);

                    return response.StatusCode != System.Net.HttpStatusCode.OK
                        ? "There was an error creating your new collection"
                        : response.Content.Contains("000000")
                            ? "This collection name already exists"
                            : "Your new collection was successfully created";
                }
            }
            else
            {
                return "You must provide a name for the collection";
            }
        }

        private bool DeleteCollection(Guid id)
        {
            string url = _apiBaseUrl + "collection/PostDeleteCollection";
            RestClient client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("collectionId", id);
            IRestResponse response = client.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        private bool DeleteService(Guid collectionId, Guid serviceId)
        {
            string url = _apiBaseUrl + "collection/PostRemoveServiceFromCollection";
            RestClient client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("CollectionId", collectionId);
            request.AddParameter("ServiceId", serviceId);
            request.AddParameter("UserProfileId", CurrentUser.UserProfileId);
            IRestResponse response = client.Execute(request);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        [HttpGet]
        public ActionResult Collection(Guid collectionId)
        {
            int page = int.Parse(Request.Params["page"] ?? "1");

            IPublishedContent collectionsNode = UmbracoContext.Content.GetAtRoot().First().DescendantOfType(DtCollections.ModelTypeAlias);

            ViewBag.Collection = GenericHelpers.GetCollection(collectionId, page);

            return View("collection", collectionsNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Collection(Guid collectionId, string action, Guid serviceId)
        {
            IPublishedContent collectionsNode = UmbracoContext.Content.GetAtRoot().First().DescendantOfType(DtCollections.ModelTypeAlias);

            ViewBag.Collection = GenericHelpers.GetCollection(collectionId);

            if (action == "delete")
            {
                ViewBag.Sent = DeleteService(collectionId, serviceId);
            }

            return View("collection", collectionsNode);
        }

        [HttpGet]
        public ActionResult EmailCollection(Guid id)
        {
            DtShareThisPage shareThisCollectionNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisCollection.ModelTypeAlias));

            ViewBag.Collection = GenericHelpers.GetCollection(id);

            return View("ShareThisCollection", shareThisCollectionNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EmailCollection(Guid id, string name, string email, string notes)
        {
            DtShareThisPage shareThisCollectionNode = new DtShareThisPage(Umbraco.ContentAtRoot().First().DescendantOrSelfOfType(DtShareThisCollection.ModelTypeAlias));

            ViewBag.Collection = GenericHelpers.GetCollection(id);

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "You must provide a name and an email address!";
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "You must provide a name!";
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "You must provide an emal address!";
            }
            else
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(email);

                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        ViewBag.Sent = true;
                    }
                    else
                    {
                        string url = _apiBaseUrl + "collection/PostEmailCollection";
                        RestClient client = new RestClient(url);
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddParameter("CollectionId", id);
                        request.AddParameter("RecipientName", name);
                        request.AddParameter("Email", email);
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

            return View("ShareThisCollection", shareThisCollectionNode);
        }
    }
}