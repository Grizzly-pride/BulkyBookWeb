using BulkyBook.DataAccess.Repository.IRepositpry;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BulkyBook.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;

		public Repository(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();
		}

		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public IEnumerable<T> GetAll(params string[] includeProperties)
		{
			IQueryable<T> query = dbSet;
			if (includeProperties is not null)
			{
				foreach (var includeProp in includeProperties)
				{
					query = query.Include(includeProp);
				}
			}
			return query.ToList();
		}

		public T GetFirstOrDefault(Expression<Func<T, bool>> filter, params string[] includeProperties)
		{
			IQueryable<T> query = dbSet;
			query = query.Where(filter);
            foreach (var includeProp in includeProperties)
            {
                query = query.Include(includeProp);
            }
            return query.FirstOrDefault();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entites)
		{
			dbSet.RemoveRange(entites);
		}
	}
}
