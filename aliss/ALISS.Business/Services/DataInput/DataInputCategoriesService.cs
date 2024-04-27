using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Category;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public CategoriesViewModel GetCategoriesForEdit(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);

                return new CategoriesViewModel()
                {
                    OrganisationId = service.OrganisationId,
                    ServiceId = serviceId,
                    SelectedCategories = string.Join(",", GetSelectedCategories(serviceId)),
                    ServiceCategories = _categoryService.GetCategoryListForService(serviceId, GetSelectedCategories(serviceId))
                };
            }
        }

        public string EditCategories(CategoriesViewModel model, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.CategoriesTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.CategoriesTestStep;
                }
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                var selectedCategories = dc.ServiceCategories.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceCategories.RemoveRange(selectedCategories);

                if (!String.IsNullOrEmpty(model.SelectedCategories))
                {
                    List<string> serviceCategories = model.SelectedCategories.Split(',').ToList();
                    foreach (var serviceCategory in serviceCategories)
                    {
                        var categoryToAdd = new ServiceCategory()
                        {
                            ServiceId = serviceToEdit.ServiceId,
                            CategoryId = Convert.ToInt32(serviceCategory)
                        };
                        dc.ServiceCategories.Add(categoryToAdd);
                    }
                }

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = model.ServiceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();

                if (serviceToEdit.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                }
            }

            return "Service Edited Successfully";
        }

        public CategoriesViewModel RepopulateCategoriesModelForError(CategoriesViewModel model)
        {
            List<string> selectedCategories = String.IsNullOrEmpty(model.SelectedCategories) ? new List<string>() : model.SelectedCategories.Split(',').ToList();
            model.ServiceCategories = _categoryService.GetCategoryListForService(null, selectedCategories);

            return model;
        }

        public List<string> GetSelectedCategories(Guid serviceId)
        {
            List<string> selectedCategories = new List<string>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<Category> levelOneCategories = dc.Categories.Where(p => p.ParentCategoryId == null).ToList();
                foreach (Category category in levelOneCategories)
                {
                    if (dc.ServiceCategories.Count(c => c.CategoryId == category.CategoryId && c.ServiceId == serviceId) == 1)
                    {
                        selectedCategories.Add(category.Name);
                    }
                        
                    List<Category> levelTwoCategories = dc.Categories.Where(p => p.ParentCategoryId == category.CategoryId).ToList();
                    foreach (Category levelTwoCat in levelTwoCategories)
                    {
                        if (dc.ServiceCategories.Count(c => c.CategoryId == levelTwoCat.CategoryId && c.ServiceId == serviceId) == 1)
                        {
                            selectedCategories.Add(levelTwoCat.Name);
                        }

                        List<Category> levelThreeCategories = dc.Categories.Where(p => p.ParentCategoryId == levelTwoCat.CategoryId).ToList();
                        foreach (Category levelThreeCat in levelThreeCategories)
                        {
                            if (dc.ServiceCategories.Count(c => c.CategoryId == levelThreeCat.CategoryId && c.ServiceId == serviceId) == 1)
                            {
                                selectedCategories.Add(levelThreeCat.Name);
                            }
                        }
                    }
                }
            }

            return selectedCategories.OrderBy(n => Regex.Replace(n, "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
        }
    }
}
