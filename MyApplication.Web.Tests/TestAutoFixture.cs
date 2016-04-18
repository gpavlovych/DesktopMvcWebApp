// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestAutoFixture.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The test auto fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace MyApplication.Web.Tests
{
    /// <summary>TODO The test auto fixture.</summary>
    public class TestAutoFixture : Fixture
    {
        /// <summary>Initializes a new instance of the <see cref="TestAutoFixture"/> class. 
        /// Constructor.</summary>
        public TestAutoFixture()
        {
            this.Customize(new AutoMoqCustomization());
        }
    }
}