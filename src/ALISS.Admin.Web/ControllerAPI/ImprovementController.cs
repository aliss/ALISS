using ALISS.Business.Services;
using ALISS.Business.ViewModels.Improvement;
using System.Configuration;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/improvements")]
    public class ImprovementController : ApiController
    {
        private ImprovementService _improvementService;
        private Business.Services.EmailService _emailService;
        private string BaseUrl = ConfigurationManager.AppSettings["BaseSiteUrl"].ToString();

        public ImprovementController()
        {
            _improvementService = new ImprovementService();
            _emailService = new Business.Services.EmailService();
        }

        [HttpPost]
        [Route("PostImprovement")]
        public string PostImprovement(EditImprovementViewModel model)
        {
            _improvementService.AddSuggestedImprovement(model);
            string url = BaseUrl + "Improvement";
            _emailService.SendGenericSystemEmail(ConfigurationManager.AppSettings["ALISSAdminContact"], "New Suggested Improvement", "A new suggested improvement has been added into the system. Please log in to the ALISS Admin site to review this.", url);
            return "Suggested Improvement submitted successfully. A member of the ALISS team will review this shortly.";
        }
    }
}
