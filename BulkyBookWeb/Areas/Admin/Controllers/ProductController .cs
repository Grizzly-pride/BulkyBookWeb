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

        public ProductController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _unitOfWork.Product.GetAll(); 
            return View(objProductList);
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
		public IActionResult Upsert(Product obj)
		{

			if (ModelState.IsValid)
			{
				_unitOfWork.Product.Update(obj);
				_unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
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
    }
}
