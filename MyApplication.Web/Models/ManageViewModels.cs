// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageViewModels.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The index view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MyApplication.Web.Models
{
    /// <summary>TODO The index view model.</summary>
    public class IndexViewModel
    {
        /// <summary>Gets or sets a value indicating whether has password.</summary>
        public bool HasPassword { get; set; }

        /// <summary>Gets or sets the logins.</summary>
        public IList<UserLoginInfo> Logins { get; set; }

        /// <summary>Gets or sets the phone number.</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets a value indicating whether two factor.</summary>
        public bool TwoFactor { get; set; }

        /// <summary>Gets or sets a value indicating whether browser remembered.</summary>
        public bool BrowserRemembered { get; set; }
    }

    /// <summary>TODO The manage logins view model.</summary>
    public class ManageLoginsViewModel
    {
        /// <summary>Gets or sets the current logins.</summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>Gets or sets the other logins.</summary>
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    /// <summary>TODO The factor view model.</summary>
    public class FactorViewModel
    {
        /// <summary>Gets or sets the purpose.</summary>
        public string Purpose { get; set; }
    }

    /// <summary>TODO The set password view model.</summary>
    public class SetPasswordViewModel
    {
        /// <summary>Gets or sets the new password.</summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        /// <summary>Gets or sets the confirm password.</summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>TODO The change password view model.</summary>
    public class ChangePasswordViewModel
    {
        /// <summary>Gets or sets the old password.</summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        /// <summary>Gets or sets the new password.</summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        /// <summary>Gets or sets the confirm password.</summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>TODO The add phone number view model.</summary>
    public class AddPhoneNumberViewModel
    {
        /// <summary>Gets or sets the number.</summary>
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    /// <summary>TODO The verify phone number view model.</summary>
    public class VerifyPhoneNumberViewModel
    {
        /// <summary>Gets or sets the code.</summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>Gets or sets the phone number.</summary>
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    /// <summary>TODO The configure two factor view model.</summary>
    public class ConfigureTwoFactorViewModel
    {
        /// <summary>Gets or sets the selected provider.</summary>
        public string SelectedProvider { get; set; }

        /// <summary>Gets or sets the providers.</summary>
        public ICollection<SelectListItem> Providers { get; set; }
    }
}