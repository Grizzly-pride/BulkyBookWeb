﻿using BulkyBook.DataAccess.Repository.IRepositpry;

namespace BulkyBook.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository Category { get; private set; }
		public ICoverTypeRepository CoverType { get; private set; }

		private ApplicationDbContext _db;

		public UnitOfWork(ApplicationDbContext db)
		{ 
			_db= db;
			Category = new CategoryRepository(_db);
			CoverType= new CoverTypeRepository(_db);
		}

		public void Save() => _db.SaveChanges();
	}
}
