using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApplication.Web.Controllers;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Tests.Controllers
{
    [TestClass]
    public class SomeControllerTests
    {
        [TestMethod]
        public void IndexTest()
        {
            // Arrange
            var fixture = new ControllerAutoFixture();
            var target = fixture.Create<SomeController>();
            
            // Act
            var result = target.Index() as ViewResult;
            
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
