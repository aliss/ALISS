using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;

namespace ALISS.CMS.Components
{
	public class RegisterCustomRoutesComponent : IComponent
	{
		public void Initialize()
		{
			RouteTable.Routes.MapRoute("ViewService", "services/{slug}", new
			{
				controller = "Service",
				action = "ViewService"
			});

			RouteTable.Routes.MapRoute("ImproveService", "services/{id}/improve", new
			{
				controller = "Service",
				action = "ImproveService"
			});

			RouteTable.Routes.MapRoute("ShareService", "services/{slug}/share", new
			{
				controller = "Service",
				action = "ShareService"
			});

            RouteTable.Routes.MapRoute("ClaimService", "services/{slug}/claim", new
            {
                controller = "Service",
                action = "ClaimService",
				requestToManage = false
            });

            RouteTable.Routes.MapRoute("ManageService", "services/{slug}/manage", new
            {
                controller = "Service",
                action = "ClaimService",
				requestToManage = true
            });

            RouteTable.Routes.MapRoute("SaveService", "services/{slug}/save", new
			{
				controller = "Service",
				action = "SaveService"
			});

			RouteTable.Routes.MapRoute("ViewOrganisation", "organisations/{slug}", new
			{
				controller = "Organisation",
				action = "ViewOrganisation"
			});

			RouteTable.Routes.MapRoute("ImproveOrganisation", "organisations/{id}/improve", new
			{
				controller = "Organisation",
				action = "ImproveOrganisation"
			});

			RouteTable.Routes.MapRoute("ShareOrganisation", "organisations/{slug}/share", new
			{
				controller = "Organisation",
				action = "ShareOrganisation"
			});

			RouteTable.Routes.MapRoute("ClaimOrganisation", "organisations/{slug}/claim", new
			{
				controller = "Organisation",
				action = "ClaimOrganisation",
				requestToManage = false
			});

            RouteTable.Routes.MapRoute("ManageOrganisation", "organisations/{slug}/manage", new
            {
                controller = "Organisation",
                action = "ClaimOrganisation",
				requestToManage = true
            });

            RouteTable.Routes.MapRoute("Collections", "collections", new
			{
				controller = "Collection",
				action = "Collections"
			});

			RouteTable.Routes.MapRoute("Collection", "collections/{collectionId}", new
			{
				controller = "Collection",
				action = "Collection"
			});

			RouteTable.Routes.MapRoute("EmailCollection", "collections/{id}/email", new
			{
				controller = "Collection",
				action = "EmailCollection"
			});

			RouteTable.Routes.MapRoute("Logout", "logout", new
			{
				controller = "Logout",
				action = "Index"
			});

			RouteTable.Routes.MapRoute("ServiceListing", "servicelisting", new
			{
				controller = "ServiceListing",
				action = "Index"
			});

            RouteTable.Routes.MapRoute("OrganisationListing", "organisationlisting", new
            {
                controller = "OrganisationListing",
                action = "Index"
            });

            RouteTable.Routes.MapRoute("Sitemap", "sitemap", new
			{
				controller = "Sitemap",
				action = "Index"
			});
		}

		public void Terminate()
		{
		}
	}
}