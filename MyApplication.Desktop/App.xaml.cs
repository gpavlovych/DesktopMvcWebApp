// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Windows;
using MyApplication.Desktop.Properties;
using MyApplication.Desktop.Services;
using MyApplication.Desktop.Views;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;

namespace MyApplication.Desktop
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App
    {
        /// <summary>The logics to be performed during application startup</summary>
        /// <param name="e">An instance of <see cref="StartupEventArgs"/> to be used.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IUnityContainer container = new UnityContainer();
            container.RegisterType<HttpClient>(new InjectionFactory(x => new HttpClient()));
            container.RegisterType<ILoginWindow>(new InjectionFactory(x => new LoginWindow()));
            container.RegisterInstance(Settings.Default);
            container.RegisterType<IHttpClient, HttpClientWrapper>(new PerResolveLifetimeManager());
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<ISearchService, SearchService>();
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewModelType => container.Resolve(viewModelType));
        }
    }
}