using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.Validators
{
    public class MinMaxValidator : ValidationAttribute
    {
        public bool IsValidMinMax(string selected)
        {
            if (String.IsNullOrEmpty(selected))
            {
                return true;
            }

            if (selected.Split('|').Length < 3) 
            {
                return true;
            }

            string minValue = selected.Split('|')[1];
            string maxValue = selected.Split('|')[2];

            if (string.IsNullOrEmpty(minValue) || string.IsNullOrEmpty(maxValue))
            {
                return false;
            }

            if (!minValue.All(Char.IsDigit) || !maxValue.All(Char.IsDigit))
            {
                return false;
            }

            return int.Parse(minValue) >= 0 && int.Parse(maxValue) >= 0 && int.Parse(minValue) <= int.Parse(maxValue);
        }

        public bool IsValidSelectedCommGroups(string selected)
        {
            foreach(string commgroup in selected.Split(','))
            {
                if (!IsValidMinMax(commgroup))
                {
                    return false;
                }
            }

            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            dynamic model = validationContext.ObjectInstance;
            string selected = model.SelectedCommunityGroups;

            return IsValidSelectedCommGroups(selected)
                ? ValidationResult.Success
                : new ValidationResult("One of the selected community groups range is invalid");
        }
    }
}
