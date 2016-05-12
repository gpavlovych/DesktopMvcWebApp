using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Ploeh.AutoFixture;

namespace MyApplication.Web.Tests.Controllers
{
    public class ControllerAutoFixture : TestAutoFixture
    {
        public ControllerAutoFixture()
        {
            this.Customize<ControllerContext>(c => c.Without(x => x.DisplayMode));

            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var requestContext = new Mock<RequestContext>();
            request.Setup(r => r.Params).Returns(new NameValueCollection());
            request.Setup(r => r.RequestContext).Returns(requestContext.Object);

            response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>()))
                .Returns<string>(x => x);

            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            var context = new Mock<HttpContextBase>();
            requestContext.Setup(r => r.HttpContext).Returns(context.Object);
            requestContext.Setup(r => r.RouteData).Returns(
                new RouteData()
                    {
                    
                    });

            this.UserMock = new Mock<IPrincipal>();
            this.IdentityMock = new Mock<IIdentity>();
            this.UserMock.Setup(it => it.Identity).Returns(IdentityMock.Object);
            context.Setup(it => it.User).Returns(this.UserMock.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            this.Register(() => context.Object);
            this.Register(() => context);
            this.Register(() => request);
            this.Register(() => response);
            this.Register(() => new UrlHelper(context.Object.Request.RequestContext));
        }

        public Mock<IPrincipal> UserMock { get; }

        public Mock<IIdentity> IdentityMock { get; }
    }
}