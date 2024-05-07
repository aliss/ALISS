using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.WebApi;

namespace ALISS.CMS.Controllers
{
    public class FooterColumn
    {
        public string Title { get; set; }
        public List<FooterLink> Links { get; set; }
        
        public FooterColumn()
        {
            Links = new List<FooterLink>();
        }
    }

    public class FooterLink
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }

    public class FooterModel
    {
        public string Copyright { get; set; }
        public List<FooterColumn> Columns { get; set; }

        public FooterModel()
        {
            Columns = new List<FooterColumn>();
        }
    }

    public class FooterController : UmbracoApiController
    {
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