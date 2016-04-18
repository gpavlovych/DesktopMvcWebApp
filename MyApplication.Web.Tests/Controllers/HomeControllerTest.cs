// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeControllerTest.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The home controller test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Web.Mvc;
using FluentAssertions;
using MyApplication.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Tests.Controllers
{
    /// <summary>TODO The home controller test.</summary>
    [TestClass]
    public class HomeControllerTest
    {
        /// <summary>TODO The about.</summary>
        [TestMethod]
        public void About()
        {
            // Arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<HomeController>();

            // Act
            ViewResult result = target.About() as ViewResult;

            // Assert
            result.ViewData["Message"].Should().Be("Your application description page.");
        }

        /// <summary>TODO The contact.</summary>
        [TestMethod]
        public void Contact()
        {
            // Arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<HomeController>();

            // Act
            ViewResult result = target.Contact() as ViewResult;

            // Assert
            result.ViewData["Message"].Should().Be("Your contact page.");
        }
    }
}
