using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedModels;
using System.Web.Mvc;
using System.Text;
using ALISS.ApiServices.ViewModels.Service;
using ALISS.ApiServices.ViewModels.Organisation;
using Umbraco.Web.Media.EmbedProviders;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ALISS.CMS.Extensions
{
    public static class HtmlExtensions
    {
        public static IHtmlString Metadata(this HtmlHelper htmlHelper, OrganisationModel content)
        {
            UmbracoContext umbracoContext = Current.UmbracoContext;
            IPublishedContent rootNode = umbracoContext.Content.GetAtRoot().First();
            DtConfiguration config = new DtConfiguration(rootNode.DescendantOrSelfOfType(DtConfiguration.ModelTypeAlias));

            HtmlStringUtilities htmlStringUtilities = new HtmlStringUtilities();
            string fixedDescription = !string.IsNullOrWhiteSpace(content.description)
				? content.description.Replace(Environment.NewLine, " ").Replace('"', '\'')
				: "";

            string title = $"{content.name}";
			string seoDescription = content.description;
			string ogTitle = $"{content.name} on ALISS.org";
			string ogDescription = fixedDescription.Length > 200 ? $"{fixedDescription.Substring(0, 200)}..." : fixedDescription;
			MediaWithCrops media = rootNode.Value<MediaWithCrops>("openGraphImage");
			string ogImageUrl = media != null
				? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}{media.MediaUrl()}"
                : $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}/ALISS.THEME/dist/img/promo-img.png";
            string twitter = config.Twitter;
			string canonical = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}/organisations/{content.id}";

			return htmlHelper.Metadata(title, seoDescription, ogTitle, ogDescription, ogImageUrl, twitter, canonical, null);
		}

		public static IHtmlString Metadata(this HtmlHelper htmlHelper, ServiceModel content)
		{
			UmbracoContext umbracoContext = Current.UmbracoContext;
			IPublishedContent rootNode = umbracoContext.Content.GetAtRoot().First();
			DtConfiguration config = new DtConfiguration(rootNode.DescendantOrSelfOfType(DtConfiguration.ModelTypeAlias));

            HtmlStringUtilities htmlStringUtilities = new HtmlStringUtilities();
            string fixedDescription = !string.IsNullOrWhiteSpace(content.description)
                ? content.description.Replace(Environment.NewLine, " ").Replace('"', '\'')
                : "";

            string title = $"{content.name}";
			string seoDescription = $"{content.name} ({content.organisation.name}) {string.Join(", ", content.categories.Select(x => x.name).ToList())} - {fixedDescription}";
			string ogTitle = $"{content.name} on ALISS.org";
			string ogDescription = fixedDescription.Length > 200 ? $"{fixedDescription.Substring(0, 200)}..." : fixedDescription;
            MediaWithCrops media = rootNode.Value<MediaWithCrops>("openGraphImage");
            string ogImageUrl = media != null
                ? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}{media.MediaUrl()}"
                : $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}/ALISS.THEME/dist/img/promo-img.png";
            string twitter = config.Twitter;
            string canonical = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}/services/{content.id}";

            return htmlHelper.Metadata(title, seoDescription, ogTitle, ogDescription, ogImageUrl, twitter, canonical, null);
        }

        public static IHtmlString Metadata(this HtmlHelper htmlHelper, IPublishedContent content)
        {
            IPublishedContent rootNode = content.AncestorOrSelf<DtHomepage>();
            DtConfiguration config = new DtConfiguration(rootNode.DescendantOrSelfOfType(DtConfiguration.ModelTypeAlias));

            string title = content.GetValueWithDefault("seoPageTitle", content.GetValueWithDefault("title", content.Name));
            string seoDescription = content.GetValueWithDefault("seoPageDescription", "");
            string ogTitle = content.GetValueWithDefault("openGraphPageTitle", title);
            string ogDescription = content.GetValueWithDefault("openGraphPageDescription", seoDescription);
            if (ogDescription.Length > 120)
            {
                ogDescription = $"{ogDescription.Substring(0, 120).Replace('"', '\'')}...";
            }
            MediaWithCrops media = content.Value<MediaWithCrops>("openGraphImage", fallback: Fallback.ToAncestors);
            string ogImageUrl = media != null
                ? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}{media.MediaUrl()}"
                : $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}/ALISS.THEME/dist/img/promo-img.png";
            string twitter = config.Twitter;
            string robotsNoIndex = content.HasProperty("robotsNoIndex") ? content.Value<bool>("robotsNoIndex") ? "noindex" : "" : "";
            string robotsNoFollow = content.HasProperty("robotsNoFollow") ? content.Value<bool>("robotsNoFollow") ? "nofollow" : "" : "";
            string robots = $"{robotsNoIndex},{robotsNoFollow}".TrimEnd(',');

            return htmlHelper.Metadata(title, seoDescription, ogTitle, ogDescription, ogImageUrl, twitter, content.Url(mode: UrlMode.Absolute), robots);
        }

        public static IHtmlString Metadata(this HtmlHelper htmlHelper, string title, string seoDescription, string ogTitle, string ogDescription, string ogImageUrl, string twitter, string canonical, string robots)
        {
            if (string.IsNullOrWhiteSpace(seoDescription))
            {
                seoDescription = "ALISS (A Local Information System for Scotland) is a system to help you find help and support close to you when you need it most.";
                if (string.IsNullOrWhiteSpace(ogDescription))
                {
                    ogDescription = seoDescription;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<meta charset=\"UTF-8\">");

            // Meta: SEO
            sb.AppendLine($"<title>{title} | ALISS</title>");
            sb.AppendLine($"<meta name=\"description\" content=\"{seoDescription}\">");
            sb.AppendLine($"<meta name=\"author\" content=\"ALISS\">");
            if (!string.IsNullOrWhiteSpace(robots))
            {
                sb.AppendLine($"<meta name=\"robots\" content=\"{robots}\"");
            }

            // Meta: Open Graph
            sb.AppendLine($"<meta property=\"og:title\" content=\"{ogTitle}\">");
            sb.AppendLine($"<meta property=\"og:description\" content=\"{ogDescription}\">");
            sb.AppendLine($"<meta property=\"og:locale\" content=\"en_GB\">");
            sb.AppendLine($"<meta property=\"og:type\" content=\"website\">");
            sb.AppendLine($"<meta property=\"og:site_name\" content=\"ALISS\">");
            sb.AppendLine($"<meta property=\"og:image\" content=\"{ogImageUrl}\">");
            sb.AppendLine($"<meta property=\"og:image:secure_url\" content=\"{ogImageUrl}\">");

            // Meta: Twitter
            sb.AppendLine($"<meta name=\"twitter:title\" content=\"{ogTitle}\">");
            sb.AppendLine($"<meta name=\"twitter:description\" content=\"{ogDescription}\">");
            sb.AppendLine($"<meta name=\"twitter:site\" content=\"@{twitter}\">");
            sb.AppendLine($"<meta name=\"twitter:creator\" content=\"@{twitter}\">");
            sb.AppendLine($"<meta name=\"twitter:card\" content=\"summary_large_image\">");
            sb.AppendLine($"<meta name=\"twitter:image\" content=\"{ogImageUrl}\">");

            // Meta: Device Optimization
            sb.AppendLine($"<meta name=\"HandheldFriendly\" content=\"True\">");
            sb.AppendLine($"<meta name=\"MobileOptimized\" content=\"320\">");
            sb.AppendLine($"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"<meta name=\"apple-mobile-web-app-capable\" content=\"yes\">");
            sb.AppendLine($"<meta name=\"format-detection\" content=\"telephone=no\"> ");

            sb.AppendLine($"<link rel=\"canonical\" href=\"{canonical}\" />");

            return new HtmlString(sb.ToString());
        }

        public static string HtmlSubstring(string description, int length)
        {
            StringBuilder sb = new StringBuilder();

            var substring = description.Substring(0, length + 8);
            // 8 in case the tag is <strong>
            for (int i = 0; i < 8; i++)
            {
                if (substring[length + 7 - i] == '>')
                {
                    substring = description.Substring(0, length + 8 - i);
                }
                else if (substring[length + 7 - i] == '<')
                {
                    substring = description.Substring(0, length + 7 - i);
                }
            }

            sb.Append(substring);
            sb.Append("...");

            var tagList = new List<string>();
            string pattern = @"<\/?[a-z]*>";

            var matches = Regex.Matches(substring, pattern);

            for (int i = 0; i < matches.Count; i++)
            {
                tagList.Add(matches[i].ToString());
            }
            var startedTags = new List<string>();
            foreach (string tag in tagList)
            {
                if (tag[1] != '/')
                {
                    startedTags.Add(tag);
                }
                else
                {
                    startedTags.RemoveAt(startedTags.Count - 1);
                }
            }
            startedTags.Reverse();
            foreach (var unclosedTag in startedTags)
            {
                sb.Append(unclosedTag.Insert(1, "/"));
            }

            return sb.ToString();
        }
    }
}
