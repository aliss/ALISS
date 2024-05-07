using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.WebApi;

namespace ALISS.CMS.Controllers
{
    public class NavigationController : UmbracoApiController
    {
        /// <summary>
        /// Get the all URLs for site settings
        /// /umbraco/api/navigation/GetSiteSettingsDocs
        /// </summary>
        /// <returns>URL String</returns>
        public JsonResult<Dictionary<string, string>> GetSiteSettingsDocs()
        {
            Dictionary<string, string> returnData = new Dictionary<string, string>();

            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";

                if (config.AboutClaimedPage != null)
                {
                    IPublishedContent aboutClaimedNode = config.AboutClaimedPage;
                    returnData.Add("About Claimed", !aboutClaimedNode.Url().StartsWith("http") ? $"{hostAndScheme}{aboutClaimedNode.Url()}" : aboutClaimedNode.Url());
                }
                else
                {
                    returnData.Add("TermsAbout Claimed", "");
                }
                if (config.TermsConditionsPdf != null)
                {
                    MediaWithCrops termsNode = config.TermsConditionsPdf;
                    returnData.Add("Terms And Conditions", !termsNode.Url().StartsWith("http") ? $"{hostAndScheme}{termsNode.Url()}" : termsNode.Url());
                }
                else
                {
                    returnData.Add("Terms And Conditions", "");
                }
                if (config.DataStandardsPdf != null)
                {
                    MediaWithCrops dataStandardsNode = config.DataStandardsPdf;
                    returnData.Add("Data Standards", !dataStandardsNode.Url().StartsWith("http") ? $"{hostAndScheme}{dataStandardsNode.Url()}" : dataStandardsNode.Url());
                }
                else
                {
                    returnData.Add("Data Standards", "");
                }
                if (config.PrivacyPolicyPdf != null)
                {
                    MediaWithCrops privacyNode = config.PrivacyPolicyPdf;
                    returnData.Add("Privacy Policy", !privacyNode.Url().StartsWith("http") ? $"{hostAndScheme}{privacyNode.Url()}" : privacyNode.Url());
                }
                else
                {
                    returnData.Add("Privacy Policy", "");
                }
            }
            catch (Exception ex)
            {
                returnData.Add("Error", ex.Message + " - " + ex.StackTrace);
            }

            return Json(returnData);
        }

        /// <summary>
        /// Get the URL for the About Claimed page
        /// /umbraco/api/navigation/GetAboutClaimedUrl
        /// </summary>
        /// <returns>URL String</returns>
        public JsonResult<string> GetAboutClaimedUrl()
        {
            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";
                IPublishedContent aboutClaimedNode = config.AboutClaimedPage;

                return !aboutClaimedNode.Url().StartsWith("http")
                    ? Json($"{hostAndScheme}{aboutClaimedNode.Url()}")
                    : Json(aboutClaimedNode.Url());
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " - " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get the URL for the Terms & Conditions PDF
        /// /umbraco/api/navigation/GetTermsAndConditionsUrl
        /// </summary>
        /// <returns>URL String</returns>
        public JsonResult<string> GetTermsAndConditionsUrl()
        {
            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";
                var mediaNode = config.TermsConditionsPdf;

                return !mediaNode.Url().StartsWith("http")
                    ? Json($"{hostAndScheme}{mediaNode.Url()}")
                    : Json(mediaNode.Url());
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " - " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get the URL for the Data Standards PDF
        /// /umbraco/api/navigation/GetDataStandardsUrl
        /// </summary>
        /// <returns>URL String</returns>
        public JsonResult<string> GetDataStandardsUrl()
        {
            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";
                var mediaNode = config.DataStandardsPdf;

                return !mediaNode.Url().StartsWith("http")
                    ? Json($"{hostAndScheme}{mediaNode.Url()}")
                    : Json(mediaNode.Url());
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " - " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get the URL for the Privacy Plicy PDF
        /// /umbraco/api/navigation/GetPrivacyPolicyUrl
        /// </summary>
        /// <returns>URL String</returns>
        public JsonResult<string> GetPrivacyPolicyUrl()
        {
            try
            {
                DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
                string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";
                var mediaNode = config.PrivacyPolicyPdf;

                return !mediaNode.Url().StartsWith("http")
                    ? Json($"{hostAndScheme}{mediaNode.Url()}")
                    : Json(mediaNode.Url());
            }
            catch (Exception ex)
            {
                return Json(ex.Message + " - " + ex.StackTrace);
            }
        }

        public JsonResult<FooterModel> GetFooterData()
        {
            DtConfiguration config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));

            string hostAndScheme = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}";

            FooterModel footer = new FooterModel();

            footer.Copyright = config.Copyright;
            if ((config.FooterNavigationColumns != null && config.FooterNavigationColumns.Any()) || (config.SocialLinks != null && config.SocialLinks.Any()))
            {
                foreach (CdtFooterNavigationColumn column in config.FooterNavigationColumns)
                {
                    IList<Link> linksToShow = new List<Link>();
                    foreach (CdtFooterLinkItem item in column.NavigationLinks)
                    {
                        if (string.IsNullOrWhiteSpace(item.LinkVisibility) || item.LinkVisibility.ToLower().Contains("admin") || item.LinkVisibility.ToLower().Contains("both"))
                        {
                            linksToShow.Add(item.Link);
                        }
                    }

                    if (linksToShow.Any())
                    {
                        FooterColumn footerColumn = new FooterColumn();
                        footerColumn.Title = column.Title;

                        foreach (Link item in linksToShow)
                        {
                            if (item != null && !string.IsNullOrWhiteSpace(item.Url))
                            {
                                string fullUrl = !item.Url.StartsWith("http") ? $"{hostAndScheme}{item.Url}" : item.Url;
                                footerColumn.Links.Add(new FooterLink { Title = item.Name, Url = fullUrl, Type = "link" });
                            }
                        }

                        footer.Columns.Add(footerColumn);
                    }
                }
                
                if (config.SocialLinks.Any())
                {
                    IList<CdtSocialLinkItem> linksToShow = new List<CdtSocialLinkItem>();
                    foreach (CdtSocialLinkItem item in config.SocialLinks)
                    {
                        if (string.IsNullOrWhiteSpace(item.LinkVisibility) || item.LinkVisibility.ToLower().Contains("admin") || item.LinkVisibility.ToLower().Contains("both"))
                        {
                            linksToShow.Add(item);
                        }
                    }

                    if (linksToShow.Any())
                    {
                        FooterColumn footerColumn = new FooterColumn();
                        footerColumn.Title = "Find ALISS on social media";

                        foreach (CdtSocialLinkItem socialLink in linksToShow)
                        {
                            footerColumn.Links.Add(new FooterLink { Title = socialLink.LinkTitle, Url = socialLink.LinkUrl, Type = socialLink.LinkType });
                        }

                        footer.Columns.Add(footerColumn);
                    }
                }
            }

            return Json(footer);
        }
    }
}