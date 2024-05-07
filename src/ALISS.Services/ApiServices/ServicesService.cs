using ALISS.ApiServices.ViewModels.Service;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace ALISS.ApiServices.ApiServices
{
    public class ServicesService
    {
        private const string _servUrlPart = "services/";
        private readonly string apiBaseUrl;

        public ServicesService()
        {
            apiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];
        }

        public ServiceAPIModel GetService(Guid id)
        {
            return GetServiceToReturn($"{apiBaseUrl}{_servUrlPart}{id}");
        }

        public ServiceAPIModel GetService(string slug)
        {
            return GetServiceToReturn($"{apiBaseUrl}{_servUrlPart}{slug}");
        }

        public ServiceAPIModel GetServiceToReturn(string url)
		{
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        var model = (ServiceAPIModel)js.Deserialize(objText, typeof(ServiceAPIModel));

                        return model;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return new ServiceAPIModel();
                }
            }

            return new ServiceAPIModel();
        }
    }
}
