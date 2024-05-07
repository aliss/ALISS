using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.PresentationTransferObjects.DataInput;

namespace ALISS.Business.Validators
{
    public class LocationValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            dynamic model = validationContext.ObjectInstance;

            if (model == null)
            {
                return ValidationResult.Success;
            }

            string howServiceAccessed = !string.IsNullOrWhiteSpace(model.HowServiceAccessed)
                ? model.HowServiceAccessed.ToLower()
                : "";

            switch (howServiceAccessed)
            {
            case "hybrid":
            case "both":
                if (string.IsNullOrWhiteSpace(model.SelectedServiceAreas))
                {
                    return new ValidationResult("You must select at least one region for this service.");
                }
                break;
            case "remote":
            case "virtual":
                if (string.IsNullOrWhiteSpace(model.SelectedServiceAreas))
                {
                    return new ValidationResult("You must select at least one region for this service.");
                }
                break;
            default:
                if (string.IsNullOrWhiteSpace(model.SelectedServiceAreas) && string.IsNullOrWhiteSpace(model.SelectedLocations))
                {
                    return new ValidationResult("You must select at least one location or one region for this service.");
                }
                break;
            }

            return ValidationResult.Success;
        }

        public string ValidateSummaryModel(string howServiceAccessed, List<string> SelectedServiceAreas, List<WhereLocationPTO> SelectedLocations)
        {
            switch (howServiceAccessed)
            {
                case "hybrid":
                case "both":
                    if (SelectedServiceAreas.Count == 0)
                    {
                        return "You must select at least one region for this service.";
                    }
                    break;
                case "remote":
                case "virtual":
                    if (SelectedServiceAreas.Count == 0)
                    {
                        return "You must select at least one region for this service.";
                    }
                    break;
                default:
                    if (SelectedServiceAreas.Count == 0 && SelectedLocations.Count == 0)
                    {
                        return "You must select at least one location or one region for this service.";
                    }
                    break;
            }

            return "Summary Model Valid";
        }
    }
}
