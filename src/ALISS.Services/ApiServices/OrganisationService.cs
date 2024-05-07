using ALISS.ApiServices.ViewModels.Organisation;
using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Configuration;
using ALISS.ApiServices.ViewModels.Service;

namespace ALISS.ApiServices.ApiServices
{
    public class OrganisationService
    {
        private const string _orgUrlPart = "organisations/";
        private readonly string apiBaseUrl;

        public OrganisationService()
		{
            apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
        }

        public OrganisationAPIModel GetOrganisation(Guid id)
        {
            return GetOrganisationToReturn($"{apiBaseUrl}{_orgUrlPart}{id}");
        }

        public OrganisationAPIModel GetOrganisation(string slug)
        {
            return GetOrganisationToReturn($"{apiBaseUrl}{_orgUrlPart}{slug}");
        }

        private OrganisationAPIModel GetOrganisationToReturn(string url)
		{
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string objText = reader.ReadToEnd();
                        OrganisationAPIModel model = (OrganisationAPIModel)js.Deserialize(objText, typeof(OrganisationAPIModel));

                        return model;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return new OrganisationAPIModel();
                }
            }

            return new OrganisationAPIModel();
        }
    }
}
