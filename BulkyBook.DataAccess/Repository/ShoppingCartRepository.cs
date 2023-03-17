﻿using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models.ViewModels;

namespace BulkyBook.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
        private ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db) 
		{
			_db = db;
		}
	}
}
