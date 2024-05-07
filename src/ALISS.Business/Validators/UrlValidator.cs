using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ALISS.Business.Validators
{
    public class UrlValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string url = value as string;

            if (string.IsNullOrEmpty(url))
            {
                return ValidationResult.Success;
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                string[] split = url.Split('/');

                if (split.Length >= 2)
                {
                    if (!char.IsLetterOrDigit(split[2].First()))
                        return new ValidationResult("Please remove any symbols between the HTTP and the web address in " + validationContext.DisplayName + ".");
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
                    if (uriResult == null || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps && uriResult.Scheme != Uri.UriSchemeFtp)
                        return new ValidationResult("This " + validationContext.DisplayName + " field is not a valid fully-qualified http, https, or ftp URL.");

                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("This " + validationContext.DisplayName + " field is not a valid fully-qualified http, https, or ftp URL.");
            }
        }
    }
}
