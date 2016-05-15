using System;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using MyApplication.Desktop.Services;
using MyApplication.Desktop.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace MyApplication.Desktop.ViewModels
{
    /// <summary>The main window view model.</summary>
    public class MainWindowViewModel : BindableBase
    {
        private string _someFoundContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="container">The container.</param>
        public MainWindowViewModel(ISearchService searchService, ISettingsService settingsService, IUnityContainer container)
        {
            this.LoadCommand = new DelegateCommand(
                () =>
                    {
                        if (this.LoginCommand.CanExecute(null))
                        {
                            this.LoginCommand.Execute(null);
                        }
                        else
                        {
                            if (this.SearchCommand.CanExecute(null))
                            {
                                this.SearchCommand.Execute(null);
                            }
                        }
                    });
            this.LoginCommand = new DelegateCommand(
                () =>
                    {
                        var view = container.Resolve<ILoginWindow>();
                        var result = view.ShowDialog();
                        ( (DelegateCommand) this.LoginCommand ).RaiseCanExecuteChanged();
                        ( (DelegateCommand) this.SearchCommand ).RaiseCanExecuteChanged();
                        ( (DelegateCommand) this.LogOffCommand ).RaiseCanExecuteChanged();
                        if (( result ?? false ) && this.SearchCommand.CanExecute(null))
                        {
                            this.SearchCommand.Execute(null);
                        }
                    }, 
                () => !settingsService.IsLoggedIn);
            this.SearchCommand = new DelegateCommand(
                async () =>
                    {
                        try
                        {
                            this.SomeFoundContent = await searchService.CallSomeApi();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            if (this.LogOffCommand.CanExecute(null))
                            {
                                this.LogOffCommand.Execute(null);
                            }
                        }
                    }, 
                () => settingsService.IsLoggedIn);
            this.LogOffCommand = new DelegateCommand(
                () =>
                    {
                        this.SomeFoundContent = string.Empty;
                        settingsService.IsLoggedIn = false;
                        settingsService.Save();
                        ( (DelegateCommand) this.LoginCommand ).RaiseCanExecuteChanged();
                        ( (DelegateCommand) this.SearchCommand ).RaiseCanExecuteChanged();
                        ( (DelegateCommand) this.LogOffCommand ).RaiseCanExecuteChanged();
                        if (this.LoginCommand.CanExecute(null))
                        {
                            this.LoginCommand.Execute(null);
                        }
                    }, 
                () => settingsService.IsLoggedIn);
        }

        /// <summary>Gets the load command.</summary>
        public ICommand LoadCommand { get; }

        /// <summary>Gets the log off command.</summary>
        public ICommand LogOffCommand { get; }

        /// <summary>Gets the login command.</summary>
        public ICommand LoginCommand { get; }

        /// <summary>Gets the search command.</summary>
        public ICommand SearchCommand { get; }

        public string SomeFoundContent
        {
            get
            {
                return this._someFoundContent;
            }
            set
            {
                this._someFoundContent = value;
                this.OnPropertyChanged(() => this.SomeFoundContent);
            }
        }
    }
}