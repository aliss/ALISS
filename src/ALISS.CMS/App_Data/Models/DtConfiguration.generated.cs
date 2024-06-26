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
	/// <summary>Configuration</summary>
	[PublishedModel("dtConfiguration")]
	public partial class DtConfiguration : PublishedContentModel, ICdtHasContactInfo, ICdtHasDataGovernance, ICdtHasGuidance, ICdtHasOrganisationConfiguration, ICdtHasSearchFilters, ICdtHasServiceConfiguration, ICdtHasSiteNavigation
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const string ModelTypeAlias = "dtConfiguration";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<DtConfiguration, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public DtConfiguration(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Address: Shown in the contact panel of content pages (About ALISS etc.)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("address")]
		public virtual string Address => global::Umbraco.Web.PublishedModels.CdtHasContactInfo.GetAddress(this);

		///<summary>
		/// Contact Scotland
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("contactScotland")]
		public virtual string ContactScotland => global::Umbraco.Web.PublishedModels.CdtHasContactInfo.GetContactScotland(this);

		///<summary>
		/// Email Address
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("emailAddress")]
		public virtual string EmailAddress => global::Umbraco.Web.PublishedModels.CdtHasContactInfo.GetEmailAddress(this);

		///<summary>
		/// Phone Number
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("phoneNumber")]
		public virtual string PhoneNumber => global::Umbraco.Web.PublishedModels.CdtHasContactInfo.GetPhoneNumber(this);

		///<summary>
		/// Twitter
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("twitter")]
		public virtual string Twitter => global::Umbraco.Web.PublishedModels.CdtHasContactInfo.GetTwitter(this);

		///<summary>
		/// Bulk Review Months: How many months ahead should an email include?
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("bulkReviewMonths")]
		public virtual int BulkReviewMonths => global::Umbraco.Web.PublishedModels.CdtHasDataGovernance.GetBulkReviewMonths(this);

		///<summary>
		/// Data Disclaimer: A message to be displayed below the services page filters detailing the review process and data moved to the end of the search results.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("dataDisclaimer")]
		public virtual string DataDisclaimer => global::Umbraco.Web.PublishedModels.CdtHasDataGovernance.GetDataDisclaimer(this);

		///<summary>
		/// Email 1 Notification Months: How many months since last review will the first email be sent?
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("email1NotificationMonths")]
		public virtual int Email1NotificationMonths => global::Umbraco.Web.PublishedModels.CdtHasDataGovernance.GetEmail1NotificationMonths(this);

		///<summary>
		/// Email 2 Notification Months: How many months since the first email will the second email be sent?
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("email2NotificationMonths")]
		public virtual int Email2NotificationMonths => global::Umbraco.Web.PublishedModels.CdtHasDataGovernance.GetEmail2NotificationMonths(this);

		///<summary>
		/// Email 3 Notification Months: How many months since the second email will the third email be sent?
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("email3NotificationMonths")]
		public virtual int Email3NotificationMonths => global::Umbraco.Web.PublishedModels.CdtHasDataGovernance.GetEmail3NotificationMonths(this);

		///<summary>
		/// Guidance
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("guidance")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.GuidanceOption> Guidance => global::Umbraco.Web.PublishedModels.CdtHasGuidance.GetGuidance(this);

		///<summary>
		/// Actions Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationActionsCopy")]
		public virtual global::System.Web.IHtmlString OrganisationActionsCopy => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationActionsCopy(this);

		///<summary>
		/// Actions Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationActionsTitle")]
		public virtual string OrganisationActionsTitle => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationActionsTitle(this);

		///<summary>
		/// Contact Info Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationContactInfoCopy")]
		public virtual global::System.Web.IHtmlString OrganisationContactInfoCopy => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationContactInfoCopy(this);

		///<summary>
		/// Contact Info Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationContactInfoTitle")]
		public virtual string OrganisationContactInfoTitle => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationContactInfoTitle(this);

		///<summary>
		/// Improve Listing Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationImproveListingCopy")]
		public virtual global::System.Web.IHtmlString OrganisationImproveListingCopy => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationImproveListingCopy(this);

		///<summary>
		/// Improve Listing Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("organisationImproveListingTitle")]
		public virtual string OrganisationImproveListingTitle => global::Umbraco.Web.PublishedModels.CdtHasOrganisationConfiguration.GetOrganisationImproveListingTitle(this);

		///<summary>
		/// What Label Text: Text to be shown above the filter text field.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("whatLabelText")]
		public virtual string WhatLabelText => global::Umbraco.Web.PublishedModels.CdtHasSearchFilters.GetWhatLabelText(this);

		///<summary>
		/// What Placeholder Text: Text to be shown inside the filter text field when blank.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("whatPlaceholderText")]
		public virtual string WhatPlaceholderText => global::Umbraco.Web.PublishedModels.CdtHasSearchFilters.GetWhatPlaceholderText(this);

		///<summary>
		/// Where Label Text: Text to be shown above the filter text field.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("whereLabelText")]
		public virtual string WhereLabelText => global::Umbraco.Web.PublishedModels.CdtHasSearchFilters.GetWhereLabelText(this);

		///<summary>
		/// Where Placeholder Text: Text to be shown inside the filter text field when blank.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("wherePlaceholderText")]
		public virtual string WherePlaceholderText => global::Umbraco.Web.PublishedModels.CdtHasSearchFilters.GetWherePlaceholderText(this);

		///<summary>
		/// Actions Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceActionsCopy")]
		public virtual global::System.Web.IHtmlString ServiceActionsCopy => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceActionsCopy(this);

		///<summary>
		/// Actions Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceActionsTitle")]
		public virtual string ServiceActionsTitle => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceActionsTitle(this);

		///<summary>
		/// Contact Info Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceContactInfoCopy")]
		public virtual global::System.Web.IHtmlString ServiceContactInfoCopy => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceContactInfoCopy(this);

		///<summary>
		/// Contact Info Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceContactInfoTitle")]
		public virtual string ServiceContactInfoTitle => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceContactInfoTitle(this);

		///<summary>
		/// Improve Listing Copy
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceImproveListingCopy")]
		public virtual global::System.Web.IHtmlString ServiceImproveListingCopy => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceImproveListingCopy(this);

		///<summary>
		/// Improve Listing Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("serviceImproveListingTitle")]
		public virtual string ServiceImproveListingTitle => global::Umbraco.Web.PublishedModels.CdtHasServiceConfiguration.GetServiceImproveListingTitle(this);

		///<summary>
		/// About Claimed Page: Select the page that will be linked to from the claims disclaimer
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("aboutClaimedPage")]
		public virtual global::Umbraco.Core.Models.PublishedContent.IPublishedContent AboutClaimedPage => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetAboutClaimedPage(this);

		///<summary>
		/// Copyright
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("copyright")]
		public virtual string Copyright => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetCopyright(this);

		///<summary>
		/// Data Standards PDF
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("dataStandardsPdf")]
		public virtual global::Umbraco.Core.Models.MediaWithCrops DataStandardsPdf => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetDataStandardsPdf(this);

		///<summary>
		/// Feedback URL
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("feedbackUrl")]
		public virtual global::Umbraco.Web.Models.Link FeedbackUrl => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetFeedbackUrl(this);

		///<summary>
		/// Footer Navigation Columns
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("footerNavigationColumns")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtFooterNavigationColumn> FooterNavigationColumns => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetFooterNavigationColumns(this);

		///<summary>
		/// Header Navigation
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("headerNavigation")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.Models.Link> HeaderNavigation => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetHeaderNavigation(this);

		///<summary>
		/// Header Navigation Items
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("headerNavigationItems")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.HeaderLinkItem> HeaderNavigationItems => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetHeaderNavigationItems(this);

		///<summary>
		/// Privacy Policy PDF
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("privacyPolicyPdf")]
		public virtual global::Umbraco.Core.Models.MediaWithCrops PrivacyPolicyPdf => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetPrivacyPolicyPdf(this);

		///<summary>
		/// Social Links
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("socialLinks")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Web.PublishedModels.CdtSocialLinkItem> SocialLinks => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetSocialLinks(this);

		///<summary>
		/// Terms & Conditions PDF
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.18.3")]
		[ImplementPropertyType("termsConditionsPdf")]
		public virtual global::Umbraco.Core.Models.MediaWithCrops TermsConditionsPdf => global::Umbraco.Web.PublishedModels.CdtHasSiteNavigation.GetTermsConditionsPdf(this);
	}
}
