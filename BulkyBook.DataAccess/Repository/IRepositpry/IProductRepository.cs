using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface IProductRepository : IRepository<Product>
	{
		void Update(Product obj);
	}
}
