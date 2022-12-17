using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories.ToList();  
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

		public IActionResult Edit(int? id)
		{
            if(id is null || id is 0) { return NotFound(); }

            var categoryFromDb = _db.Categories.Find(id);

            if(categoryFromDb is null) { return NotFound(); }

			return View(categoryFromDb);
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
				_db.Categories.Update(obj);
				_db.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
			}
			return View(obj);
		}

        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0) { return NotFound(); }

            var categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb is null) { return NotFound(); }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb is null) { return NotFound(); }

            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");       
        }
    }
}
