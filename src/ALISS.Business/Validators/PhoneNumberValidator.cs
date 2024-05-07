using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;

namespace ALISS.Business.Validators
{
    public class PhoneNumberValidator : ValidationAttribute
    {
        // The pattern allows the user to enter the number in any format they are comfortable with,
        // but it enforces specific requirements for a valid UK phone number, ensuring that the phone number
        // starts with '+44' or '0', and it should consist of either 6 digits or be between 10 and 14 digits in length.
        // Match UK telephone number in any format

        private readonly string phoneRegex = @"^((?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4}))|(\d{3}[\s-]?\d{3})?$";

        public bool IsValidPhoneNumber(string PhoneNumber)
        {
            if (String.IsNullOrEmpty(PhoneNumber)) return true;

            PhoneNumber = PhoneNumber.Replace(" ", "");

            if (!PhoneNumber.TrimStart('+').All(Char.IsDigit))
            {
                return false;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(PhoneNumber, phoneRegex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            dynamic model = validationContext.ObjectInstance;
            string Phonenumber;
            string type = validationContext.ObjectInstance.GetType().ToString().Split('.').Last();

            if (value == null)
            {
                if(type == "SummaryViewModel")
                {
                    Phonenumber = model.ServicePhoneNumber;
                }
                else
                {
                    Phonenumber = model.PhoneNumber;
                }
            }
            else
            {
                Phonenumber = value.ToString();
            }

            return IsValidPhoneNumber(Phonenumber)
                ? ValidationResult.Success
                : new ValidationResult("Please enter a valid UK phone number.");
        }
    }
}
