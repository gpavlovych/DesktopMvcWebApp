using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FluentAssertions;
using MyApplication.Web.Models;
using MyApplication.Web.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyApplication.Web.Services;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        #region Login

        [TestMethod]
        public void LoginGetTest()
        {
           // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var returnUrl = fixture.Create<string>("someReturnUrl");

            // act
            var result = target.Login(returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewData["ReturnUrl"].Should().Be(returnUrl);
        }

        [TestMethod]
        public async Task LoginPostTestSuccessUrlLocal()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInStatus.Success);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = fixture.Create<string>("/myurl");

            // act
            var result = await target.Login(model, returnUrl) as RedirectResult;

            // assert
            result.Should().NotBeNull();
            result.Url.Should().Be(returnUrl);
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        [TestMethod]
        public async Task LoginPostTestSuccessUrlNonLocal()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInStatus.Success);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = fixture.Create<string>("http://myurl");

            // act
            var result = await target.Login(model, returnUrl) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["controller"].Should().Be("Some");
            result.RouteValues["action"].Should().Be("Index");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        [TestMethod]
        public async Task LoginPostTestLockedOut()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInStatus.LockedOut);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = fixture.Create<string>("http://myurl");

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Lockout");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        [TestMethod]
        public async Task LoginPostTestRequiresVerification()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInStatus.RequiresVerification);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = fixture.Create<string>("http://myurl");

            // act
            var result = await target.Login(model, returnUrl) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("SendCode");
            result.RouteValues["ReturnUrl"].Should().Be(returnUrl);
            result.RouteValues["RememberMe"].Should().Be(model.RememberMe);
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

        [TestMethod]
        public async Task LoginPostTestFailure()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<LoginViewModel>().Create();
            signInManagerMock.Setup(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
                .ReturnsAsync(SignInStatus.Failure);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            var returnUrl = fixture.Create<string>("http://myurl");

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[string.Empty].Errors.Should().Contain(it => it.ErrorMessage == "Invalid login attempt.");
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false));
        }

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
            target.ModelState.AddModelError(string.Empty, fixture.Create<string>());
            var returnUrl = fixture.Create<string>("http://myurl");

            // act
            var result = await target.Login(model, returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            signInManagerMock.Verify(it => it.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false), Times.Never());
        }

        #endregion Login

        #region VerifyCode

        [TestMethod]
        public async Task VerifyCodeGetTestHasBeenVerified()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.HasBeenVerifiedAsync()).ReturnsAsync(true);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
       
            string provider = fixture.Create<string>();
            string returnUrl = fixture.Create<string>("http://myurl"); 
            bool remember = fixture.Create<bool>();

            // act
            var result = await target.VerifyCode(provider, returnUrl, remember) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.ShouldBeEquivalentTo(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = remember });
            signInManagerMock.Verify(it => it.HasBeenVerifiedAsync());
        }

        [TestMethod]
        public async Task VerifyCodeGetTestHasNotBeenVerified()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.HasBeenVerifiedAsync()).ReturnsAsync(false);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            string provider = fixture.Create<string>();
            string returnUrl = fixture.Create<string>("http://myurl");
            bool remember = fixture.Create<bool>();

            // act
            var result = await target.VerifyCode(provider, returnUrl, remember) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
            signInManagerMock.Verify(it => it.HasBeenVerifiedAsync());
        }

        [TestMethod]
        public async Task VerifyCodePostTestSuccessUrlLocal()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().With(it => it.ReturnUrl, fixture.Create<string>("/localurl")).Create();
            signInManagerMock.Setup(
                it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser))
                .ReturnsAsync(SignInStatus.Success);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as RedirectResult;

            // assert
            result.Should().NotBeNull();
            result.Url.Should().Be(model.ReturnUrl);
            signInManagerMock.Verify(
                it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        [TestMethod]
        public async Task VerifyCodePostTestSuccessUrlRemote()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().With(it => it.ReturnUrl, fixture.Create<string>("http://localurl")).Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).ReturnsAsync(SignInStatus.Success);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["controller"].Should().Be("Some");
            result.RouteValues["action"].Should().Be("Index");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        [TestMethod]
        public async Task VerifyCodePostTestLockedOut()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).ReturnsAsync(SignInStatus.LockedOut);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Lockout");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        [TestMethod]
        public async Task VerifyCodeTestFailure()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).ReturnsAsync(SignInStatus.Failure);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[string.Empty].Errors.Should().Contain(it => it.ErrorMessage == "Invalid code.");
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        [TestMethod]
        public async Task VerifyCodeTestModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<VerifyCodeViewModel>().Create();
            signInManagerMock.Setup(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser)).ReturnsAsync(SignInStatus.Failure);
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

            // act
            var result = await target.VerifyCode(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            signInManagerMock.Verify(it => it.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser), Times.Never());
        }

        #endregion VerifyCode

        #region Register

        [TestMethod]
        public void RegisterGetTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.Register() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RegisterPostTestModelStateValidLocal()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);

            signInManagerMock.Setup(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false))
                .Returns(Task.Run(() => { }));
            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.Register(model) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["controller"].Should().Be("Some");
            result.RouteValues["action"].Should().Be("Index");
            userManagerMock.Verify(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password));
            signInManagerMock.Verify(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false));
        }

        [TestMethod]
        public async Task RegisterTestFailure()
        {
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Failed("Invalid"));

            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();

            // act
            var result = await target.Register(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            userManagerMock.Verify(it => it.CreateAsync(It.IsAny<ApplicationUser>(), model.Password));
            signInManagerMock.Verify(it => it.SignInAsync(It.IsAny<ApplicationUser>(), false, false), Times.Never());
        }

        [TestMethod]
        public async Task RegisterTestModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var model = fixture.Build<RegisterViewModel>().Create();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            userManagerMock.Setup(it => it.CreateAsync(user, model.Password)).ReturnsAsync(IdentityResult.Failed("Invalid"));

            var target = fixture.Create<AccountController>();
            target.Url = fixture.Create<UrlHelper>();
            target.ModelState.Clear();
            target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

            // act
            var result = await target.Register(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.Model.Should().BeSameAs(model);
            target.ModelState.IsValid.Should().BeFalse();
            userManagerMock.Verify(it => it.CreateAsync(user, model.Password), Times.Never());
            signInManagerMock.Verify(it => it.SignInAsync(user, false, false), Times.Never());
        }

        #endregion VerifyCode

        #region ConfirmEmail

        [TestMethod]
        public async Task ConfirmEmailTestNonNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var target = fixture.Create<AccountController>();
            var userId = fixture.Create<string>();
            var code = fixture.Create<string>();
            userManagerMock.Setup(it => it.ConfirmEmailAsync(userId, code))
                .ReturnsAsync(IdentityResult.Success);

            // act
            var result = await target.ConfirmEmail(userId, code) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("ConfirmEmail");
            userManagerMock.Verify(it => it.ConfirmEmailAsync(userId, code));
        }

        [TestMethod]
        public async Task ConfirmEmailTestNonNullFailed()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var target = fixture.Create<AccountController>();
            var userId = fixture.Create<string>();
            var code = fixture.Create<string>();
            userManagerMock.Setup(it => it.ConfirmEmailAsync(userId, code))
                .ReturnsAsync(IdentityResult.Failed(fixture.Create<string>()));

            // act
            var result = await target.ConfirmEmail(userId, code) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
            userManagerMock.Verify(it => it.ConfirmEmailAsync(userId, code));
        }

        [TestMethod]
        public async Task ConfirmEmailTestNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var code = fixture.Create<string>();

            // act
            var result = await target.ConfirmEmail(null, code) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        [TestMethod]
        public async Task ConfirmEmailTestCodeNullSucceeded()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();
            var userId = fixture.Create<string>();

            // act
            var result = await target.ConfirmEmail(userId, null) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        #endregion ConfirmEmail

        #region Forgot password

        [TestMethod]
        public void ForgotPasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ForgotPassword() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
        }

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
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).ReturnsAsync(user);
            userManagerMock.Setup(it => it.IsEmailConfirmedAsync(user.Id)).ReturnsAsync(false);

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("ForgotPasswordConfirmation");
        }

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
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).ReturnsAsync(user);
            userManagerMock.Setup(it => it.IsEmailConfirmedAsync(user.Id)).ReturnsAsync(true);

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(viewModel);
        }

        [TestMethod]
        public async Task ForgotPasswordPostTestUserNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(viewModel.Email)).ReturnsAsync(null);
         
            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("ForgotPasswordConfirmation");
        }

        [TestMethod]
        public async Task ForgotPasswordPostTestInvalidModelState()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var viewModel = fixture.Build<ForgotPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

            // act
            var result = await target.ForgotPassword(viewModel) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(viewModel);
        }

        #endregion ForgotPassword

        #region ForgotPasswordConfirmation

        [TestMethod]
        public void ForgotPasswordConfirmationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ForgotPasswordConfirmation() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
        }

        #endregion ForgotPasswordConfirmation

        #region ResetPassword

        [TestMethod]
        public void ResetPasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPassword(fixture.Create<string>()) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
        }

        [TestMethod]
        public void ResetPasswordTestNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPassword((string)null) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

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
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).ReturnsAsync(user);
            userManagerMock.Setup(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password)).ReturnsAsync(IdentityResult.Success);

            // act
            var result = await target.ResetPassword(model) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("ResetPasswordConfirmation");
            result.RouteValues["controller"].Should().Be("Account");
            userManagerMock.Verify(it=>it.ResetPasswordAsync(user.Id, model.Code, model.Password));
        }

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
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).ReturnsAsync(user);
            userManagerMock.Setup(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password)).ReturnsAsync(IdentityResult.Failed());

            // act
            var result = await target.ResetPassword(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            userManagerMock.Verify(it => it.ResetPasswordAsync(user.Id, model.Code, model.Password));
        }

        [TestMethod]
        public async Task ResetPasswordTestUserNullPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            userManagerMock.Setup(it => it.FindByNameAsync(model.Email)).ReturnsAsync(null);

            // act
            var result = await target.ResetPassword(model) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("ResetPasswordConfirmation");
            result.RouteValues["controller"].Should().Be("Account");
        }

        [TestMethod]
        public async Task ResetPasswordTestModelInvalidPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ResetPasswordViewModel>().Create();
            var target = fixture.Create<AccountController>();
            target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

            // act
            var result = await target.ResetPassword(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().BeSameAs(model);
        }

        #endregion ResetPassword

        #region ResetPasswordConfirmation

        [TestMethod]
        public void ResetPasswordConfirmationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ResetPasswordConfirmation() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
        }

        #endregion ResetPasswordConfirmation

        #region ExternalLogin

        [TestMethod]
        public void ExternalLoginTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var provider = fixture.Create<string>();
            var returnUrl = fixture.Create<string>();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ExternalLogin(provider, returnUrl) as HttpUnauthorizedResult;

            // assert
            result.Should().NotBeNull();
        }

        #endregion ExternalLogin

        #region SendCode

        [TestMethod]
        public async Task SendCodeTestUserIdNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var rememberMe = fixture.Create<bool>();
            var returnUrl = fixture.Create<string>();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.GetVerifiedUserIdAsync()).ReturnsAsync(null);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.SendCode(returnUrl, rememberMe) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        [TestMethod]
        public async Task SendCodeTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var rememberMe = fixture.Create<bool>();
            var returnUrl = fixture.Create<string>();
            var userId = fixture.Create<string>();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.GetVerifiedUserIdAsync()).ReturnsAsync(userId);
            var userFactorsMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var providers = fixture.CreateMany<string>().ToList() as IList<string>;
            userFactorsMock.Setup(it => it.GetValidTwoFactorProvidersAsync(userId)).ReturnsAsync(providers);
            var factorOptions = providers.Select(
                 purpose => new SelectListItem
                 {
                     Text = purpose,
                     Value = purpose
                 }).ToList();
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.SendCode(returnUrl, rememberMe) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.Model.ShouldBeEquivalentTo(new SendCodeViewModel
            {
                Providers = factorOptions,
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            });
        }

        [TestMethod]
        public async Task SendCodeTestPostModelStateInvalid()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<SendCodeViewModel>().Create();
            var target = fixture.Create<AccountController>();
            target.ModelState.AddModelError(fixture.Create<string>(), fixture.Create<string>());
           
            // act
            var result = await target.SendCode(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.ViewData.Should().BeEmpty();
        }
        
        [TestMethod]
        public async Task SendCodeTestPostSendTwoFactoryCodeFalse()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<SendCodeViewModel>().Create();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.SendTwoFactorCodeAsync(model.SelectedProvider))
                             .ReturnsAsync(false);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.SendCode(model) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Error");
        }

        [TestMethod]
        public async Task SendCodeTestPost()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<SendCodeViewModel>().Create();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.SendTwoFactorCodeAsync(model.SelectedProvider))
                             .ReturnsAsync(true);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.SendCode(model) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "VerifyCode");
            result.RouteValues.Should().Contain("Provider", model.SelectedProvider);
            result.RouteValues.Should().Contain("ReturnUrl", model.ReturnUrl);
            result.RouteValues.Should().Contain("RememberMe", model.RememberMe);
        }

        #endregion SendCode

        #region ExternalLoginCallback

        [TestMethod]
        public async Task ExternalLoginCallbackTestLoginInfoNull()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var returnUrl = fixture.Create<string>();
            ExternalLoginInfo loginInfo = null;
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(loginInfo);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginCallback(returnUrl) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "Login");
        }

        [TestMethod]
        public async Task ExternalLoginCallbackTestLoginInfoSuccess()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var returnUrl = fixture.Create<string>("/myurl");
            var loginInfo =
                fixture.Build<ExternalLoginInfo>()
                    .Without(it => it.ExternalIdentity)
                    .Without(it => it.Login)
                    .Create();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(loginInfo);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.ExternalSignInAsync(loginInfo, false))
                             .ReturnsAsync(SignInStatus.Success);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginCallback(returnUrl) as RedirectResult;

            // assert
            result.Should().NotBeNull();
            result.Url.Should().Be(returnUrl);
        }

        [TestMethod]
        public async Task ExternalLoginCallbackTestLoginInfoLockedOut()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var returnUrl = fixture.Create<string>();
            var loginInfo =
               fixture.Build<ExternalLoginInfo>()
                   .Without(it => it.ExternalIdentity)
                   .Without(it => it.Login)
                   .Create();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(loginInfo);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.ExternalSignInAsync(loginInfo, false))
                             .ReturnsAsync(SignInStatus.LockedOut);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginCallback(returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().Contain("Lockout");
        }

        [TestMethod]
        public async Task ExternalLoginCallbackTestLoginInfoRequiresVerification()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var returnUrl = fixture.Create<string>();
            var loginInfo =
                fixture.Build<ExternalLoginInfo>()
                    .Without(it => it.ExternalIdentity)
                    .Without(it => it.Login)
                    .Create();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(loginInfo);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.ExternalSignInAsync(loginInfo, false))
                             .ReturnsAsync(SignInStatus.RequiresVerification);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginCallback(returnUrl) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "SendCode");
            result.RouteValues.Should().Contain("ReturnUrl", returnUrl);
            result.RouteValues.Should().Contain("RememberMe", false);
        }

        [TestMethod]
        public async Task ExternalLoginCallbackTestLoginInfoFailure()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var returnUrl = fixture.Create<string>();
            var loginInfo =
                fixture.Build<ExternalLoginInfo>()
                    .Without(it => it.ExternalIdentity)
                    .Create();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(loginInfo);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            signInManagerMock.Setup(it => it.ExternalSignInAsync(loginInfo, false))
                             .ReturnsAsync(SignInStatus.Failure);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginCallback(returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewData.Should().Contain("ReturnUrl", returnUrl);
            result.ViewData.Should().Contain("LoginProvider", loginInfo.Login.LoginProvider);
            result.ViewName.Should().Contain("ExternalLoginConfirmation");
            result.Model.ShouldBeEquivalentTo(
                new ExternalLoginConfirmationViewModel
                    {
                        Email = loginInfo.Email
                    });
        }

        #endregion ExternalLoginCallback

        #region ExternalLoginConfirmation

        [TestMethod]
        public async Task ExternalLoginConfirmationTestIsAuthenticated()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ExternalLoginConfirmationViewModel>().Create();
            var returnUrl = fixture.Create<string>();
            fixture.IdentityMock.SetupGet(it => it.IsAuthenticated).Returns(true);
            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginConfirmation(model, returnUrl) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "Index");
            result.RouteValues.Should().Contain("controller", "Manage");
        }

        [TestMethod]
        public async Task ExternalLoginConfirmationTestCreateFailed()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ExternalLoginConfirmationViewModel>().Create();
            var returnUrl = fixture.Create<string>("/myurl");
            var externalLoginInfo = fixture.Build<ExternalLoginInfo>().Without(it => it.ExternalIdentity).Create();
            fixture.IdentityMock.SetupGet(it => it.IsAuthenticated).Returns(false);

            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(externalLoginInfo);

            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var userId = fixture.Create<string>();
            var error = fixture.Create<string>();

            ApplicationUser createdUser = null;
            userManagerMock.Setup(
                it =>
                it.CreateAsync(
                    It.IsAny<ApplicationUser>()))
                           .Returns<ApplicationUser>(
                               user =>
                               {
                                   createdUser = user;
                                   user.Id = userId;
                                   return Task.FromResult(IdentityResult.Failed(error));
                               });

            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginConfirmation(model, returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            createdUser.Should().NotBeNull();
            createdUser.UserName.Should().Be(model.Email);
            createdUser.Email.Should().Be(model.Email);
            result.ViewName.Should().BeNullOrEmpty();
            result.Model.Should().BeSameAs(model);
            result.ViewData.Should().Contain("ReturnUrl", returnUrl);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[string.Empty].Errors.Should().ContainSingle(it => it.ErrorMessage == error);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmationTestAddLoginFailed()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ExternalLoginConfirmationViewModel>().Create();
            var returnUrl = fixture.Create<string>("/myurl");
            var externalLoginInfo = fixture.Build<ExternalLoginInfo>().Without(it => it.ExternalIdentity).Create();
            fixture.IdentityMock.SetupGet(it => it.IsAuthenticated).Returns(false);

            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(externalLoginInfo);

            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var userId = fixture.Create<string>();
            ApplicationUser createdUser = null;
            userManagerMock.Setup(
                it =>
                it.CreateAsync(
                    It.IsAny<ApplicationUser>()))
                           .Returns<ApplicationUser>(
                               user =>
                               {
                                   createdUser = user;
                                   user.Id = userId;
                                   return Task.FromResult(IdentityResult.Success);
                               });
            var error = fixture.Create<string>();
            userManagerMock.Setup(
                it =>
                it.AddLoginAsync(userId, externalLoginInfo.Login))
                           .ReturnsAsync(IdentityResult.Failed(error));

            var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginConfirmation(model, returnUrl) as ViewResult;

            // assert
            result.Should().NotBeNull();
            createdUser.Should().NotBeNull();
            createdUser.UserName.Should().Be(model.Email);
            createdUser.Email.Should().Be(model.Email);
            result.ViewName.Should().BeNullOrEmpty();
            result.Model.Should().BeSameAs(model);
            result.ViewData.Should().Contain("ReturnUrl", returnUrl);
            target.ModelState.IsValid.Should().BeFalse();
            target.ModelState[ string.Empty ].Errors.Should().ContainSingle(it => it.ErrorMessage == error);
        }

        [TestMethod]
        public async Task ExternalLoginConfirmationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var model = fixture.Build<ExternalLoginConfirmationViewModel>().Create();
            var returnUrl = fixture.Create<string>("/myurl");
            var externalLoginInfo = fixture.Build<ExternalLoginInfo>().Without(it => it.ExternalIdentity).Create();
            fixture.IdentityMock.SetupGet(it => it.IsAuthenticated).Returns(false);

            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(externalLoginInfo);

            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var userId = fixture.Create<string>();
            ApplicationUser createdUser = null;
            userManagerMock.Setup(
                it =>
                it.CreateAsync(
                    It.IsAny<ApplicationUser>()))
                           .Returns<ApplicationUser>(
                               user =>
                                   {
                                       createdUser = user;
                                       user.Id = userId;
                                       return Task.FromResult(IdentityResult.Success);
                                   });
            userManagerMock.Setup(
                it =>
                it.AddLoginAsync(userId, externalLoginInfo.Login))
                           .ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            //signInManagerMock.Setup(it => it.SignInAsync(It.Is<ApplicationUser>(user=>user==createdUser), false, false)).Returns(default(object));
                var target = fixture.Create<AccountController>();

            // act
            var result = await target.ExternalLoginConfirmation(model, returnUrl) as RedirectResult;

            // assert
            result.Should().NotBeNull();
            createdUser.Should().NotBeNull();
            createdUser.UserName.Should().Be(model.Email);
            createdUser.Email.Should().Be(model.Email);
            result.Url.Should().Be(returnUrl);
            signInManagerMock.Verify(it=>it.SignInAsync(It.Is<ApplicationUser>(user => user == createdUser), false, false));
        }

        #endregion ExternalLoginConfirmation

        #region LogOff

        [TestMethod]
        public void LogOffTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.LogOff() as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Some");
        }

        #endregion LogOff

        #region Dispose

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

        #region ExternalLoginFailure

        [TestMethod]
        public void ExternalLoginFailureTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<AccountController>();

            // act
            var result = target.ExternalLoginFailure() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.ViewData.Should().BeEmpty();
        }

        #endregion ExternalLoginFailure
    }
}