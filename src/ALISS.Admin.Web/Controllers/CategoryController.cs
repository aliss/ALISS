using ALISS.Business.Services;
using ALISS.Business.ViewModels.Category;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class CategoryController : ALISSBaseController
    {
        private readonly CategoryService _categoryService;

        public CategoryController()
        {
            _categoryService = new CategoryService();
        }

        // GET: Category
        public ActionResult Index(string searchTerm = "")
        {
            ViewBag.ResponseMessage = Session["ResponseMessage"] != null ? Session["ResponseMessage"].ToString() : "";
            CategoryListingViewModel model = _categoryService.ListCategories(searchTerm);
            return View(model);
        }

        public ActionResult AddCategory(int parentCategoryId = 0)
        {
            if (parentCategoryId == 0)
            {
                return View("AddTopLevelCategory", new EditTopLevelCategoryViewModel());
            }
            else
            {
                EditLowerLevelCategoryViewModel viewModel = new EditLowerLevelCategoryViewModel()
                {
                    ParentCategoryId = parentCategoryId,
                    Categories = _categoryService.GetCategoryDropdown(parentCategoryId)
                };
                return View("AddLowerLevelCategory", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTopLevelCategory(EditTopLevelCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                return View(model);
            }
            string responseMessage = "";
            if (model.CategoryId == 0)
            {
                responseMessage = _categoryService.AddTopLevelCategory(model);
            }
            else
            {
                responseMessage = _categoryService.EditTopLevelCategory(model);
            }
            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                return View(model);
            }
            //Session["ResponseMessage"] = responseMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLowerLevelCategory(EditLowerLevelCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = true;
                model.Categories = _categoryService.GetCategoryDropdown(model.ParentCategoryId, model.CategoryId);
                return View(model);
            }

            string responseMessage = "";

            if (model.CategoryId == 0)
            {
                responseMessage = _categoryService.AddLowerLevelCategory(model);
            }
            else
            {
                responseMessage = _categoryService.EditLowerLevelCategory(model);
            }

            if (responseMessage.Contains("Error"))
            {
                ViewBag.Error = true;
                ModelState.AddModelError("DuplicateName", responseMessage);
                model.Categories = _categoryService.GetCategoryDropdown(model.ParentCategoryId, model.CategoryId);
                return View(model);
            }
            //Session["ResponseMessage"] = responseMessage;
            return RedirectToAction("Index");
        }

        public ActionResult EditCategory(int id, bool topLevelCategory)
        {
            if (topLevelCategory)
            {
                EditTopLevelCategoryViewModel editViewModel = _categoryService.GetTopLevelCategoryForEdit(id);
                return View("AddTopLevelCategory", editViewModel);
            }
            else
            {
                EditLowerLevelCategoryViewModel editViewModel = _categoryService.GetLowerLevelCategoryForEdit(id);
                return View("AddLowerLevelCategory", editViewModel);
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            DeleteCategoryViewModel model = _categoryService.GetCategoryForDelete(id);
            if (!model.CanDelete)
            {
                ViewBag.ResponseMessage = "You cannot delete this category as it has a number of categories below it.";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirm(int id)
        {
            _categoryService.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}