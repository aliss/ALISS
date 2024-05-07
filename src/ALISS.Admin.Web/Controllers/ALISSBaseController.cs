using ALISS.Admin.Web.Models;
using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ALISS.Admin.Web.Controllers
{
    public class ALISSBaseController : Controller
    {
        public CurrentUserViewModel CurrentUser;

        public ApplicationUserManager UserManager;

        public ALISSBaseController()
        {
            if (System.Web.HttpContext.Current.Request.Cookies["ALISSAdmin.User"] == null)
            {
                System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                if (System.Web.HttpContext.Current.Request.Cookies[".AspNet.ApplicationCookie"] != null)
                {
                    System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.RawUrl);
                }
            }

            UserManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            System.Security.Principal.IPrincipal user = System.Web.HttpContext.Current.User;
            UserProfileService upService = new UserProfileService();
            CurrentUser = upService.GetCurentLoggedInUser(user.Identity.Name);
            if (CurrentUser.UserProfileId != 0)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string currentUser = js.Serialize(CurrentUser);
                string urlHost = System.Web.HttpContext.Current.Request.Url.Host;
                urlHost = urlHost.Substring(urlHost.IndexOf('.') + 1);
                if (!urlHost.Contains("."))
				{
                    urlHost = System.Web.HttpContext.Current.Request.Url.Host;
                }
                HttpCookie cookie = new HttpCookie("ALISSAdmin.User")
                {
                    Domain = $".{urlHost}",
                    Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(currentUser))
                };

                ApplicationUser systemUser = UserManager.FindByName(CurrentUser.Username);
                ViewBag.IsAdmin = UserManager.IsInRole(systemUser.Id, RolesEnum.ALISSAdmin.ToString());
                ViewBag.IsEditor = UserManager.IsInRole(systemUser.Id, RolesEnum.Editor.ToString());
                ViewBag.DisplayName = $"{CurrentUser.Name} ({CurrentUser.Username})";
                ViewBag.ProfileId = CurrentUser.UserProfileId;
                using (ALISSContext dc = new ALISSContext())
                {
                    ViewBag.IsLeadOrganisationClaimant = dc.OrganisationClaimUsers.Where(x => x.ClaimedUserId == CurrentUser.UserProfileId && x.IsLeadClaimant).Any();
                    ViewBag.IsLeadServiceClaimant = dc.ServiceClaimUsers.Where(x => x.ClaimedUserId == CurrentUser.UserProfileId && x.IsLeadClaimant).Any();
                    ViewBag.HasUnsubmittedServices = dc.Services.Where(x => x.CreatedUserId == CurrentUser.UserProfileId && x.LastEditedStep != (int)DataInputStepsEnum.DataInputSubmitted).Any();
                }
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/')}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("/umbraco/api/footer/getFooterData").Result;
            if (response.IsSuccessStatusCode)
            {
                dynamic footerData = response.Content.ReadAsAsync<dynamic>().Result;
                FooterModel footer = new FooterModel();
                footer.Copyright = footerData.Copyright;
                foreach (dynamic columnData in footerData.Columns)
                {
                    FooterColumn column = new FooterColumn
                    {
                        Title = columnData.Title.Value
                    };

                    foreach (dynamic link in columnData.Links)
                    {
                        column.Links.Add(new FooterLink { Title = link.Title.Value, Url = link.Url.Value, Type = link.Type.Value });
                    }

                    footer.Columns.Add(column);
                }

                ViewBag.Footer = footer;
            }

            client = new HttpClient();
            client.BaseAddress = new Uri($"{ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/')}");
            try
            {
                Dictionary<string, string> siteSettingsDocs = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.GetStringAsync("/umbraco/api/navigation/GetSiteSettingsDocs").Result);

                ViewBag.AboutClaimedUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/') + siteSettingsDocs["About Claimed"];
                ViewBag.TermsAndConditionsUrl = siteSettingsDocs["Terms And Conditions"];
                ViewBag.DataStandardsUrl = siteSettingsDocs["Data Standards"];
                ViewBag.PrivacyPolicyUrl = siteSettingsDocs["Privacy Policy"];
            }
            catch (Exception)
            {
                ViewBag.AboutClaimedUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/') + "about-claimed";
                ViewBag.TermsAndConditionsUrl = "";
                ViewBag.DataStandardsUrl = "";
                ViewBag.PrivacyPolicyUrl = "";
            }
        }
    }
}