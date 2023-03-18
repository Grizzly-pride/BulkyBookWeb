using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface IRepository<T> where T : class
	{
		T GetFirstOrDefault(Expression<Func<T, bool>> filter, params string[] includeProperties);
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, params string[] includeProperties);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entites);
	}	
}
