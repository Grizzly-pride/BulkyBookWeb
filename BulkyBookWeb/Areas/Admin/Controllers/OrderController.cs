using BulkyBook.DataAccess.Repository.IRepositpry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace BulkyBookWeb.Areas.Admin.Controllers;

public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork; 
    }

    public IActionResult Index()
    {
        return View();
    }

    #region API CALS
    [HttpGet]
    public IActionResult GetAll()
    {
        var orderList = _unitOfWork.OrderHeader
            .GetAll(includeProperties: new string[] { "ApplicationUser"});

        return Json(new { data = orderList });
    }
    #endregion
}
