using Core.Entity;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;
        private DbSet<T> table = null;
        public GenericRepository(DbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }

        public void Delete(object id)
        {
            T existing = GetById(id);
            table.Remove(existing);
        }

        public T GetById(object id)
        {
            var result = table.Find(id);
            return result;

        }
        public void Insert(T entity)
        {
            table.Add(entity);
           // _context.SaveChanges();
        }

        public void Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

    }
}
