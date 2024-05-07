using ALISS.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class ExportController : ALISSBaseController
    {
        // GET: Export
        public FileResult ExportAllUsers()
        {
            byte[] allUsersCSV = ExportService.ExportAllUsers();

            return File(allUsersCSV, "text/csv", "AllUsers.csv");
        }

        public FileResult ExportClaimedUsers()
        {
            byte[] allClaimsCSV = ExportService.ExportClaimedUsers();

            return File(allClaimsCSV, "text/csv", "ClaimedUsers.csv");
        }

        public FileResult ExportEditorUsers()
        {
            byte[] allUsersCSV = ExportService.ExportEditorUsers();

            return File(allUsersCSV, "text/csv", "EditorUsers.csv");
        }
    }
}