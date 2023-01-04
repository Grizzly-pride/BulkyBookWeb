using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface IRepository<T> where T : class
	{
		T GetFirstOrDefault(Expression<Func<T, bool>> filter, params string[] includeProperties);
		IEnumerable<T> GetAll(params string[] includeProperties);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entites);
	}	
}
