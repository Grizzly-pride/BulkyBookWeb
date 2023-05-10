using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
