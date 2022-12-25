using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll(); 
            return View(objCoverTypeList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if (ModelState.IsValid)
            {
				_unitOfWork.CoverType.Add(obj);
				_unitOfWork.Save();
                TempData["success"] = "CoverType created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

		public IActionResult Edit(int? id)
		{
            if(id is null || id is 0) { return NotFound(); }

            var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if(coverTypeFromDbFirst is null) { return NotFound(); }

			return View(coverTypeFromDbFirst);
		}
         
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(CoverType obj)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.CoverType.Update(obj);
				_unitOfWork.Save();
                TempData["success"] = "CoverType updated successfully";
                return RedirectToAction("Index");
			}
			return View(obj);
		}

        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0) { return NotFound(); }

            var categoryFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst is null) { return NotFound(); }

            return View(categoryFromDbFirst);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var coverTypeFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

			if (coverTypeFromDb is null) { return NotFound(); }

            _unitOfWork.CoverType.Remove(coverTypeFromDb);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfully";
            return RedirectToAction("Index");       
        }
    }
}
