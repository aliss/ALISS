using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ALISS.CMS.Models.Collection;
using ALISS.CMS.Models.User;
using ALISS.Models.Models;
using RestSharp;

namespace ALISS.CMS.Helpers
{
	public static class GenericHelpers
	{
		private static string _apiBaseUrl = ConfigurationManager.AppSettings["Settings:AdminApiBaseUrl"];

		public static CurrentUserViewModel GetLoggedInUser()
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			string alissAdminUser = System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"] != null
				? System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"].Value
				: "";
			CurrentUserViewModel currentUser = !string.IsNullOrEmpty(alissAdminUser)
				? (CurrentUserViewModel)js.Deserialize(Encoding.UTF8.GetString(Convert.FromBase64String(alissAdminUser)), typeof(CurrentUserViewModel))
				: new CurrentUserViewModel();

			return currentUser;
		}

		public static CollectionListingViewModel GetCollections(int userProfileId, int page = 1)
		{
			string url = $"{_apiBaseUrl}collection/GetCollectionsForUser?userProfileId={userProfileId}&page={page}";

			RestClient client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			JavaScriptSerializer js = new JavaScriptSerializer();

			return (CollectionListingViewModel)js.Deserialize(response.Content, typeof(CollectionListingViewModel));
		}

		public static CollectionViewModel GetCollection(Guid collectionId, int page = 1)
		{
			string url = $"{_apiBaseUrl}collection/GetCollection?collectionId={collectionId}&page={page}";

			RestClient client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			JavaScriptSerializer js = new JavaScriptSerializer();

			return (CollectionViewModel)js.Deserialize(response.Content, typeof(CollectionViewModel));
		}

		public static string GetOrganisationLogo(Guid organisationId)
        {
			string url = $"{_apiBaseUrl}organisation/GetOrganisationLogo?organisationId={organisationId}";

			RestClient client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			JavaScriptSerializer js = new JavaScriptSerializer();

			return response.Content != null ? (string)js.Deserialize(response.Content, typeof(string)) : "";
		}

        public static string GetServiceLogo(Guid serviceId)
        {
            string url = $"{_apiBaseUrl}service/GetServiceLogo?serviceId={serviceId}";

            RestClient client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JavaScriptSerializer js = new JavaScriptSerializer();

            return response.Content != null ? (string)js.Deserialize(response.Content, typeof(string)) : "";
        }

        public static bool CanUserEditOrganisation(int userProfileId, Guid organisationId)
        {
			string url = $"{_apiBaseUrl}organisation/GetCanUserEditOrganisation?userProfileId={userProfileId}&organisationId={organisationId}";

			RestClient client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			JavaScriptSerializer js = new JavaScriptSerializer();

			return response.Content != null ? (bool)js.Deserialize(response.Content, typeof(bool)) : false;
        }

		public static bool CanUserEditService(int userProfileId, Guid serviceId)
		{
            string url = $"{_apiBaseUrl}service/GetCanUserEditService?userProfileId={userProfileId}&serviceId={serviceId}";

            RestClient client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            JavaScriptSerializer js = new JavaScriptSerializer();

            return response.Content != null ? (bool)js.Deserialize(response.Content, typeof(bool)) : false;
        }
    }
}