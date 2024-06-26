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
	// Mixin Content Type with alias "cdtHasSearchSuggestions"
	/// <summary>Has Search Suggestions</summary>
	public partial interface ICdtHasSearchSuggestions : IPublishedElement
	{
		/// <summary>Search Suggestions</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtSearchSuggestion> SearchSuggestions { get; }
	}

	/// <summary>Has Search Suggestions</summary>
	[PublishedModel("cdtHasSearchSuggestions")]
	public partial class CdtHasSearchSuggestions : PublishedElementModel, ICdtHasSearchSuggestions
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const string ModelTypeAlias = "cdtHasSearchSuggestions";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<CdtHasSearchSuggestions, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public CdtHasSearchSuggestions(IPublishedElement content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Search Suggestions: Set up pre configured search suggestions
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("searchSuggestions")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtSearchSuggestion> SearchSuggestions => GetSearchSuggestions(this);

		/// <summary>Static getter for Search Suggestions</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtSearchSuggestion> GetSearchSuggestions(ICdtHasSearchSuggestions that) => that.Value<global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtSearchSuggestion>>("searchSuggestions");
	}
}
