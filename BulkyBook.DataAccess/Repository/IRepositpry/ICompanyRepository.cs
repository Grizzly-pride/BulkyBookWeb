using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepositpry
{
	public interface ICompanyRepository : IRepository<Company>
	{
		void Update(Company obj);
	}
}
