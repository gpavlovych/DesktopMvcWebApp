using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace MyApplication.Desktop.Tests
{
    public class TestAutoFixture : Fixture
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TestAutoFixture()
        {
            this.Customize(new AutoMoqCustomization());
        }
    }
}