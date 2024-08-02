using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Domain.Interfaces.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, bool istracking = true);
        Task<T?> GetById(int id);
        Task Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
