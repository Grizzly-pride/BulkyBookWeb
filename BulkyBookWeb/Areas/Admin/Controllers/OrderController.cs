using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("admin")]
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

    public IActionResult Details(int orderId)
    {
        OrderViewModel orderViewModel = new()
        {
            OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(filter: order => order.Id == orderId, includeProperties: new string[] { "ApplicationUser" }),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(filter: i => i.OrderId == orderId, includeProperties: new string[] { "Product" })
        };

        return View(orderViewModel);
    }


    #region API CALS
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        var orderList = _unitOfWork.OrderHeader
            .GetAll(includeProperties: new string[] { "ApplicationUser"});

        switch (status)
        {
            case "pending":
                orderList = orderList.Where(i => i.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                orderList = orderList.Where(i => i.OrderStatus == SD.StatusInProcess);
                break;
            case "completed":
                orderList = orderList.Where(i => i.OrderStatus == SD.StatusShipped);
                break;
            case "approved":
                orderList = orderList.Where(i => i.OrderStatus == SD.StatusApproved);
                break;
            default:
                break;
        }

        return Json(new { data = orderList });
    }
    #endregion
}
