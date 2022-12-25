﻿using BulkyBook.DataAccess.Repository.IRepositpry;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
	public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
	{
		public CoverTypeRepository(ApplicationDbContext db) : base(db) { }

		public void Update(CoverType obj) => _db.Update(obj);
	}
}
