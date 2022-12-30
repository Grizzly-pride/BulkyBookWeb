using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
			_unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
				_unitOfWork.Product.Add(obj);
				_unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

		public IActionResult Upsert(int? id)
		{
            ProductViewModel productView = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };


			if (id is null || id is 0) 
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productView);
            }
            else 
            {
                //update product
            }

			return View(productView);
		}
         
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upsert(ProductViewModel obj, IFormFile file)
		{

			if (ModelState.IsValid)
			{
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file is not null) 
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStrim = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        file.CopyTo(fileStrim);
                    }
                    obj.Product.ImageUrl = @"\image\products" + fileName + extension;
                }
				_unitOfWork.Product.Add(obj.Product);
				_unitOfWork.Save();
                TempData["success"] = "Product added successfully";
                return RedirectToAction("Index");
			}
			return View(obj);
		}

        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0) { return NotFound(); }

            var ProductFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (ProductFromDbFirst is null) { return NotFound(); }

            return View(ProductFromDbFirst);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var ProductFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

			if (ProductFromDb is null) { return NotFound(); }

            _unitOfWork.Product.Remove(ProductFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");       
        }

        #region API CALS
        [HttpGet]
        public IActionResult GetAll() 
        {
            var productList = _unitOfWork.Product.GetAll();
            return Json(new { data = productList });
        }
		#endregion
	}
}
