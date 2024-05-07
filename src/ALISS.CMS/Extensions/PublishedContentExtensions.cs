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

namespace ALISS.CMS.Extensions
{
	public static class PublishedContentExtensions
	{
		public static IEnumerable<KeyValuePair<string, Uri>> Breadcrumb(this IPublishedContent content, int fromIndex = 0)
		{
			return content
				.Ancestors()
				.Reverse()
				.Skip(fromIndex)
				.Select(x => new KeyValuePair<string, Uri>(x.Name, new Uri(x.Url(), UriKind.Relative)));
		}

		public static Uri GetMediaWithCrop(this IPublishedContent content, string propertyName, string cropAlias)
		{
			if (content.HasProperty(propertyName))
			{
				Image media = content.Value<Image>(propertyName);
				if (media != null)
				{
					return new Uri(media.GetCropUrl(cropAlias), UriKind.RelativeOrAbsolute);
				}
			}

			return null;
		}

		public static string GetValueWithDefault(this IPublishedContent content, string propertyName, string defaultValue)
		{
			return content.HasProperty(propertyName) && content.HasValue(propertyName)
				? content.Value<string>(propertyName)
				: defaultValue;
		}
	}
}