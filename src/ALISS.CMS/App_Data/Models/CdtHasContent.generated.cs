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
	// Mixin Content Type with alias "cdtHasContent"
	/// <summary>Has Content</summary>
	public partial interface ICdtHasContent : IPublishedContent
	{
		/// <summary>Body Text</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		global::System.Web.IHtmlString BodyText { get; }
	}

	/// <summary>Has Content</summary>
	[PublishedModel("cdtHasContent")]
	public partial class CdtHasContent : PublishedContentModel, ICdtHasContent
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const string ModelTypeAlias = "cdtHasContent";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<CdtHasContent, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public CdtHasContent(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Body Text: Main content of the page To enter a new line rather than a new paragraph, press shift + enter
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("bodyText")]
		public virtual global::System.Web.IHtmlString BodyText => GetBodyText(this);

		/// <summary>Static getter for Body Text</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static global::System.Web.IHtmlString GetBodyText(ICdtHasContent that) => that.Value<global::System.Web.IHtmlString>("bodyText");
	}
}
