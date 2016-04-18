// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountControllerTests.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The account controller tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Threading.Tasks;
using System.Web.Mvc;
using FluentAssertions;
using MyApplication.Web.Models;
using MyApplication.Web.Tests.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Controllers.Tests
{
    /// <summary>TODO The account controller tests.</summary>
    [TestClass]
    public class AccountControllerTests
    {
        #region Login

        /// <summary>TODO The login get test.</summary>
        [TestMethod]
        public void LoginGetTest()
        {
           // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var returnUrl = "someReturnUrl";

            // act
            var result = target.Login(returnUrl) as ViewResult;

            // assert
            result.ViewData["ReturnUrl"].Should().Be(returnUrl);
        }

        /// <summary>TODO The login post test success url local.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestSuccessUrlLocal()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .Returns(Task.FromResult(SignInStatus.Success));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = "/myurl";

            // act
            var result = await target.Login(model, returnUrl) as RedirectResult;

            // assert
            result.Url.Should().Be(returnUrl);
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        /// <summary>TODO The login post test success url non local.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestSuccessUrlNonLocal()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .Returns(Task.FromResult(SignInStatus.Success));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = "http://myurl";

            // act
            var result = await target.Login(model, returnUrl) as RedirectToRouteResult;

            // assert
            result.RouteValues["controller"].Should().Be("Publications");
            result.RouteValues["action"].Should().Be("Index");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        /// <summary>TODO The login post test locked out.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestLockedOut()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .Returns(Task.FromResult(SignInStatus.LockedOut));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = "http://myurl";

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.ViewName.Should().Be("Lockout");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        /// <summary>TODO The login post test requires verification.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestRequiresVerification()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .Returns(Task.FromResult(SignInStatus.RequiresVerification));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = "http://myurl";

            // act
            var result = await target.Login(model, returnUrl) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("SendCode");
            result.RouteValues["ReturnUrl"].Should().Be(returnUrl);
            result.RouteValues["RememberMe"].Should().Be(model.RememberMe);
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        /// <summary>TODO The login post test failure.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestFailure()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .Returns(Task.FromResult(SignInStatus.Failure));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = "http://myurl";

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[string.Empty].Errors.Should().Contain(it => it.ErrorMessage == "Invalid login attempt.");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        /// <summary>TODO The login post test model state invalid.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task LoginPostTestModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            
            var model = fixture.Build<LoginViewModel>().Create();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();

            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            target.ModelState.AddModelError(string.Empty, "someError");
            var returnUrl = "http://myurl";

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false), Times.Never());
        }

        #endregion Login

        #region VerifyCode

        /// <summary>TODO The verify code get test has been verified.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodeGetTestHasBeenVerified()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.HasBeenVerifiedAsync()).Returns(Task.FromResult(true));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
       
            string provider = "someProvider";
            string returnUrl = "http://myurl"; 
            bool remember = false;

            // act
            var result = await target.VerifyCode(provider, returnUrl, remember) as ViewResult;
            
            // assert
            result.Model.ShouldBeEquivalentTo(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = remember });
            signInManagerMock.Verify(it => it.HasBeenVerifiedAsync());
        }

        /// <summary>TODO The verify code get test has not been verified.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodeGetTestHasNotBeenVerified()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.HasBeenVerifiedAsync()).Returns(Task.FromResult(false));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            string provider = "someProvider";
            string returnUrl = "http://myurl"; 
            bool remember = false;

            // act
            var result = await target.VerifyCode(provider, returnUrl, remember) as ViewResult;

            // assert
            result.ViewName.Should().Be("Error");
            signInManagerMock.Verify(it => it.HasBeenVerifiedAsync());
        }

        /// <summary>TODO The verify code post test success url local.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodePostTestSuccessUrlLocal()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().With(it => it.ReturnUrl, "/localurl").Create();
            signInManagerMock.Setup(
                it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser))
                .Returns(Task.FromResult(SignInStatus.Success));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as RedirectResult;

            // assert
            result.Url.Should().Be(model.ReturnUrl);
            signInManagerMock.Verify(
                it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        /// <summary>TODO The verify code post test success url remote.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodePostTestSuccessUrlRemote()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().With(it => it.ReturnUrl, "http://localurl").Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).Returns(Task.FromResult(SignInStatus.Success));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["controller"].Should().Be("Publications");
            result.RouteValues["action"].Should().Be("Index");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        /// <summary>TODO The verify code post test locked out.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodePostTestLockedOut()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).Returns(Task.FromResult(SignInStatus.LockedOut));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.ViewName.Should().Be("Lockout");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        /// <summary>TODO The verify code test failure.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodeTestFailure()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).Returns(Task.FromResult(SignInStatus.Failure));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[string.Empty].Errors.Should().Contain(it => it.ErrorMessage == "Invalid code.");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        /// <summary>TODO The verify code test model state invalid.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task VerifyCodeTestModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).Returns(Task.FromResult(SignInStatus.Failure));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            target.ModelState.AddModelError(string.Empty, "someError");

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser), Times.Never());
        }

        #endregion VerifyCode

        #region Register

        /// <summary>TODO The register get test.</summary>
        [TestMethod]
        public void RegisterGetTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.Register() as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        /// <summary>TODO The register post test model state valid local.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task RegisterPostTestModelStateValidLocal()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).Returns(Task.FromResult(IdentityResult.Success));

            signInManagerMock.Setup(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false))
                .Returns(Task.Run(() => { }));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.Register(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["controller"].Should().Be("Publications");
            result.RouteValues["action"].Should().Be("Index");
            userManagerMock.Verify(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password));
            signInManagerMock.Verify(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false));
        }

        /// <summary>TODO The register test failure.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task RegisterTestFailure()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).Returns(Task.FromResult(IdentityResult.Failed("Invalid")));

            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.Register(model) as ViewResult;
            
            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            userManagerMock.Verify(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password));
            signInManagerMock.Verify(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false), Times.Never());
        }

        /// <summary>TODO The register test model state invalid.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task RegisterTestModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            userManagerMock.Setup(it => it.CreateAsync(user, model.Password)).Returns(Task.FromResult(IdentityResult.Failed("Invalid")));

            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            target.ModelState.AddModelError(string.Empty, "someError");

            // act
            var result = await target.Register(model) as ViewResult;

            // assert
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            userManagerMock.Verify(it => it.CreateAsync(user, model.Password), Times.Never());
            signInManagerMock.Verify(it => it.SignInAsync(user, false, false), Times.Never());
        }

        #endregion VerifyCode

        #region ConfirmEmail

        /// <summary>TODO The confirm email test non null succeeded.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ConfirmEmailTestNonNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var target = fixture.Create<AccountController>();
            var userId = "someUsr";
            var code = "someCode";
            userManagerMock.Setup(it => it.ConfirmEmailAsync(userId, code))
                .Returns(Task.FromResult(IdentityResult.Success));

            // act
            var result = await target.ConfirmEmail(userId, code) as ViewResult;

            // assert
            result.ViewName.Should().Be("ConfirmEmail");
            userManagerMock.Verify(it => it.ConfirmEmailAsync(userId, code));
        }

        /// <summary>TODO The confirm email test non null failed.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ConfirmEmailTestNonNullFailed()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var target = fixture.Create<AccountController>();
            var userId = "someUsr";
            var code = "someCode";
            userManagerMock.Setup(it => it.ConfirmEmailAsync(userId, code))
                .Returns(Task.FromResult(IdentityResult.Failed("someResult")));

            // act
            var result = await target.ConfirmEmail(userId, code) as ViewResult;

            // assert
            result.ViewName.Should().Be("Error");
            userManagerMock.Verify(it => it.ConfirmEmailAsync(userId, code));
        }

        /// <summary>TODO The confirm email test null succeeded.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ConfirmEmailTestNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var code = "someCode";
            
            // act
            var result = await target.ConfirmEmail(null, code) as ViewResult;

            // assert
            result.ViewName.Should().Be("Error");
        }

        /// <summary>TODO The confirm email test code null succeeded.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ConfirmEmailTestCodeNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var userId = "someUsr";

            // act
            var result = await target.ConfirmEmail(userId, null) as ViewResult;

            // assert
            result.ViewName.Should().Be("Error");
        }

        #endregion ConfirmEmail

        #region Forgot password

        /// <summary>TODO The forgot password test.</summary>
        [TestMethod]
        public void ForgotPasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ForgotPassword() as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        /// <summary>TODO The forgot password post test.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ForgotPasswordPostTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var user =
                fixture.Build<ApplicationUser>().Create();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).Returns(Task.FromResult(user));
            userManagerMock.Setup(it => it.IsEmailConfirmedAsync(user.Id)).Returns(Task.FromResult(false));

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.ViewName.Should().Be("ForgotPasswordConfirmation");
        }

        /// <summary>TODO The forgot password post test not email confirmed.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ForgotPasswordPostTestNotEmailConfirmed()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var user =
                fixture.Build<ApplicationUser>().Create();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).Returns(Task.FromResult(user));
            userManagerMock.Setup(it => it.IsEmailConfirmedAsync(user.Id)).Returns(Task.FromResult(true));

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(viewModel);
        }

        /// <summary>TODO The forgot password post test user null.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ForgotPasswordPostTestUserNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).Returns(Task.FromResult((ApplicationUser)null));
         
            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.ViewName.Should().Be("ForgotPasswordConfirmation");
        }

        /// <summary>TODO The forgot password post test invalid model state.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ForgotPasswordPostTestInvalidModelState()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            target.ModelState.AddModelError(string.Empty, "someErrors");

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(viewModel);
        }

        #endregion ForgotPassword

        #region ForgotPasswordConfirmation

        /// <summary>TODO The forgot password confirmation test.</summary>
        [TestMethod]
        public void ForgotPasswordConfirmationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ForgotPasswordConfirmation() as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        #endregion ForgotPasswordConfirmation

        #region ResetPassword

        /// <summary>TODO The reset password test.</summary>
        [TestMethod]
        public void ResetPasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPassword("someCode") as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        /// <summary>TODO The reset password test null.</summary>
        [TestMethod]
        public void ResetPasswordTestNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPassword((string)null) as ViewResult;

            // assert
            result.ViewName.Should().Be("Error");
        }

        /// <summary>TODO The reset password test post.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ResetPasswordTestPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            var user =
                 fixture.Build<ApplicationUser>().Create();
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).Returns(Task.FromResult(user));
            userManagerMock.Setup(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password)).Returns(Task.FromResult(IdentityResult.Success));

            // act
            var result = await target.ResetPassword(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("ResetPasswordConfirmation");
            result.RouteValues["controller"].Should().Be("Account");
            userManagerMock.Verify(it=>it.ResetPasswordAsync(user.Id, model.Code, model.Password));
        }

        /// <summary>TODO The reset password result failed test post.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ResetPasswordResultFailedTestPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            var user =
                 fixture.Build<ApplicationUser>().Create();
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).Returns(Task.FromResult(user));
            userManagerMock.Setup(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password)).Returns(Task.FromResult(IdentityResult.Failed()));

            // act
            var result = await target.ResetPassword(model) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
            userManagerMock.Verify(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password));
        }

        /// <summary>TODO The reset password test user null post.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ResetPasswordTestUserNullPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).Returns(Task.FromResult((ApplicationUser)null));

            // act
            var result = await target.ResetPassword(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("ResetPasswordConfirmation");
            result.RouteValues["controller"].Should().Be("Account");
        }

        /// <summary>TODO The reset password test model invalid post.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [TestMethod]
        public async Task ResetPasswordTestModelInvalidPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            target.ModelState.AddModelError(string.Empty, "TestError");

            // act
            var result = await target.ResetPassword(model) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(model);
        }

        #endregion ResetPassword

        #region ResetPasswordConfirmation

        /// <summary>TODO The reset password confirmation test.</summary>
        [TestMethod]
        public void ResetPasswordConfirmationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPasswordConfirmation() as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        #endregion ResetPasswordConfirmation

        /// <summary>TODO The external login test.</summary>
        [TestMethod]
        public void ExternalLoginTest()
        {
            Assert.Inconclusive();
        }

        /// <summary>TODO The send code test.</summary>
        [TestMethod()]
        public void SendCodeTest()
        {
            Assert.Inconclusive();
        }

        /// <summary>TODO The send code test 1.</summary>
        [TestMethod()]
        public void SendCodeTest1()
        {
            Assert.Inconclusive();
        }

        /// <summary>TODO The external login callback test.</summary>
        [TestMethod()]
        public void ExternalLoginCallbackTest()
        {
            Assert.Inconclusive();
        }

        /// <summary>TODO The external login confirmation test.</summary>
        [TestMethod()]
        public void ExternalLoginConfirmationTest()
        {
            Assert.Inconclusive();
        }

        #region LogOff

        /// <summary>TODO The log off test.</summary>
        [TestMethod]
        public void LogOffTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.LogOff() as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Publications");
        }

        #endregion LogOff

        #region Dispose

        /// <summary>TODO The dispose test.</summary>
        [TestMethod]
        public void DisposeTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var target = fixture.Create<AccountController>();

            // act
            target.Dispose();

            // assert
            userManagerMock.Verify(it => it.Dispose());
            signInManagerMock.Verify(it => it.Dispose());
        }

        #endregion Dispose

        /// <summary>TODO The external login failure test.</summary>
        [TestMethod()]
        public void ExternalLoginFailureTest()
        {
            Assert.Inconclusive();
        }
    }
}