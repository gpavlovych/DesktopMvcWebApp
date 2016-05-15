using System.Windows.Input;
using MyApplication.Desktop.Services;
using MyApplication.Desktop.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace MyApplication.Desktop.ViewModels
{
    /// <summary>The login window view model.</summary>
    public class LoginWindowViewModel : BindableBase
    {
        /// <summary>The password.</summary>
        private string _password;

        /// <summary>The user name.</summary>
        private string _userName;

        /// <summary>Initializes a new instance of the <see cref="LoginWindowViewModel"/> class.</summary>
        /// <param name="settingsService">The settings service.</param>
        public LoginWindowViewModel(ISettingsService settingsService)
        {
            this.ReloadCredentialsCommand = new DelegateCommand(
                () =>
                    {
                        this.UserName = settingsService.UserName;
                        this.Password = settingsService.Password;
                    }, 
                () => !settingsService.IsLoggedIn);
            this.LoginCommand = new DelegateCommand<ILoginWindow>(
                window =>
                    {
                        settingsService.UserName = this.UserName;
                        settingsService.Password = this.Password;
                        settingsService.IsLoggedIn = true;
                        settingsService.Save();
                        ( (DelegateCommand) this.ReloadCredentialsCommand ).RaiseCanExecuteChanged();
                        ( (DelegateCommand<ILoginWindow>) this.LoginCommand ).RaiseCanExecuteChanged();
                        window.DialogResult = true;
                    }, 
                window => !settingsService.IsLoggedIn);
            this.CancelCommand = new DelegateCommand<ILoginWindow>(
                window =>
                    {
                        window.DialogResult = false;
                    });
        }

        /// <summary>Gets the reload credentials command.</summary>
        public ICommand ReloadCredentialsCommand { get; }

        /// <summary>Gets the login command.</summary>
        public ICommand LoginCommand { get; }

        /// <summary>Gets the cancel command.</summary>
        public ICommand CancelCommand { get; }

        /// <summary>Gets or sets the user name.</summary>
        public string UserName
        {
            get
            {
                return this._userName;
            }

            set
            {
                this._userName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the password.</summary>
        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                this._password = value;
                this.OnPropertyChanged();
            }
        }
    }
}