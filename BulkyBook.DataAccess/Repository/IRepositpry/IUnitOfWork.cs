﻿namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }
		ICoverTypeRepository CoverType { get; }
		IProductRepository Product { get; }
		ICompanyRepository Company { get; }
		IApplicationUserRepository ApplicationUser { get; }
		IShoppingCartRepository ShoppingCart { get; }
		void Save();
	}
}
