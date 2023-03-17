﻿using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
	[Authorize]
	[Area("Customer")]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}
        public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartViewModel()
			{
				ListCart = _unitOfWork.ShoppingCart
				.GetAll(
					filter: u => u.ApplicationUserId == claim.Value,
					includeProperties: "Product")
			};

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.CartTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
		}


        private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }
    }
}
