using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace MyApplication.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class GenericRepository<T> : IGenericRepository<T> where T: class
    {
        protected DbContext Entities;

        protected readonly IDbSet<T> Dbset;

        public GenericRepository(DbContext context)
        {
            this.Entities = context;
            this.Dbset = context.Set<T>();
        }

        public virtual T FindById<TId>(TId id)
        {
            return this.Dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {

            return this.Dbset.AsEnumerable<T>();
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return this.Dbset;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {

            IEnumerable<T> query = this.Dbset.Where(predicate).AsEnumerable();
            return query;
        }

        public virtual T Add(T entity)
        {
            return this.Dbset.Add(entity);
        }

        public virtual T Delete(T entity)
        {
            return this.Dbset.Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            this.Entities.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            this.Entities.SaveChanges();
        }
    }
}