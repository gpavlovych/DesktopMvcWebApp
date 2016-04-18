// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The UnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;

namespace MyApplication.Web.Controllers
{
    /// <summary>TODO The UnitOfWork interface.</summary>
    public interface IUnitOfWork : IDisposable
    {

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();
    }
}