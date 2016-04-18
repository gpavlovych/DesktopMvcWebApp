// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The unit of work.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The unit of work.</summary>
    [ExcludeFromCodeCoverage]
    public sealed class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The DbContext
        /// </summary>
        private DbContext _dbContext;

        /// <summary>Initializes a new instance of the UnitOfWork class.</summary>
        /// <param name="context">The object context</param>
        public UnitOfWork(DbContext context)
        {

            this._dbContext = context;
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public int Commit()
        {
            // Save changes with the default options
            return this._dbContext.SaveChanges();
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes all external resources.</summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._dbContext != null)
                {
                    this._dbContext.Dispose();
                    this._dbContext = null;
                }
            }
        }
    }
}