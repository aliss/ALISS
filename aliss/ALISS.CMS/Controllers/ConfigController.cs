using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.WebApi;

namespace ALISS.CMS.Controllers
{
    public class ConfigController : UmbracoApiController
    {
        private readonly DtConfiguration _config;

        public ConfigController()
        {
            _config = new DtConfiguration(Umbraco.ContentAtRoot().First().DescendantOfType(DtConfiguration.ModelTypeAlias));
        }

        /// <summary>
        /// Get data governance configuration values
        /// /umbraco/api/config/GetDataGovernance
        /// </summary>
        /// <returns>Data dictionary</returns>
        public JsonResult<Dictionary<string, string>> GetDataGovernance()
        {
            Dictionary<string, string> returnData = new Dictionary<string, string>();

            try
            {
                returnData.Add("Email1NotificationMonths", _config.Email1NotificationMonths.ToString());
                returnData.Add("Email2NotificationMonths", _config.Email2NotificationMonths.ToString());
                returnData.Add("Email3NotificationMonths", _config.Email3NotificationMonths.ToString());
                returnData.Add("BulkReviewMonths", _config.BulkReviewMonths.ToString());
            }
            catch (Exception ex)
            {
                returnData.Add("Error", ex.Message + " - " + ex.StackTrace);
            }

            return Json(returnData);
        }
    }
}