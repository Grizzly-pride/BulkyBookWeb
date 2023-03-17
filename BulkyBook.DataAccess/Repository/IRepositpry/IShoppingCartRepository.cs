using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		int DecrementCount(ShoppingCart shoppingCart, int count);

		int IncrementCount(ShoppingCart shoppingCart, int count);
	}
}
