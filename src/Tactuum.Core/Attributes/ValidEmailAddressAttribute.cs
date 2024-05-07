using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tactuum.Core.Attributes
{
    public class ValidEmailAddressAttribute : ValidationAttribute
    {
        public ValidEmailAddressAttribute() :
            base()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {

                string testString = value.ToString();


                if (!Regex.IsMatch(testString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult("Please enter a valid email address.");
                }
                return null;

            }
            else
            {
                return null;
            }

        }
    }
}
