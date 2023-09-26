using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Upsert(int? id)
		{
            Company company = new();


			if (id == null || id == 0) 
            {
                return View(company);
            }
            else 
            {
				company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
			    return View(company);
            }
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upsert(Company obj)
		{
			if (ModelState.IsValid)
			{
				if (obj.Id == 0)
				{
					_unitOfWork.Company.Add(obj);
					TempData["success"] = "Product created successfully";
				}
				else
				{
					_unitOfWork.Company.Update(obj);
					TempData["success"] = "Product updated successfully";
				}

				_unitOfWork.Save();
                return RedirectToAction(nameof(Index));
			}
			return RedirectToAction();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Company obj)
        {
			if (ModelState.IsValid)
            {
				_unitOfWork.Company.Add(obj);
				_unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
       

        #region API CALS
        [HttpGet]
        public IActionResult GetAll() 
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }
		#endregion

		[HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
