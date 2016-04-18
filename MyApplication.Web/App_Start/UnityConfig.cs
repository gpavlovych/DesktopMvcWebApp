// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityConfig.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The unity config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MyApplication.Web.Controllers;
using MyApplication.Web.DAL;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;

namespace MyApplication.Web
{
    /// <summary>TODO The unity config.</summary>
    public static class UnityConfig
    {
        /// <summary>TODO The register components.</summary>
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Types registration
            container.RegisterInstance(Properties.Settings.Default);
            container.RegisterType<IApplicationUserManager, ApplicationUserManager>();
            container.RegisterType<IApplicationSignInManager, ApplicationSignInManager>();
            container.RegisterType<ISettingsProvider, SettingsProvider>();
            container.RegisterType<DbContext, ApplicationDbContext>(new PerRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType(typeof (IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
          
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(
                new InjectionConstructor(typeof(ApplicationDbContext)));
            DependencyResolver.SetResolver(new Microsoft.Practices.Unity.Mvc.UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            // Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }
    }
}