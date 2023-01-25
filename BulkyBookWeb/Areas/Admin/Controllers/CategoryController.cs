using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public CategoryController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll(); 
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name.Equals(obj.DisplayOrder.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("name", "The Display cannot exactly matchthe Name.");
            }
            if (ModelState.IsValid)
            {
				_unitOfWork.Category.Add(obj);
				_unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

		public IActionResult Edit(int? id)
		{
            if(id is null || id is 0) { return NotFound(); }

            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if(categoryFromDbFirst is null) { return NotFound(); }

			return View(categoryFromDbFirst);
		}
         
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category obj)
		{
			if (obj.Name.Equals(obj.DisplayOrder.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				ModelState.AddModelError("name", "The Display cannot exactly matchthe Name.");
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Update(obj);
				_unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
			}
			return View(obj);
		}

        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0) { return NotFound(); }

            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst is null) { return NotFound(); }

            return View(categoryFromDbFirst);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

			if (categoryFromDb is null) { return NotFound(); }

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");       
        }
    }
}
