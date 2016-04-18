// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiControllerAutoFixture.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The web api controller auto fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using MyApplication.Web.Tests;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace MyApplication.Web.Controllers.Tests
{
    /// <summary>TODO The web api controller auto fixture.</summary>
    public class WebApiControllerAutoFixture : TestAutoFixture
    {
        /// <summary>TODO The api controller customization.</summary>
        private class ApiControllerCustomization : ICustomization
        {
            /// <summary>TODO The customize.</summary>
            /// <param name="fixture">TODO The fixture.</param>
            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new FilteringSpecimenBuilder(
                        new Postprocessor(new MethodInvoker(new ModestConstructorQuery()), new ApiControllerFiller()), 
                        new ApiControllerSpecification()));
            }

            /// <summary>TODO The api controller filler.</summary>
            private class ApiControllerFiller : ISpecimenCommand
            {
                /// <summary>TODO The execute.</summary>
                /// <param name="specimen">TODO The specimen.</param>
                /// <param name="context">TODO The context.</param>
                /// <exception cref="ArgumentNullException"></exception>
                /// <exception cref="ArgumentException"></exception>
                public void Execute(object specimen, ISpecimenContext context)
                {
                    if (specimen == null) throw new ArgumentNullException("specimen");
                    if (context == null) throw new ArgumentNullException("context");

                    var target = specimen as ApiController;
                    if (target == null) throw new ArgumentException("The specimen must be an instance of ApiController.", "specimen");

                    target.Request = (HttpRequestMessage)context.Resolve(typeof(HttpRequestMessage));
                }
            }

            /// <summary>TODO The api controller specification.</summary>
            private class ApiControllerSpecification : IRequestSpecification
            {
                /// <summary>TODO The is satisfied by.</summary>
                /// <param name="request">TODO The request.</param>
                /// <returns>The <see cref="bool"/>.</returns>
                public bool IsSatisfiedBy(object request)
                {
                    var requestType = request as Type;
                    if (requestType == null) return false;
                    return typeof(ApiController).IsAssignableFrom(requestType);
                }
            }
        }

        /// <summary>TODO The api controller conventions.</summary>
        private class ApiControllerConventions : CompositeCustomization
        {
            /// <summary>Initializes a new instance of the <see cref="ApiControllerConventions"/> class.</summary>
            internal ApiControllerConventions()
                : base(
                    new HttpRequestMessageCustomization(), 
                    new ApiControllerCustomization(), 
                    new AutoMoqCustomization())
            {
            }
        }

        /// <summary>TODO The http request message customization.</summary>
        private class HttpRequestMessageCustomization : ICustomization
        {
            /// <summary>TODO The customize.</summary>
            /// <param name="fixture">TODO The fixture.</param>
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpRequestMessage>(
                    c =>
                    c.Without(x => x.Content)
                        .Do(x => x.Properties[HttpPropertyKeys.HttpConfigurationKey] = new HttpConfiguration()));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="WebApiControllerAutoFixture"/> class.</summary>
        public WebApiControllerAutoFixture()
            : base()
        {
            this.Customize(new ApiControllerCustomization());
        }
    }
}