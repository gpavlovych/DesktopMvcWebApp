using FluentAssertions;
using MyApplication.Desktop.Properties;
using MyApplication.Desktop.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace MyApplication.Desktop.Tests.Services
{
    [TestClass]
    public class SettingsServiceTests
    {
        [TestMethod]
        public void IsLoggedInTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settings = fixture.Build<Settings>().Create();
            fixture.Inject(settings);

            var target = fixture.Create<SettingsService>();

            //act
            var result = target.IsLoggedIn;

            //assert
            result.Should().Be(settings.IsLoggedIn);
        }


        [TestMethod]
        public void PasswordTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settings = fixture.Build<Settings>().Create();
            fixture.Inject(settings);

            var target = fixture.Create<SettingsService>();

            //act
            var result = target.Password;

            //assert
            result.Should().Be(settings.Password);
        }

        [TestMethod]
        public void UserNameTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settings = fixture.Build<Settings>().Create();
            fixture.Inject(settings);

            var target = fixture.Create<SettingsService>();

            //act
            var result = target.UserName;

            //assert
            result.Should().Be(settings.UserName);
        }

        [TestMethod]
        public void WebAppUrlTest()
        {
            //arrange
            var fixture = new TestAutoFixture();
            var settings = fixture.Build<Settings>().Create();
            fixture.Inject(settings);

            var target = fixture.Create<SettingsService>();

            //act
            var result = target.WebAppUrl;

            //assert
            result.Should().Be(settings.WebAppUrl);
        }
    }
}