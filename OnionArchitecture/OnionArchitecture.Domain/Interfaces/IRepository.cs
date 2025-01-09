using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Domain.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetById(int id, CancellationToken cancellationToken);
        Task Add(T entity,CancellationToken cancellationToken);
        Task<T> GetByName(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    }
}
