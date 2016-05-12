using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace MyApplication.Web.Tests
{
    public class TestAutoFixture : Fixture
    {
        public TestAutoFixture()
        {
            this.Customize(new AutoMoqCustomization());
        }
    }
}