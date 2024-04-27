using System.ComponentModel.DataAnnotations;

namespace Tactuum.Core.Attributes
{
    public class NumericOnlyAttribute : ValidationAttribute
    {

        public NumericOnlyAttribute()
            : base()
        {
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {

                string testString = value.ToString();


                foreach (char c in testString)
                {
                    if (!char.IsDigit(c))
                    {
                        return new ValidationResult("");
                    }
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
