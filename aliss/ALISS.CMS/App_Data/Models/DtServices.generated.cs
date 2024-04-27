//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v8.18.3
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder.Embedded;

namespace Umbraco.Web.PublishedModels
{
	/// <summary>Services</summary>
	[PublishedModel("dtServices")]
	public partial class DtServices : PublishedContentModel, ICdtHasListingPageConfiguration, ICdtHasNavigation, ICdtHasSearchEngineOptimisation
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const string ModelTypeAlias = "dtServices";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<DtServices, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public DtServices(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Page Size: The number of results per page
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("pageSize")]
		public virtual int PageSize => global::Umbraco.Web.PublishedModels.CdtHasListingPageConfiguration.GetPageSize(this);

		///<summary>
		/// Use Content From: Will load the selected page’s content transparently at this node's location
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("umbracoInternalRedirectId")]
		public virtual global::Umbraco.Core.Models.PublishedContent.IPublishedContent UmbracoInternalRedirectId => global::Umbraco.Web.PublishedModels.CdtHasNavigation.GetUmbracoInternalRedirectId(this);

		///<summary>
		/// Redirect to Content: Will redirect to the selected content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("umbracoRedirect")]
		public virtual global::Umbraco.Core.Models.PublishedContent.IPublishedContent UmbracoRedirect => global::Umbraco.Web.PublishedModels.CdtHasNavigation.GetUmbracoRedirect(this);

		///<summary>
		/// Aliases: A comma separated list of alternate url's for this content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("umbracoUrlAlias")]
		public virtual string UmbracoUrlAlias => global::Umbraco.Web.PublishedModels.CdtHasNavigation.GetUmbracoUrlAlias(this);

		///<summary>
		/// Hide children from Sitemap: This node's children will be excluded from the generated sitemap, regardless of their individual setting
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("hideChildrenFromSitemap")]
		public virtual bool HideChildrenFromSitemap => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetHideChildrenFromSitemap(this);

		///<summary>
		/// Hide from Sitemap: This node will be excluded form the generated sitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("hideFromSitemap")]
		public virtual bool HideFromSitemap => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetHideFromSitemap(this);

		///<summary>
		/// Open Graph Image: The image used for Open Graph
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("openGraphImage")]
		public virtual global::Umbraco.Core.Models.MediaWithCrops OpenGraphImage => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetOpenGraphImage(this);

		///<summary>
		/// Open Graph Page Description: The page description for Open Graph (If blank, SEO Page Description will be used)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("openGraphPageDescription")]
		public virtual string OpenGraphPageDescription => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetOpenGraphPageDescription(this);

		///<summary>
		/// Open Graph Page Title: The page title for Open Graph (If blank, SEO Page Title will be used)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("openGraphPageTitle")]
		public virtual string OpenGraphPageTitle => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetOpenGraphPageTitle(this);

		///<summary>
		/// Robots "No Follow": Prevent Google from following links on this page
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("robotsNoFollow")]
		public virtual bool RobotsNoFollow => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetRobotsNoFollow(this);

		///<summary>
		/// Robots "No Index": Exclude this page from Google search results
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("robotsNoIndex")]
		public virtual bool RobotsNoIndex => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetRobotsNoIndex(this);

		///<summary>
		/// SEO Page Description: The page description for SEO
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("seoPageDescription")]
		public virtual string SeoPageDescription => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetSeoPageDescription(this);

		///<summary>
		/// SEO Page Title: The page title for SEO (If blank, Page Title will be used)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("seoPageTitle")]
		public virtual string SeoPageTitle => global::Umbraco.Web.PublishedModels.CdtHasSearchEngineOptimisation.GetSeoPageTitle(this);
	}
}
