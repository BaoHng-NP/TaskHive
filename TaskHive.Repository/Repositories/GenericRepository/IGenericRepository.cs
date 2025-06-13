using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Repositories.GenericRepository
{

        public interface IGenericRepository<T> where T : class
        {
            Task<IEnumerable<T>> GetAllAsync();
            Task<T> GetByIdAsync(int id);
            Task AddAsync(T entity);
            Task UpdateAsync(T entity);
            Task DeleteAsync(T entity);
            Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
            Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
