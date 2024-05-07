using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Tactuum.Core.Attributes
{
    public class ConditionallyRequiredAttribute : ValidationAttribute
    {
        private readonly string _conditionalPropertyName;
        private readonly object _requiredValue;

        public ConditionallyRequiredAttribute(string conditionalPropertyName, object requiredValue) : base("The field {0} is required")
        {
            _conditionalPropertyName = conditionalPropertyName;
            _requiredValue = requiredValue;
        }

        public override bool IsValid(object value)
        {
            return IsValid(value, new ValidationContext(value)) == ValidationResult.Success;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(_conditionalPropertyName))
            {
                return null;
            }

            var propertyInfo = validationContext.ObjectType.GetProperty(_conditionalPropertyName);
            if (propertyInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", _conditionalPropertyName));
            }

            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (!_requiredValue.Equals(propertyValue) ||
                (value != null && value.ToString().Length > 0))
            {
                return null;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name });
        }
    }
}
