using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Mvc;
using ALISS.CMS.Models.User;
using HtmlAgilityPack;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.WebApi;
using static Umbraco.Core.Constants;

namespace ALISS.CMS.Controllers
{
    public class GuidanceController : UmbracoApiController
    {
        /// <summary>
        /// Get the guidance content for a specific page
        /// /umbraco/api/guidance/GetGuidanceContent
        /// </summary>
        /// <returns>HTML output for a guidance section</returns>
        public JsonResult<string> GetGuidanceContent(string pageName)
        {
            string returnData = "";

            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";

                foreach (GuidanceOption guidance in config.Guidance)
                {
                    string bodyText = "";
                    if (guidance.ShowOnPages.Select(t => Regex.Replace(t, @"\s+", "")).Contains(Regex.Replace(pageName, @"\s+", ""), StringComparer.OrdinalIgnoreCase))
                    {
                        bodyText = guidance.BodyText.ToString();

                        if (!string.IsNullOrWhiteSpace(bodyText) && (bodyText.Contains("<a ") || bodyText.Contains("<img ")))
                        {
                            byte[] byteArray = Encoding.UTF8.GetBytes(bodyText);
                            MemoryStream stream = new MemoryStream(byteArray);
                            HtmlDocument doc = new HtmlDocument();
                            doc.Load(stream, Encoding.UTF8);

                            HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a[@href]");
                            if (links != null && links.Count > 0)
                            {
                                foreach (HtmlNode link in links)
                                {
                                    HtmlAttribute attribute = link.Attributes["href"];
                                    if (attribute.Value.StartsWith("/"))
                                    {
                                        attribute.Value = $"{hostAndScheme}{attribute.Value}";
                                    }
                                }
                            }

                            HtmlNodeCollection images = doc.DocumentNode.SelectNodes("//img[@src]");
                            if (images != null && images.Count > 0)
                            {
                                foreach (HtmlNode image in images)
                                {
                                    HtmlAttribute attribute = image.Attributes["src"];
                                    if (attribute.Value.StartsWith("/"))
                                    {
                                        attribute.Value = $"{hostAndScheme}{attribute.Value}";
                                    }
                                }
                            }

                            MemoryStream memoryStream = new MemoryStream();
                            doc.Save(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8);
                            bodyText = streamReader.ReadToEnd();
                        }
                    }
                    returnData += bodyText;
                }
            }
            catch (Exception ex)
            {
                returnData = "Error: " + ex.Message + " - " + ex.StackTrace;
            }

            return Json(returnData);
        }

        /// <summary>
        /// Get all the guidance content
        /// /umbraco/api/guidance/GetAllGuidanceContent
        /// </summary>
        /// <returns>HTML output for all guidance sections</returns>
        public JsonResult<List<GuidanceDataModel>> GetAllGuidanceContent()
        {
            List<GuidanceDataModel> allGuidance = new List<GuidanceDataModel>();

            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";

                foreach (GuidanceOption guidance in config.Guidance)
                {
                    string returnData = guidance.BodyText.ToString();
                    if (!string.IsNullOrWhiteSpace(returnData) && (returnData.Contains("<a ") || returnData.Contains("<img ")))
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(returnData);
                        MemoryStream stream = new MemoryStream(byteArray);
                        HtmlDocument doc = new HtmlDocument();
                        doc.Load(stream, Encoding.UTF8);

                        HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a[@href]");
                        if (links != null && links.Count > 0)
                        {
                            foreach (HtmlNode link in links)
                            {
                                HtmlAttribute attribute = link.Attributes["href"];
                                if (attribute.Value.StartsWith("/"))
                                {
                                    attribute.Value = $"{hostAndScheme}{attribute.Value}";
                                }
                            }
                        }

                        HtmlNodeCollection images = doc.DocumentNode.SelectNodes("//img[@src]");
                        if (images != null && images.Count > 0)
                        {
                            foreach (HtmlNode image in images)
                            {
                                HtmlAttribute attribute = image.Attributes["src"];
                                if (attribute.Value.StartsWith("/"))
                                {
                                    attribute.Value = $"{hostAndScheme}{attribute.Value}";
                                }
                            }
                        }

                        MemoryStream memoryStream = new MemoryStream();
                        doc.Save(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8);
                        returnData = streamReader.ReadToEnd();
                    }
                    allGuidance.Add(new GuidanceDataModel { ShowOnPages = guidance.ShowOnPages.ToList(), BodyText = returnData });
                }
            }
            catch (Exception)
            {
                allGuidance = new List<GuidanceDataModel>();
            }

            return Json(allGuidance);
        }
    }
}