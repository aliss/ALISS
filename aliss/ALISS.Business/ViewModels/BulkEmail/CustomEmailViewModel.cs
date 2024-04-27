using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ALISS.Business.ViewModels.BulkEmail
{
    public class CustomEmailViewModel
    {
        public Guid? Id {  get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public string Logo = ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "Ui/dist/img/email-logo.png";
        [Display(Name = "CC ALISS Admin in Email")]
        public bool CCAlissAdmin { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }
}
