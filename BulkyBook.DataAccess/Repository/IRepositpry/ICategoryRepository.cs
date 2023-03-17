using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface ICategoryRepository : IRepository<Category>
	{
		void Update(Category obj);
	}
}
