using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Migrations;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("admin")]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderViewModel OrderViewModel { get; set; }

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
        OrderViewModel = new()
        {
            OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(filter: order => order.Id == orderId, includeProperties: new string[] { "ApplicationUser" }),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(filter: i => i.OrderId == orderId, includeProperties: new string[] { "Product" })
        };

        return View(OrderViewModel);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + ","+ SD.Role_Employee)]
    public IActionResult UpdateOrderDetail(int orderId)
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);
        orderHeaderFromDb.Name = OrderViewModel.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
        orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;
        if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
        {
            orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.TrackingNumber;
        }
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        TempData["Success"] = "Order Details Updated Successfully.";


        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
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
