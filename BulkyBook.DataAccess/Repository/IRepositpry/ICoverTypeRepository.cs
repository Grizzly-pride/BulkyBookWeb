using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface ICoverTypeRepository : IRepository<CoverType>
	{
		void Update(CoverType obj);
	}
}
