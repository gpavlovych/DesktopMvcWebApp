
using System;
using FluentAssertions;
using MyApplication.Desktop.Services;
using MyApplication.Desktop.Tests;
using MyApplication.Desktop.Views;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Ploeh.AutoFixture;

namespace MyApplication.Desktop.ViewModels.Tests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        #region Test commands

        #region LoadCommand

        [TestMethod]
        public void LoadCommandTestShowLoginShownAndSuccess()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            loginWindowMock.Setup(it => it.ShowDialog()).Returns(
                () =>
                    {
                        settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);
                        return true;
                    });
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.LoadCommand.CanExecute(null);
            target.LoadCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            loginWindowMock.Verify(it => it.ShowDialog());
        }

        [TestMethod]
        public void LoadCommandTestShowLoginShownAndUserCanceled()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            var count = 0;
            loginWindowMock.Setup(it => it.ShowDialog()).Returns(
                () =>
                    {
                        count++;
                        settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(count >= 2);
                        return true;
                    });
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var target = fixture.Create<MainWindowViewModel>();
            settingsServiceMock.SetupSet(it => it.IsLoggedIn = false)
                               .Callback(() => settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false));

            //act
            var actualCanExecuteResult = target.LoadCommand.CanExecute(null);
            target.LoadCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            loginWindowMock.Verify(it => it.ShowDialog(), Times.Exactly(1));
        }

        [TestMethod]
        public void LoadCommandTestAlreadyLoggedIn()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            loginWindowMock.Setup(it => it.ShowDialog()).Returns(
                () =>
                    {
                        settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);
                        return true;
                    });
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.LoadCommand.CanExecute(null);
            target.LoadCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            loginWindowMock.Verify(it => it.ShowDialog(), Times.Never);
        }

        #endregion LoadCommand

        #region LoginCommand

        [TestMethod]
        public void LoginCommandTestShowLoginShownAndSuccess()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            loginWindowMock.Setup(it => it.ShowDialog()).Returns(
                () =>
                    {
                        settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);
                        return true;
                    });
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.LoginCommand.CanExecute(null);
            target.LoginCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            loginWindowMock.Verify(it => it.ShowDialog());
        }

        [TestMethod]
        public void LoginCommandTestShowLoginShownAndUserCanceled()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            var count = 0;
            loginWindowMock.Setup(it => it.ShowDialog()).Returns(
                () =>
                    {
                        count++;
                        settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(count >= 2);
                        return true;
                    });
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var target = fixture.Create<MainWindowViewModel>();
            settingsServiceMock.SetupSet(it => it.IsLoggedIn = false)
                               .Callback(() => settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false));

            //act
            var actualCanExecuteResult = target.LoginCommand.CanExecute(null);
            target.LoginCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            loginWindowMock.Verify(it => it.ShowDialog(), Times.Exactly(1));
        }

        #endregion LoginCommand

        #region SearchCommand

        [TestMethod]
        public void SearchCommandTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var searchServiceMock = fixture.Freeze<Mock<ISearchService>>();
            var expectedSomeFoundContent = fixture.Create<string>("someContent");
            searchServiceMock.Setup(it => it.CallSomeApi()).ReturnsAsync(expectedSomeFoundContent);
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);

            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.SearchCommand.CanExecute(null);
            target.SearchCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            target.SomeFoundContent.Should().Be(expectedSomeFoundContent);
        }

        [TestMethod]
        public void SearchCommandTestException()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var searchServiceMock = fixture.Freeze<Mock<ISearchService>>();
            searchServiceMock.Setup(it => it.CallSomeApi()).Throws<Exception>();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);

            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.SearchCommand.CanExecute(null);
            target.SearchCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            settingsServiceMock.SetupSet(it => it.IsLoggedIn = false);
        }

        [TestMethod]
        public void SearchCommandTestCanNotBeExecutedIsNotLoggedIn()
        {
            //arrange
            var fixture = new TestAutoFixture();

            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);

            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.SearchCommand.CanExecute(null);

            //assert
            actualCanExecuteResult.Should().BeFalse();
        }

        #endregion SearchCommand

        #region LogOffCommand

        [TestMethod]
        public void LogOffCommandTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var loginWindowMock = fixture.Freeze<Mock<ILoginWindow>>();
            var unityMock = fixture.Freeze<Mock<IUnityContainer>>();
            unityMock.Setup(it => it.Resolve(typeof(ILoginWindow), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                     .Returns(loginWindowMock.Object);
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(true);
            settingsServiceMock.SetupSet(it => it.IsLoggedIn = false)
                               .Callback(() => settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false));
            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.LogOffCommand.CanExecute(null);
            target.LogOffCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeTrue();
            settingsServiceMock.VerifySet(it => it.IsLoggedIn = false);
            settingsServiceMock.Verify(it => it.Save());
            loginWindowMock.Verify(it => it.ShowDialog());
        }

        [TestMethod]
        public void LogOffCommandTestCanNotExecute()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settingsServiceMock = fixture.Freeze<Mock<ISettingsService>>();
            settingsServiceMock.SetupGet(it => it.IsLoggedIn).Returns(false);
            var target = fixture.Create<MainWindowViewModel>();

            //act
            var actualCanExecuteResult = target.LogOffCommand.CanExecute(null);
            target.LogOffCommand.Execute(null);

            //assert
            actualCanExecuteResult.Should().BeFalse();
        }

        #endregion LogOffCommand

        #endregion Test commands

        #region Test properties

        #endregion Test properties
    }
}