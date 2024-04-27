using System.Web.Mvc;
using System.Web.Routing;

namespace ALISS.Admin.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DataInputOrganisation",
                url: "AddToAliss/Organisation/{id}",
                defaults: new { controller = "DataInputOrganisation", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputService",
                url: "AddToAliss/Service/{id}",
                defaults: new { controller = "DataInputService", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputSuggestedService",
                url: "AddToAliss/SuggestService/{id}",
                defaults: new { controller = "DataInputService", action = "SuggestService", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
				name: "DataInputWhere",
				url: "AddToAliss/Where/{id}",
				defaults: new { controller = "DataInputWhere", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
			);

            routes.MapRoute(
                name: "DataInputCategories",
                url: "AddToAliss/Categories/{id}",
                defaults: new { controller = "DataInputCategories", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputWho",
                url: "AddToAliss/Who/{id}",
                defaults: new { controller = "DataInputWho", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputAccessibility",
                url: "AddToAliss/Accessibility/{id}",
                defaults: new { controller = "DataInputAccessibility", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputMedia",
                url: "AddToAliss/Media/{id}",
                defaults: new { controller = "DataInputMedia", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputSummary",
                url: "AddToAliss/Summary/{id}",
                defaults: new { controller = "DataInputSummary", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputOrganisationSummary",
                url: "AddToAliss/OrganisationSummary/{id}",
                defaults: new { controller = "DataInputOrganisationSummary", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputConfirmation",
                url: "AddToAliss/Confirmation/{id}",
                defaults: new { controller = "DataInputConfirmation", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "DataInputCancel",
                url: "AddToAliss/Cancel/{id}",
                defaults: new { controller = "DataInputCancel", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers.DataInputControllers" }
            );

            routes.MapRoute(
                name: "ServiceReviewSummary",
                url: "ServiceReviewSummary/{id}",
                defaults: new { controller = "ServiceReviewSummary", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ALISS.Admin.Web.Controllers" }
            );
        }
    }
}
