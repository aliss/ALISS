using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Category;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.ViewModels.Category;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ALISS.Business.Services
{
    public class CategoryService
    {

        private ElasticSearchService _elasticSearchService;

        public CategoryService()
        {
            _elasticSearchService = new ElasticSearchService();
        }

        public CategoryListingViewModel ListCategories(string searchTerm)
        {
         
            CategoryListingViewModel categoryList = new CategoryListingViewModel()
            {
                SearchTerm = searchTerm,
                Categories = new List<CategoryPTO>(),
                TotalResults = 0
            };
            
            using (ALISSContext dc = new ALISSContext())
            {
                List<Category> levelOneCategories = dc.Categories.Where(p => p.ParentCategoryId == null).ToList();
                foreach (Category category in levelOneCategories)
                {
                    bool matchesSearch = true;
                    int levelTwoCount = 0;
                    int levelThreeCount = 0;
                    List<Guid> servicesWithLevelOneCategorySelected = dc.ServiceCategories.Where(c => c.CategoryId == category.CategoryId && c.Service.Published && c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).Select(c => c.ServiceId).ToList();

                    CategoryPTO categoryListItem = new CategoryPTO()
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        ServiceCount = servicesWithLevelOneCategorySelected.Count,
                        NextLevelCategories = new List<CategoryPTO>()
                    };

                    List<Category> levelTwoCategories = dc.Categories.Where(p => p.ParentCategoryId == category.CategoryId).ToList();
                    foreach (Category levelTwoCat in levelTwoCategories)
                    {
                        List<ServiceCategory> levelTwoServices = dc.ServiceCategories.Where(c => c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && c.Service.Published && c.CategoryId == levelTwoCat.CategoryId).ToList();
                        List<ServiceCategory> levelTwoAddToCount = levelTwoServices.Where(c => !servicesWithLevelOneCategorySelected.Contains(c.ServiceId)).ToList(); 
                        List<Guid> servicesWithLevelTwoCategorySelected = levelTwoServices.Select(c => c.ServiceId).ToList();
                        CategoryPTO levelTwoCategoryListItem = new CategoryPTO()
                        {
                            CategoryId = levelTwoCat.CategoryId,
                            Name = levelTwoCat.Name,
                            ServiceCount = levelTwoServices.Count,
                            NextLevelCategories = new List<CategoryPTO>()
                        };

                        categoryListItem.ServiceCount += levelTwoAddToCount.Count;
                        servicesWithLevelOneCategorySelected.AddRange(levelTwoAddToCount.Select(c => c.ServiceId));

                        List<Category> levelThreeCategories = dc.Categories.Where(p => p.ParentCategoryId == levelTwoCat.CategoryId).ToList();
                        foreach (Category levelThreeCat in levelThreeCategories)
                        {
                            List<ServiceCategory> levelThreeServices = dc.ServiceCategories.Where(c => c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && c.Service.Published && c.CategoryId == levelThreeCat.CategoryId).ToList();
                            
                            List<ServiceCategory> levelThreeAddToCount = levelThreeServices.Where(c => !servicesWithLevelOneCategorySelected.Contains(c.ServiceId)).ToList();
                            categoryListItem.ServiceCount += levelThreeAddToCount.Count;
                            servicesWithLevelOneCategorySelected.AddRange(levelThreeAddToCount.Select(c => c.ServiceId));

                            List<ServiceCategory> levelThreeSubCategoryAddToCount = levelThreeServices.Where(c => !servicesWithLevelTwoCategorySelected.Contains(c.ServiceId)).ToList();
                            levelTwoCategoryListItem.ServiceCount += levelThreeSubCategoryAddToCount.Count;
                            servicesWithLevelTwoCategorySelected.AddRange(levelThreeSubCategoryAddToCount.Select(c => c.ServiceId));

                            if (!String.IsNullOrEmpty(searchTerm) && !levelThreeCat.Name.ToLower().Contains(searchTerm.ToLower()))
                            {
                               continue;
                            }
                            CategoryPTO levelThreeCategoryListItem = new CategoryPTO()
                            {
                                CategoryId = levelThreeCat.CategoryId,
                                Name = levelThreeCat.Name,
                                ServiceCount = levelThreeServices.Count,
                            };
                            levelTwoCategoryListItem.NextLevelCategories.Add(levelThreeCategoryListItem);
                        }

                        levelThreeCount = levelTwoCategoryListItem.NextLevelCategories.Count;
                        if (!String.IsNullOrEmpty(searchTerm) && !levelTwoCat.Name.ToLower().Contains(searchTerm.ToLower()) && levelThreeCount == 0)
                        {
                            continue;
                        }

                        levelTwoCategoryListItem.NextLevelCategories = levelTwoCategoryListItem.NextLevelCategories.OrderBy(n => n.NextLevelCategories).ToList();
                        categoryList.TotalResults += levelThreeCount;
                        categoryListItem.NextLevelCategories.Add(levelTwoCategoryListItem);
                    }

                    levelTwoCount = categoryListItem.NextLevelCategories.Count;
                    categoryList.TotalResults += levelTwoCount;
                    categoryListItem.NextLevelCategories = categoryListItem.NextLevelCategories.OrderBy(n => n.Name).ToList();
                    if (!String.IsNullOrEmpty(searchTerm) && !categoryListItem.Name.ToLower().Contains(searchTerm.ToLower()))
                    {
                       
                        matchesSearch = false;
                    }

                    if (matchesSearch || (!matchesSearch && (levelTwoCount > 0 || levelThreeCount > 0)))
                    {
                      
                        categoryList.Categories.Add(categoryListItem);
                    }
                }
                categoryList.TotalResults += categoryList.Categories.Count;
                categoryList.Categories = categoryList.Categories.OrderBy(n => Regex.Replace(n.Name.ToLower(), "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
            }
            
            return categoryList;
        }

        public string AddTopLevelCategory(EditTopLevelCategoryViewModel model)
        {
            if (DoesCategoryExist(model.Name))
            {
                return $"Error: The category {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Category catToAdd = new Category()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    Icon = model.Icon
                };

                dc.Categories.Add(catToAdd);
                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCategoriesToElasticSearch();

            return "Category added successfully";
        }

        public string AddLowerLevelCategory(EditLowerLevelCategoryViewModel model)
        {
            if (DoesCategoryExist(model.Name))
            {
                return $"Error: The category {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Category catToAdd = new Category()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    ParentCategoryId = model.ParentCategoryId
                };

                dc.Categories.Add(catToAdd);
                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCategoriesToElasticSearch();

            return "Category added successfully";
        }

        public EditTopLevelCategoryViewModel GetTopLevelCategoryForEdit(int categoryId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var categoryToEdit = dc.Categories.Find(categoryId);
                return new EditTopLevelCategoryViewModel()
                {
                    CategoryId = categoryToEdit.CategoryId,
                    Name = categoryToEdit.Name,
                    Slug = categoryToEdit.Slug,
                    Icon = categoryToEdit.Icon
                };
            }
        }

        public EditLowerLevelCategoryViewModel GetLowerLevelCategoryForEdit(int categoryId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var categoryToEdit = dc.Categories.Find(categoryId);
                EditLowerLevelCategoryViewModel model = new EditLowerLevelCategoryViewModel()
                {
                    CategoryId = categoryToEdit.CategoryId,
                    Name = categoryToEdit.Name,
                    Slug = categoryToEdit.Slug,
                    ParentCategoryId = categoryToEdit.ParentCategoryId.Value,
                    Categories = GetCategoryDropdown(categoryToEdit.ParentCategoryId.Value, categoryId)
                };

                return model;
            }
        }

        public string EditTopLevelCategory(EditTopLevelCategoryViewModel model)
        {
            bool nameOrSlugChanged = false;

            using (ALISSContext dc = new ALISSContext())
            {
                Category categoryToEdit = dc.Categories.Find(model.CategoryId);
                if (model.Name != categoryToEdit.Name && DoesCategoryExist(model.Name))
                {
                    return $"Error: The category {model.Name} already exists, please choose another name.";
                }

                nameOrSlugChanged = !categoryToEdit.Name.Equals(model.Name) || !categoryToEdit.Slug.Equals(model.Slug);

                categoryToEdit.Name = model.Name;
                categoryToEdit.Slug = model.Slug;
                categoryToEdit.Icon = model.Icon;

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCategoriesToElasticSearch();
            
            if (nameOrSlugChanged)
            {
                _elasticSearchService.ReindexServicesOnCategoryChange(model.CategoryId);
            }

            return "Category edited successfully";
        }

        public string EditLowerLevelCategory(EditLowerLevelCategoryViewModel model)
        {
            bool nameOrSlugChanged = false;

            using (ALISSContext dc = new ALISSContext())
            {
                Category categoryToEdit = dc.Categories.Find(model.CategoryId);
                if (model.Name != categoryToEdit.Name && DoesCategoryExist(model.Name))
                {
                    return $"Error: The category {model.Name} already exists, please choose another name.";
                }

                nameOrSlugChanged = !categoryToEdit.Name.Equals(model.Name) || !categoryToEdit.Slug.Equals(model.Slug);

                categoryToEdit.Name = model.Name;
                categoryToEdit.Slug = model.Slug;
                categoryToEdit.ParentCategoryId = model.ParentCategoryId;

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCategoriesToElasticSearch();

            if (nameOrSlugChanged)
            {
                _elasticSearchService.ReindexServicesOnCategoryChange(model.CategoryId);
            }

            return "Category edited successfully";

        }

        public DeleteCategoryViewModel GetCategoryForDelete(int categoryId)
        {
            var model = new DeleteCategoryViewModel()
            {
                CategoryId = categoryId,
                CanDelete = true,
                RelatedServices = new List<RelatedServicePTO>()
            };

            using (ALISSContext dc = new ALISSContext())
            {
                Category categoryToDelete = dc.Categories.Find(categoryId);
                model.CategoryName = categoryToDelete.Name;

                if (dc.Categories.Count(p => p.ParentCategoryId == categoryId) > 0)
                {
                    model.CanDelete = false;
                    return model;
                }
                else
                {
                    var relatedServices = dc.ServiceCategories.Where(c => c.CategoryId == categoryId).Select(s => s.ServiceId).ToList();
                    var singleCategoryServices = dc.ServiceCategories.GroupBy(s => s.ServiceId).Where(s => s.Count() == 1 && relatedServices.Contains(s.Key)).Select(s => s.FirstOrDefault()).ToList();
                    foreach (var relatedService in singleCategoryServices)
                    {
                        Service service = dc.Services.Find(relatedService.ServiceId);
                        model.RelatedServices.Add(new RelatedServicePTO()
                        {
                            ServiceId = relatedService.ServiceId,
                            ServiceName = service.Name,
                            OrganisationId = service.OrganisationId
                        });
                    }
                }
            }

            return model;
        }

        public void DeleteCategory(int categoryId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var relatedServices = dc.ServiceCategories.Where(c => c.CategoryId == categoryId);
                if (relatedServices.Count() > 0)
                {
                    dc.ServiceCategories.RemoveRange(relatedServices);
                }

                Category categoryToDelete = dc.Categories.Find(categoryId);
                dc.Categories.Remove(categoryToDelete);
                dc.SaveChanges();
            }

            _elasticSearchService.DeleteCategory(categoryId);
        }

        public List<SelectListItem> GetCategoryDropdown(int parentCategoryId, int currentCategoryId = 0)
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<Category> levelOneCategories = dc.Categories.Where(p => p.ParentCategoryId == null).ToList();
                foreach (var levelOneCategory in levelOneCategories)
                {
                    SelectListItem categoryToAdd = new SelectListItem()
                    {
                        Value = levelOneCategory.CategoryId.ToString(),
                        Text = levelOneCategory.Name,
                        Selected = levelOneCategory.CategoryId == parentCategoryId
                    };
                    categories.Add(categoryToAdd);
                    List<Category> levelTwoCategories = dc.Categories.Where(p => p.ParentCategoryId == levelOneCategory.CategoryId).ToList();
                    foreach (var levelTwoCategory in levelTwoCategories)
                    {
                        categoryToAdd = new SelectListItem()
                        {
                            Value = levelTwoCategory.CategoryId.ToString(),
                            Text = $"-- {levelTwoCategory.Name}",
                            Selected = levelTwoCategory.CategoryId == parentCategoryId
                        };
                        if (currentCategoryId == 0 || (currentCategoryId != 0 && levelTwoCategory.CategoryId != currentCategoryId))
                        {
                            categories.Add(categoryToAdd);
                        }
                    }
                }
            }
            return categories;
        }

        public List<ServiceCategoryPTO> GetCategoryListForService(Guid? serviceId, List<string> selectedCategories)
        {
            List<ServiceCategoryPTO> categoryList = new List<ServiceCategoryPTO>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<Category> levelOneCategories = dc.Categories.Where(p => p.ParentCategoryId == null).ToList();
                foreach (Category category in levelOneCategories)
                {
                    ServiceCategoryPTO categoryListItem = new ServiceCategoryPTO()
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        Icon = category.Icon,
                        Selected = serviceId.HasValue ? dc.ServiceCategories.Count(c => c.CategoryId == category.CategoryId && c.ServiceId == serviceId.Value) == 1 : selectedCategories.Count() > 0 ? selectedCategories.Contains(category.CategoryId.ToString()) : false,
                        NextLevelCategories = new List<ServiceCategoryPTO>()
                    };
                    List<Category> levelTwoCategories = dc.Categories.Where(p => p.ParentCategoryId == category.CategoryId).ToList();
                    foreach (Category levelTwoCat in levelTwoCategories)
                    {
                        ServiceCategoryPTO levelTwoCategoryListItem = new ServiceCategoryPTO()
                        {
                            CategoryId = levelTwoCat.CategoryId,
                            Name = levelTwoCat.Name,
                            Selected = serviceId.HasValue ? dc.ServiceCategories.Count(c => c.CategoryId == levelTwoCat.CategoryId && c.ServiceId == serviceId.Value) == 1 : selectedCategories.Count() > 0 ? selectedCategories.Contains(levelTwoCat.CategoryId.ToString()) : false,
                            NextLevelCategories = new List<ServiceCategoryPTO>()
                        };
                        List<Category> levelThreeCategories = dc.Categories.Where(p => p.ParentCategoryId == levelTwoCat.CategoryId).ToList();
                        foreach (Category levelThreeCat in levelThreeCategories)
                        {
                            ServiceCategoryPTO levelThreeCategoryListItem = new ServiceCategoryPTO()
                            {
                                CategoryId = levelThreeCat.CategoryId,
                                Name = levelThreeCat.Name,
                                Selected = serviceId.HasValue ? dc.ServiceCategories.Count(c => c.CategoryId == levelThreeCat.CategoryId && c.ServiceId == serviceId.Value) == 1 : selectedCategories.Count() > 0 ? selectedCategories.Contains(levelThreeCat.CategoryId.ToString()) : false,
                            };
                            levelTwoCategoryListItem.NextLevelCategories.Add(levelThreeCategoryListItem);
                        }
                        levelTwoCategoryListItem.NextLevelCategories = levelTwoCategoryListItem.NextLevelCategories.OrderBy(n => n.Name).ToList();
                        categoryListItem.NextLevelCategories.Add(levelTwoCategoryListItem);
                    }
                    categoryListItem.NextLevelCategories = categoryListItem.NextLevelCategories.OrderBy(n => n.Name).ToList();
                    categoryList.Add(categoryListItem);
                }
                categoryList = categoryList.OrderBy(n => n.Name).ToList();
            }
            return categoryList;
        }

            private bool DoesCategoryExist(string categoryName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Category category = dc.Categories.Where(n => n.Name.ToLower() == categoryName.ToLower()).FirstOrDefault();

                if (category != null)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
