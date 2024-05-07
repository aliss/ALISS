using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class HomeController : ALISSBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}