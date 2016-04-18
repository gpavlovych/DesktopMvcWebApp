// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericRepository.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The generic repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The generic repository.</summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public class GenericRepository<T> : IGenericRepository<T> where T: class
    {
        /// <summary>TODO The entities.</summary>
        protected DbContext Entities;

        /// <summary>TODO The dbset.</summary>
        protected readonly IDbSet<T> Dbset;

        /// <summary>Initializes a new instance of the <see cref="GenericRepository{T}"/> class.</summary>
        /// <param name="context">TODO The context.</param>
        public GenericRepository(DbContext context)
        {
            this.Entities = context;
            this.Dbset = context.Set<T>();
        }

        /// <summary>TODO The find by id.</summary>
        /// <param name="id">TODO The id.</param>
        /// <typeparam name="TId"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        public virtual T FindById<TId>(TId id)
        {
            return this.Dbset.Find(id);
        }

        /// <summary>TODO The get all.</summary>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        public virtual IEnumerable<T> GetAll()
        {

            return this.Dbset.AsEnumerable<T>();
        }

        /// <summary>TODO The get queryable.</summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public virtual IQueryable<T> GetQueryable()
        {
            return this.Dbset;
        }

        /// <summary>TODO The find by.</summary>
        /// <param name="predicate">TODO The predicate.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {

            IEnumerable<T> query = this.Dbset.Where(predicate).AsEnumerable();
            return query;
        }

        /// <summary>TODO The add.</summary>
        /// <param name="entity">TODO The entity.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public virtual T Add(T entity)
        {
            return this.Dbset.Add(entity);
        }

        /// <summary>TODO The delete.</summary>
        /// <param name="entity">TODO The entity.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public virtual T Delete(T entity)
        {
            return this.Dbset.Remove(entity);
        }

        /// <summary>TODO The edit.</summary>
        /// <param name="entity">TODO The entity.</param>
        public virtual void Edit(T entity)
        {
            this.Entities.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>TODO The save.</summary>
        public virtual void Save()
        {
            this.Entities.SaveChanges();
        }
    }
}