using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Migrations;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("admin")]
[Authorize]
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


    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();

        TempData["Success"] = "Order Details Updated Successfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
    }


    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);
        orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
        orderHeader.OrderStatus = SD.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;
        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        }

        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        TempData["Success"] = "Order Shipped Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult CancelOrder()
    {

        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);

        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
        }
        _unitOfWork.Save();
        TempData["Success"] = "Order Cancelled Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });

    }


    #region API CALS
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> objOrderHeaders;

        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            objOrderHeaders = _unitOfWork.OrderHeader
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        switch (status)
        {
            case "pending":
                objOrderHeaders = objOrderHeaders.Where(i => i.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                objOrderHeaders = objOrderHeaders.Where(i => i.OrderStatus == SD.StatusInProcess);
                break;
            case "completed":
                objOrderHeaders = objOrderHeaders.Where(i => i.OrderStatus == SD.StatusShipped);
                break;
            case "approved":
                objOrderHeaders = objOrderHeaders.Where(i => i.OrderStatus == SD.StatusApproved);
                break;
            default:
                break;
        }

        return Json(new { data = objOrderHeaders });
    }
    #endregion
}
