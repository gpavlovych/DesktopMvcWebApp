﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsProviderTests.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The settings provider tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using FluentAssertions;
using MyApplication.Web.Properties;
using MyApplication.Web.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace MyApplication.Web.DAL.Tests
{
    /// <summary>TODO The settings provider tests.</summary>
    [TestClass]
    public class SettingsProviderTests
    {
        /// <summary>TODO The page size test.</summary>
        [TestMethod]
        public void PageSizeTest()
        {
            // arrange
            var fixture = new TestAutoFixture();
            var settings = fixture.Build<Settings>().Create();
            var target = fixture.Create<SettingsProvider>();

            // act
            var result = target.PageSize;

            // assert
            result.Should().Be(settings.PageSize);
        }
    }
}
