using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.Validators
{
    public class ConditionallyRequiredBoolTrueValidator : ValidationAttribute
    {
        private readonly string _conditionalPropertyName;
        private readonly object _requiredValue;

        public ConditionallyRequiredBoolTrueValidator(string conditionalPropertyName) : base("The field {0} is required")
        {
            _conditionalPropertyName = conditionalPropertyName;
        }

        public override bool IsValid(object value)
        {
            return IsValid(value, new ValidationContext(value)) == ValidationResult.Success;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(_conditionalPropertyName) )
            {
                return ValidationResult.Success;
            }
            else
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(_conditionalPropertyName);                
                var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

                if ((bool)propertyValue) {
                    if ((bool)value == true) return ValidationResult.Success;
                }
                else
                {
                    return ValidationResult.Success;
                }
                
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name });
        } 
    }
}
