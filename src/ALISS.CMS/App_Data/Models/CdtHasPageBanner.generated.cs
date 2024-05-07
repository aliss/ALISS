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
	// Mixin Content Type with alias "cdtHasPageBanner"
	/// <summary>Has Page Banner</summary>
	public partial interface ICdtHasPageBanner : IPublishedElement
	{
		/// <summary>Page Banner</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		global::Umbraco.Core.Models.MediaWithCrops PageBanner { get; }
	}

	/// <summary>Has Page Banner</summary>
	[PublishedModel("cdtHasPageBanner")]
	public partial class CdtHasPageBanner : PublishedElementModel, ICdtHasPageBanner
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const string ModelTypeAlias = "cdtHasPageBanner";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<CdtHasPageBanner, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public CdtHasPageBanner(IPublishedElement content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Page Banner
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("pageBanner")]
		public virtual global::Umbraco.Core.Models.MediaWithCrops PageBanner => GetPageBanner(this);

		/// <summary>Static getter for Page Banner</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static global::Umbraco.Core.Models.MediaWithCrops GetPageBanner(ICdtHasPageBanner that) => that.Value<global::Umbraco.Core.Models.MediaWithCrops>("pageBanner");
	}
}
