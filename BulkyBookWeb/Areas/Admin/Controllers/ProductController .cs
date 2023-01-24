using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


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

		[HttpGet]
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

			if (id == null || id == 0) 
            {
                return View(productView);
            }
            else 
            {
                productView.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
			    return View(productView);
            }
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upsert(ProductViewModel obj, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file is not null) 
                {
                    string fileName = Guid.NewGuid().ToString();
                    string uploads = Path.Combine(wwwRootPath, @"images\products");
                    string extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageUrl != null)
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStrim = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        file.CopyTo(fileStrim);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
				if (obj.Product.Id == 0)
				{
					_unitOfWork.Product.Add(obj.Product);
				}
				else
				{
					_unitOfWork.Product.Update(obj.Product);
				}

				_unitOfWork.Save();
                TempData["success"] = "Product added successfully";
                return RedirectToAction(nameof(Index));
			}
			return RedirectToAction();
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
       

        #region API CALS
        [HttpGet]
        public IActionResult GetAll() 
        {
            var productList = _unitOfWork.Product.GetAll("Category", "CoverType");
            return Json(new { data = productList });
        }
		#endregion

		[HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
