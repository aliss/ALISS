using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tactuum.Core.Attributes
{
    public class ValidPostcodeAttribute : ValidationAttribute
    {
        public ValidPostcodeAttribute() : base()
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string testString = value.ToString();

                if (!Regex.IsMatch(testString, @"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))\s?[0-9][A-Za-z]{2})$", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult("Please enter a valid UK postcode.");
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
