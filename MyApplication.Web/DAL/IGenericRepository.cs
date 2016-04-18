// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGenericRepository.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The GenericRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The GenericRepository interface.</summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>TODO The get all.</summary>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        IEnumerable<T> GetAll();

        /// <summary>TODO The get queryable.</summary>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        IQueryable<T> GetQueryable();

        /// <summary>TODO The find by.</summary>
        /// <param name="predicate">TODO The predicate.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>TODO The find by id.</summary>
        /// <param name="id">TODO The id.</param>
        /// <typeparam name="TId"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        T FindById<TId>(TId id);

        /// <summary>TODO The add.</summary>
        /// <param name="entity">TODO The entity.</param>
        /// <returns>The <see cref="T"/>.</returns>
        T Add(T entity);

        /// <summary>TODO The delete.</summary>
        /// <param name="entity">TODO The entity.</param>
        /// <returns>The <see cref="T"/>.</returns>
        T Delete(T entity);

        /// <summary>TODO The edit.</summary>
        /// <param name="entity">TODO The entity.</param>
        void Edit(T entity);

        /// <summary>TODO The save.</summary>
        void Save();
    }
}