using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyApplication.Web.Startup))]
namespace MyApplication.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
