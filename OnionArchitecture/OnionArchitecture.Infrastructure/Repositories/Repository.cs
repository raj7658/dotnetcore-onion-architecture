using Microsoft.EntityFrameworkCore;
using OnionArchitecture.Domain.Interfaces;
using OnionArchitecture.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> GetByName(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        public async Task Add(T entity,CancellationToken cancellationToken)
        {
           await _context.Set<T>().AddAsync(entity, cancellationToken);
           await _context.SaveChangesAsync();
        }

        
    }
}
