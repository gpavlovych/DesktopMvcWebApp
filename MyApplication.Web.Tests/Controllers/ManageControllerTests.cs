using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyApplication.Web.Controllers;
using MyApplication.Web.Models;
using MyApplication.Web.Services;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Tests.Controllers
{
    [TestClass]
    public class ManageControllerTests
    {
        #region Index

        private async Task IndexTest(ManageController.ManageMessageId? message)
        {
            //arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var passwordHash = fixture.Create<string>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(
                               fixture.Build<ApplicationUser>().With(it => it.PasswordHash, passwordHash).Create());
            var expectedPhoneNumber = fixture.Create<string>();
            userManagerMock.Setup(it => it.GetPhoneNumberAsync(It.IsAny<string>())).ReturnsAsync(expectedPhoneNumber);
            var expectedTwoFactor = fixture.Create<bool>();
            userManagerMock.Setup(it => it.GetTwoFactorEnabledAsync(It.IsAny<string>())).ReturnsAsync(expectedTwoFactor);
            var expectedLogins = fixture.CreateMany<UserLoginInfo>().ToList();
            userManagerMock.Setup(it => it.GetLoginsAsync(It.IsAny<string>())).ReturnsAsync(expectedLogins);
            var expectedBrowserRemembered = fixture.Create<bool>();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.TwoFactorBrowserRememberedAsync(It.IsAny<string>())).ReturnsAsync(expectedBrowserRemembered);
            var expectedStatusMessage = message == ManageController.ManageMessageId.ChangePasswordSuccess
                                            ? "Your password has been changed."
                                            : message == ManageController.ManageMessageId.SetPasswordSuccess
                                                  ? "Your password has been set."
                                                  : message == ManageController.ManageMessageId.SetTwoFactorSuccess
                                                        ? "Your two-factor authentication provider has been set."
                                                        : message == ManageController.ManageMessageId.Error
                                                              ? "An error has occurred."
                                                              : message
                                                                == ManageController.ManageMessageId.AddPhoneSuccess
                                                                    ? "Your phone number was added."
                                                                    : message
                                                                      == ManageController.ManageMessageId
                                                                                         .RemovePhoneSuccess
                                                                          ? "Your phone number was removed."
                                                                          : string.Empty;
            var target = fixture.Create<ManageController>();

            var expectedModel = new IndexViewModel
                                    {
                                        HasPassword = passwordHash != null,
                                        PhoneNumber = expectedPhoneNumber,
                                        TwoFactor = expectedTwoFactor,
                                        Logins = expectedLogins,
                                        BrowserRemembered = expectedBrowserRemembered
                                    };

            // act
            var result = await target.Index(message) as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewData.Should().Contain("StatusMessage", expectedStatusMessage);
            result.Model.ShouldBeEquivalentTo(expectedModel);
        }

        [TestMethod]
        public async Task IndexTestChangeSetPasswordSuccess()
        {
            await IndexTest(ManageController.ManageMessageId.SetPasswordSuccess);
        }

        [TestMethod]
        public async Task IndexTestChangePasswordSuccess()
        {
            await IndexTest(ManageController.ManageMessageId.ChangePasswordSuccess);
        }

        [TestMethod]
        public async Task IndexTestSetTwoFactorSuccess()
        {
            await IndexTest(ManageController.ManageMessageId.SetTwoFactorSuccess);
        }

        [TestMethod]
        public async Task IndexTestError()
        {
            await IndexTest(ManageController.ManageMessageId.Error);
        }

        [TestMethod]
        public async Task IndexTestAddPhoneSuccess()
        {
            await IndexTest(ManageController.ManageMessageId.AddPhoneSuccess);
        }

        [TestMethod]
        public async Task IndexTestRemovePhoneSuccess()
        {
            await IndexTest(ManageController.ManageMessageId.RemovePhoneSuccess);
        }

        [TestMethod]
        public async Task IndexTestNull()
        {
            await IndexTest(null);
        }

        #endregion Index

        #region RemoveLogin

        private async Task RemoveLoginTest(
            Func<ControllerAutoFixture, IdentityResult> removeLoginResult,
            Func<ControllerAutoFixture, ApplicationUser> user)
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var loginProvider = fixture.Create<string>();
            var providerKey = fixture.Create<string>();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var expectedUser = user(fixture);
            var expectedRemoveLoginResult = removeLoginResult(fixture);
            userManagerMock.Setup(it => it.RemoveLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
                           .ReturnsAsync(expectedRemoveLoginResult);
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(expectedUser);
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.RemoveLogin(loginProvider, providerKey) as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "ManageLogins");
            if (expectedRemoveLoginResult.Succeeded)
            {
                if (expectedUser != null)
                {
                    signInManagerMock.Verify(it => it.SignInAsync(expectedUser, false, false));
                }
                result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.RemoveLoginSuccess);
            }
            else
            {
                result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.Error);
            }
        }

        [TestMethod]
        public async Task RemoveLoginTestSuccess()
        {
            await RemoveLoginTest(fixture => IdentityResult.Success, fixture => fixture.Create<ApplicationUser>());
        }

        [TestMethod]
        public async Task RemoveLoginTestFailed()
        {
            await RemoveLoginTest(
                fixture => IdentityResult.Failed(fixture.Create<string>()),
                fixture => fixture.Create<ApplicationUser>());
        }

        #endregion RemoveLogin

        #region AddPhoneNumber

        [TestMethod]
        public void AddPhoneNumberTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<ManageController>();

            // act
            var result = target.AddPhoneNumber() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.ViewData.Should().BeEmpty();
        }

        #endregion AddPhoneNumber

        #region AddPhoneNumberPost

        private async Task AddPhoneNumberTestPost(bool isValidState)
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var expectedCode = fixture.Create<string>();
            var model = fixture.Build<AddPhoneNumberViewModel>().Create();
            var smsServiceMock = fixture.Freeze<Mock<IIdentityMessageService>>();
            userManagerMock.Setup(it => it.GenerateChangePhoneNumberTokenAsync(It.IsAny<string>(), model.Number))
                           .ReturnsAsync(expectedCode);
            userManagerMock.SetupGet(it => it.SmsService).Returns(smsServiceMock.Object);
            var target = fixture.Create<ManageController>();
            if (isValidState)
            {
                // act
                var result = await target.AddPhoneNumber(model) as RedirectToRouteResult;

                // assert
                smsServiceMock.Verify(
                    it =>
                    it.SendAsync(
                        It.Is<IdentityMessage>(
                            msg =>
                            msg.Body == "Your security code is: " + expectedCode && msg.Destination == model.Number)));

                result.Should().NotBeNull();
                result.RouteValues.Should().Contain("action", "VerifyPhoneNumber");
                result.RouteValues.Should().Contain("PhoneNumber", model.Number);
            }
            else
            {
                target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

                // act
                var result = await target.AddPhoneNumber(model) as ViewResult;

                // assert
                result.Should().NotBeNull();
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.Should().BeSameAs(model);
            }
        }

        [TestMethod]
        public async Task AddPhoneNumberPostTest()
        {
            await this.AddPhoneNumberTestPost(true);
        }

        [TestMethod]
        public async Task AddPhoneNumberPostTestIsModelStateInvalid()
        {
            await this.AddPhoneNumberTestPost(false);
        }

        #endregion AddPhoneNumberTestPost

        #region EnableTwoFactorAuthentication

        [TestMethod]
        public async Task EnableTwoFactorAuthenticationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var user = fixture.Create<ApplicationUser>();
            var userMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.EnableTwoFactorAuthentication() as RedirectToRouteResult;

            // assert
            signInManagerMock.Verify(it => it.SignInAsync(user, false, false));
            userMock.Verify(it => it.SetTwoFactorEnabledAsync(It.IsAny<string>(), true));
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "Index");
            result.RouteValues.Should().Contain("controller", "Manage");
        }

        #endregion EnableTwoFactorAuthentication

        #region DisableTwoFactorAuthentication

        [TestMethod]
        public async Task DisableTwoFactorAuthenticationTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var user = fixture.Create<ApplicationUser>();
            var userMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.DisableTwoFactorAuthentication() as RedirectToRouteResult;

            // assert
            signInManagerMock.Verify(it => it.SignInAsync(user, false, false));
            userMock.Verify(it => it.SetTwoFactorEnabledAsync(It.IsAny<string>(), false));
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "Index");
            result.RouteValues.Should().Contain("controller", "Manage");
        }

        #endregion DisableTwoFactorAuthentication

        #region VerifyPhoneNumber

        private async Task VerifyPhoneNumberTest(Func<ControllerAutoFixture, string> getPhoneNumber)
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var phoneNumber = getPhoneNumber(fixture);
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.VerifyPhoneNumber(phoneNumber) as ViewResult;

            // assert
            result.Should().NotBeNull();
            if (phoneNumber == null)
            {
                result.ViewName.Should().Be("Error");
                result.ViewData.Should().BeEmpty();
            }
            else
            {
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.ShouldBeEquivalentTo(
                    new VerifyPhoneNumberViewModel
                        {
                            PhoneNumber = phoneNumber
                        });
            }
        }

        [TestMethod]
        public async Task VerifyPhoneNumberTest()
        {
            await VerifyPhoneNumberTest(fixture => fixture.Create<string>());
        }

        [TestMethod]
        public async Task VerifyPhoneNumberTestPhoneNumberNull()
        {
            await VerifyPhoneNumberTest(fixture => null);
        }

        #endregion VerifyPhoneNumber

        #region VerifyPhoneNumberPost

        private async Task VerifyPhoneNumberPostTest(
            bool isModelValid,
            Func<ControllerAutoFixture, IdentityResult> getResult)
        {
            // arrange
            var fixture = new ControllerAutoFixture();

            var model = fixture.Build<VerifyPhoneNumberViewModel>().Create();
            var identityResult = getResult(fixture);
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.ChangePhoneNumberAsync(It.IsAny<string>(), model.PhoneNumber, model.Code))
                           .ReturnsAsync(identityResult);

            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var user = fixture.Create<ApplicationUser>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var target = fixture.Create<ManageController>();
            if (isModelValid)
            {
                if (identityResult.Succeeded)
                {
                    // act
                    var result = await target.VerifyPhoneNumber(model) as RedirectToRouteResult;

                    // assert
                    signInManagerMock.Verify(it=>it.SignInAsync(user, false, false));
                    result.Should().NotBeNull();
                    result.RouteValues.Should().Contain("action", "Index");
                    result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.AddPhoneSuccess);
                }
                else
                {
                    // act
                    var result = await target.VerifyPhoneNumber(model) as ViewResult;

                    // assert
                    target.ModelState.IsValid.Should().BeFalse();
                    target.ModelState[ string.Empty ].Errors.Should()
                                                     .Contain(err => err.ErrorMessage == "Failed to verify phone");
                    result.Should().NotBeNull();
                    result.ViewName.Should().BeNullOrEmpty();
                    result.Model.Should().BeSameAs(model);
                }
            }
            else
            {
                target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

                // act
                var result = await target.VerifyPhoneNumber(model) as ViewResult;

                // assert
                result.Should().NotBeNull();
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.Should().BeSameAs(model);
            }
        }

        [TestMethod]
        public async Task VerifyPhoneNumberPostModelStateInvalid()
        {
            await VerifyPhoneNumberPostTest(false,fixture=>IdentityResult.Success);
        }

        [TestMethod]
        public async Task VerifyPhoneNumberPost()
        {
            await VerifyPhoneNumberPostTest(true, fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task VerifyPhoneNumberPostFailed()
        {
            await VerifyPhoneNumberPostTest(true, fixture => IdentityResult.Failed(fixture.Create<string>()));
        }

        #endregion VerifyPhoneNumberPost

        #region RemovePhoneNumber

        private async Task RemovePhoneNumberTest(Func<ControllerAutoFixture, IdentityResult> getResult)
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var identityResult = getResult(fixture);
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.SetPhoneNumberAsync(It.IsAny<string>(), null)).ReturnsAsync(identityResult);
            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var user = fixture.Create<ApplicationUser>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.RemovePhoneNumber() as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "Index");
            if (identityResult.Succeeded)
            {
                signInManagerMock.Verify(it => it.SignInAsync(user, false, false));
                result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.RemovePhoneSuccess);
            }
            else
            {
                result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.Error);
            }
        }

        [TestMethod]
        public async Task RemovePhoneNumberTestFailed()
        {
            await RemovePhoneNumberTest(fixture => IdentityResult.Failed(fixture.Create<string>()));
        }

        [TestMethod]
        public async Task RemovePhoneNumberTest()
        {
            await RemovePhoneNumberTest(fixture => IdentityResult.Success);
        }

        #endregion RemovePhoneNumber

        #region ChangePassword

        [TestMethod]
        public void ChangePasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<ManageController>();

            // act
            var result = target.ChangePassword() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.ViewData.Should().BeEmpty();
        }

        #endregion ChangePassword

        #region ChangePasswordPost

        private async Task ChangePasswordPostTest(
            bool isModelValid,
            Func<ControllerAutoFixture, IdentityResult> getResult)
        {
            // arrange
            var fixture = new ControllerAutoFixture();

            var model = fixture.Build<ChangePasswordViewModel>().Create();
            var identityResult = getResult(fixture);
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.ChangePasswordAsync(It.IsAny<string>(), model.OldPassword, model.NewPassword))
                           .ReturnsAsync(identityResult);

            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var user = fixture.Create<ApplicationUser>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var target = fixture.Create<ManageController>();
            if (isModelValid)
            {
                if (identityResult.Succeeded)
                {
                    // act
                    var result = await target.ChangePassword(model) as RedirectToRouteResult;

                    // assert
                    signInManagerMock.Verify(it => it.SignInAsync(user, false, false));
                    result.Should().NotBeNull();
                    result.RouteValues.Should().Contain("action", "Index");
                    result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.ChangePasswordSuccess);
                }
                else
                {
                    // act
                    var result = await target.ChangePassword(model) as ViewResult;

                    // assert
                    target.ModelState.IsValid.Should().BeFalse();
                    target.ModelState[ string.Empty ].Errors.Select(it => it.ErrorMessage)
                                                     .Should()
                                                     .BeEquivalentTo(identityResult.Errors);
                    result.Should().NotBeNull();
                    result.ViewName.Should().BeNullOrEmpty();
                    result.Model.Should().BeSameAs(model);
                }
            }
            else
            {
                target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

                // act
                var result = await target.ChangePassword(model) as ViewResult;

                // assert
                result.Should().NotBeNull();
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.Should().BeSameAs(model);
            }
        }

        [TestMethod]
        public async Task ChangePasswordPostModelStateInvalid()
        {
            await ChangePasswordPostTest(false, fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task ChangePasswordPost()
        {
            await ChangePasswordPostTest(true, fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task ChangePasswordPostFailed()
        {
            await ChangePasswordPostTest(true, fixture => IdentityResult.Failed(fixture.Create<string>()));
        }

        #endregion ChangePasswordPost

        #region SetPassword

        [TestMethod]
        public void SetPasswordTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<ManageController>();

            // act
            var result = target.SetPassword() as ViewResult;

            // assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.ViewData.Should().BeEmpty();
        }

        #endregion SetPassword

        #region SetPasswordPost

        private async Task SetPasswordPostTest(
            bool isModelValid,
            Func<ControllerAutoFixture, IdentityResult> getResult)
        {
            // arrange
            var fixture = new ControllerAutoFixture();

            var model = fixture.Build<SetPasswordViewModel>().Create();
            var identityResult = getResult(fixture);
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.AddPasswordAsync(It.IsAny<string>(), model.NewPassword))
                           .ReturnsAsync(identityResult);

            var signInManagerMock = fixture.Freeze<Mock<IApplicationSignInManager>>();
            var user = fixture.Create<ApplicationUser>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var target = fixture.Create<ManageController>();
            if (isModelValid)
            {
                if (identityResult.Succeeded)
                {
                    // act
                    var result = await target.SetPassword(model) as RedirectToRouteResult;

                    // assert
                    signInManagerMock.Verify(it => it.SignInAsync(user, false, false));
                    result.Should().NotBeNull();
                    result.RouteValues.Should().Contain("action", "Index");
                    result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.SetPasswordSuccess);
                }
                else
                {
                    // act
                    var result = await target.SetPassword(model) as ViewResult;

                    // assert
                    target.ModelState.IsValid.Should().BeFalse();
                    target.ModelState[string.Empty].Errors.Select(it => it.ErrorMessage)
                                                     .Should()
                                                     .BeEquivalentTo(identityResult.Errors);
                    result.Should().NotBeNull();
                    result.ViewName.Should().BeNullOrEmpty();
                    result.Model.Should().BeSameAs(model);
                }
            }
            else
            {
                target.ModelState.AddModelError(string.Empty, fixture.Create<string>());

                // act
                var result = await target.SetPassword(model) as ViewResult;

                // assert
                result.Should().NotBeNull();
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.Should().BeSameAs(model);
            }
        }

        [TestMethod]
        public async Task SetPasswordPostModelStateInvalid()
        {
            await SetPasswordPostTest(false, fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task SetPasswordPost()
        {
            await SetPasswordPostTest(true, fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task SetPasswordPostFailed()
        {
            await SetPasswordPostTest(true, fixture => IdentityResult.Failed(fixture.Create<string>()));
        }

        #endregion SetPasswordPost

        #region ManageLogins

        private async Task ManageLoginsTest(ManageController.ManageMessageId? message, Func<ControllerAutoFixture, ApplicationUser> getUser)
        {
            // arrange
            var expectedStatusMessage = message == ManageController.ManageMessageId.RemoveLoginSuccess
                                             ? "The external login was removed."
                                             : message == ManageController.ManageMessageId.Error
                                                   ? "An error has occurred."
                                                   : string.Empty;
            var fixture = new ControllerAutoFixture();

            var user = getUser(fixture);

            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            userManagerMock.Setup(it => it.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var expectedCurrentLogins = fixture.CreateMany<UserLoginInfo>().ToList();
            userManagerMock.Setup(it => it.GetLoginsAsync(It.IsAny<string>())).ReturnsAsync(expectedCurrentLogins);
            var expectedOtherLogins = fixture.CreateMany<AuthenticationDescription>().ToList();
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalAuthenticationTypes()).Returns(expectedOtherLogins);
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.ManageLogins(message) as ViewResult;

            // assert
            result.Should().NotBeNull();
            if (user == null)
            {
                result.ViewName.Should().Be("Error");
            }
            else
            {
                result.ViewName.Should().BeNullOrEmpty();
                result.Model.ShouldBeEquivalentTo(new ManageLoginsViewModel
                {
                    CurrentLogins = expectedCurrentLogins,
                    OtherLogins = expectedOtherLogins
                });
            }
            result.ViewData.Should().Contain("StatusMessage", expectedStatusMessage);
        }

        [TestMethod]
        public async Task ManageLoginsTestRemoveLoginSuccess()
        {
            await this.ManageLoginsTest(ManageController.ManageMessageId.RemoveLoginSuccess, fixture=>fixture.Create<ApplicationUser>());
        }

        [TestMethod]
        public async Task ManageLoginsTestError()
        {
            await this.ManageLoginsTest(ManageController.ManageMessageId.Error, fixture => fixture.Create<ApplicationUser>());
        }
        [TestMethod]
        public async Task ManageLoginsTest()
        {
            await this.ManageLoginsTest(null, fixture => fixture.Create<ApplicationUser>());
        }
        [TestMethod]
        public async Task ManageLoginsTestUserNull()
        {
            await this.ManageLoginsTest(null, fixture => null);
        }
        #endregion ManageLogins

        #region LinkLogin

        [TestMethod]
        public void LinkLoginTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var provider = fixture.Create<string>();
            var target = fixture.Create<ManageController>();

            // act
            var result = target.LinkLogin(provider) as HttpUnauthorizedResult;

            // assert
            result.Should().NotBeNull();
        }

        #endregion LinkLogin

        #region LinkLoginCallback

        private async Task LinkLoginCallbackTest(
            Func<ControllerAutoFixture, ExternalLoginInfo> getExternalLoginInfo,
            Func<ControllerAutoFixture, IdentityResult> getResult)
        {
            // arrange
            var fixture = new ControllerAutoFixture();
            var externalLoginInfo = getExternalLoginInfo(fixture);
            var identityResult = getResult(fixture);
            var userServiceMock = fixture.Freeze<Mock<IUserService>>();
            userServiceMock.Setup(it => it.GetExternalLoginInfoAsync()).ReturnsAsync(externalLoginInfo);
            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            if (externalLoginInfo != null)
            {
                userManagerMock.Setup(it => it.AddLoginAsync(It.IsAny<string>(), externalLoginInfo.Login))
                               .ReturnsAsync(identityResult);
            }
            var target = fixture.Create<ManageController>();

            // act
            var result = await target.LinkLoginCallback() as RedirectToRouteResult;

            // assert
            result.Should().NotBeNull();
            result.RouteValues.Should().Contain("action", "ManageLogins");
            if (externalLoginInfo == null || !identityResult.Succeeded)
            {
                result.RouteValues.Should().Contain("Message", ManageController.ManageMessageId.Error);
            }
        }

        [TestMethod]
        public async Task LinkLoginCallbackTest()
        {
            await this.LinkLoginCallbackTest(
                fixture => fixture.Build<ExternalLoginInfo>().Without(it=>it.ExternalIdentity).Create(),
                fixture => IdentityResult.Success);
        }

        [TestMethod]
        public async Task LinkLoginCallbackTestFailed()
        {
            await this.LinkLoginCallbackTest(
                fixture => fixture.Build<ExternalLoginInfo>().Without(it => it.ExternalIdentity).Create(),
                fixture => IdentityResult.Failed(fixture.Create<string>()));
        }

        [TestMethod]
        public async Task LinkLoginCallbackTestLoginInfoNull()
        {
            await this.LinkLoginCallbackTest(
                fixture => null,
                fixture => IdentityResult.Success);
        }

        #endregion LinkLoginCallback

        #region Dispose

        [TestMethod]
        public void DisposeTest()
        {
            // arrange
            var fixture = new ControllerAutoFixture();

            var userManagerMock = fixture.Freeze<Mock<IApplicationUserManager>>();
            var target = fixture.Create<ManageController>();

            // act
            target.Dispose();

            // assert
            userManagerMock.Verify(it => it.Dispose());
        }

        #endregion Dispose
    }
}