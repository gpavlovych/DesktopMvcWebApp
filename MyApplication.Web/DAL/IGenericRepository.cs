using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyApplication.Web.Controllers
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();

        IQueryable<T> GetQueryable();

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        T FindById<TId>(TId id);

        T Add(T entity);

        T Delete(T entity);

        void Edit(T entity);

        void Save();
    }
}