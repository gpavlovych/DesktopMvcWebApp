using FluentAssertions;
using MyApplication.Desktop.Services;
using MyApplication.Desktop.ViewModels;
using MyApplication.Desktop.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;

namespace MyApplication.Desktop.Tests.ViewModels
{
    [TestClass]
    public class LoginWindowViewModelTests
    {
        #region Test commands

        [TestMethod]
        public void ReloadCredentialsCommandTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            const string EXPECTED_USER_NAME = "someUser";
            const string EXPECTED_PASSWORD = "expectedPassword";
            const bool EXPECTED_CAN_EXECUTE = true;
            settingsServiceMock.SetupGet(it => it.UserName).Returns(EXPECTED_USER_NAME);
            settingsServiceMock.SetupGet(it => it.Password).Returns(EXPECTED_PASSWORD);
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(!EXPECTED_CAN_EXECUTE);
            var target = fixture.Create<LoginWindowViewModel>();

            //act
            var actualCanExecuteResult = target.ReloadCredentialsCommand.CanExecute(null);
            target.ReloadCredentialsCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().Be(EXPECTED_CAN_EXECUTE);
            target.UserName.Should().Be(EXPECTED_USER_NAME);
            target.Password.Should().Be(EXPECTED_PASSWORD);
        }

        [TestMethod]
        public void LoginCommandTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            const string EXPECTED_USER_NAME = "someUser";
            const string EXPECTED_PASSWORD = "expectedPassword";
            const bool EXPECTED_CAN_EXECUTE = true;
            var dialogWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(!EXPECTED_CAN_EXECUTE);
            var target = fixture.Create<LoginWindowViewModel>();
            target.UserName = EXPECTED_USER_NAME;
            target.Password = EXPECTED_PASSWORD;

            //act
            var actualCanExecuteResult = target.LoginCommand.CanExecute(dialogWindowMock.Object);
            target.LoginCommand.Execute(dialogWindowMock.Object);

            //assert
            actualCanExecuteResult.Should().Be(EXPECTED_CAN_EXECUTE);
            settingsServiceMock.VerifySet(it => it.UserName = EXPECTED_USER_NAME);
            settingsServiceMock.VerifySet(it => it.Password = EXPECTED_PASSWORD);
            settingsServiceMock.VerifySet(it => it.IsLoggedIn = true);
            settingsServiceMock.Verify(it => it.Save());
            dialogWindowMock.VerifySet(it => it.DialogResult = true);
        }

        [TestMethod]
        public void CancelCommandTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            fixture.Freeze<Mock<ISettingsService>>();
            var dialogWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            var target = fixture.Create<LoginWindowViewModel>();

            //act
            var actualCanExecuteResult = target.CancelCommand.CanExecute(dialogWindowMock.Object);
            target.CancelCommand.Execute(dialogWindowMock.Object);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            dialogWindowMock.VerifySet(it => it.DialogResult = false);
        }

        #endregion Test commands

        #region Test properties

        [TestMethod]
        public void UserNameTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            fixture.Freeze<Mock<ISettingsService>>();
            var target = fixture.Create<LoginWindowViewModel>();
            target.MonitorEvents();
            var expectedValue = "someUserName";

            //act
            target.UserName = expectedValue;
            var result = target.UserName;

            //assert
            result.Should().Be(expectedValue);
            target.ShouldRaisePropertyChangeFor(it => it.UserName);
        }

        [TestMethod]
        public void PasswordTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            fixture.Freeze<Mock<ISettingsService>>();
            var target = fixture.Create<LoginWindowViewModel>();
            target.MonitorEvents();
            const string EXPECTED_VALUE = "somePassword";

            //act
            target.Password = EXPECTED_VALUE;
            var result = target.Password;

            //assert
            result.Should().Be(EXPECTED_VALUE);
            target.ShouldRaisePropertyChangeFor(it => it.Password);
        }

        #endregion Test properties
    }
}