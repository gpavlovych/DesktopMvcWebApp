// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountViewModels.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The external login confirmation view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyApplication.Web.Models
{
    /// <summary>TODO The external login confirmation view model.</summary>
    public class ExternalLoginConfirmationViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>TODO The external login list view model.</summary>
    public class ExternalLoginListViewModel
    {
        /// <summary>Gets or sets the return url.</summary>
        public string ReturnUrl { get; set; }
    }

    /// <summary>TODO The send code view model.</summary>
    public class SendCodeViewModel
    {
        /// <summary>Gets or sets the selected provider.</summary>
        public string SelectedProvider { get; set; }

        /// <summary>Gets or sets the providers.</summary>
        public ICollection<SelectListItem> Providers { get; set; }

        /// <summary>Gets or sets the return url.</summary>
        public string ReturnUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether remember me.</summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>TODO The verify code view model.</summary>
    public class VerifyCodeViewModel
    {
        /// <summary>Gets or sets the provider.</summary>
        [Required]
        public string Provider { get; set; }

        /// <summary>Gets or sets the code.</summary>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        /// <summary>Gets or sets the return url.</summary>
        public string ReturnUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether remember browser.</summary>
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        /// <summary>Gets or sets a value indicating whether remember me.</summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>TODO The forgot view model.</summary>
    public class ForgotViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>TODO The login view model.</summary>
    public class LoginViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>Gets or sets a value indicating whether remember me.</summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    /// <summary>TODO The register view model.</summary>
    public class RegisterViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>Gets or sets the confirm password.</summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>TODO The reset password view model.</summary>
    public class ResetPasswordViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>Gets or sets the confirm password.</summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>Gets or sets the code.</summary>
        public string Code { get; set; }
    }

    /// <summary>TODO The forgot password view model.</summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>Gets or sets the email.</summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
