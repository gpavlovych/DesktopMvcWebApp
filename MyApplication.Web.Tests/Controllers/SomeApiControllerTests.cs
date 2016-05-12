// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SomeApiControllerTests.cs" company="">
//   
// </copyright>
// <summary>
//   Some controller tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentAssertions;
using MyApplication.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Controllers.Tests
{
    /// <summary>
    /// Tests for <see cref="SomeApiController"/>
    /// </summary>
    [TestClass]
    public class SomeApiControllerTests
    {
        [TestMethod]
        public async Task GetTest()
        {
            // arrange
            var fixture = new WebApiControllerAutoFixture();
            var userRepositoryMock = fixture.Freeze<Mock<IGenericRepository<ApplicationUser>>>();
            var user =
                fixture.Build<ApplicationUser>()
                    .Create();
            userRepositoryMock.Setup(it => it.FindById(It.IsAny<string>())).Returns(user);
            var target = fixture.Create<SomeApiController>();

            // act
            var response = target.Get();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ( await response.Content.As<StringContent>().ReadAsStringAsync() ).Should().Be("Some String!!!!!!!!");
        }

        [TestMethod]
        public async Task GetTestUserNull()
        {
            // arrange
            var fixture = new WebApiControllerAutoFixture();
            var userRepositoryMock = fixture.Freeze<Mock<IGenericRepository<ApplicationUser>>>();
            userRepositoryMock.Setup(it => it.FindById(It.IsAny<string>())).Returns<ApplicationUser>(null);
            var target = fixture.Create<SomeApiController>();

            // act
            var response = target.Get();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}