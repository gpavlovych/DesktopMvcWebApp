// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The count o data routing convention.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using MyApplication.Web.Models;

namespace MyApplication.Web
{
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.OData.Routing;
    using System.Web.Http.OData.Routing.Conventions;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    /// <summary>TODO The count o data routing convention.</summary>
    public class CountODataRoutingConvention : EntitySetRoutingConvention
    {
        /// <summary>TODO The select action.</summary>
        /// <param name="odataPath">TODO The odata path.</param>
        /// <param name="controllerContext">TODO The controller context.</param>
        /// <param name="actionMap">TODO The action map.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (controllerContext.Request.Method == HttpMethod.Get && odataPath.PathTemplate == "~/entityset/$count")
            {
                if (actionMap.Contains("GetCount"))
                {
                    return "GetCount";
                }
            }

            return null;
        }
    }

    /// <summary>TODO The count o data path handler.</summary>
    public class CountODataPathHandler : DefaultODataPathHandler
    {
        /// <summary>TODO The parse at entity collection.</summary>
        /// <param name="model">TODO The model.</param>
        /// <param name="previous">TODO The previous.</param>
        /// <param name="previousEdmType">TODO The previous edm type.</param>
        /// <param name="segment">TODO The segment.</param>
        /// <returns>The <see cref="ODataPathSegment"/>.</returns>
        protected override ODataPathSegment ParseAtEntityCollection(IEdmModel model, ODataPathSegment previous, IEdmType previousEdmType, string segment)
        {
            if (segment == "$count")
            {
                return new CountPathSegment();
            }

            return base.ParseAtEntityCollection(model, previous, previousEdmType, segment);
        }
    }

    /// <summary>TODO The count path segment.</summary>
    public class CountPathSegment : ODataPathSegment
    {
        /// <summary>Gets the segment kind.</summary>
        public override string SegmentKind
        {
            get
            {
                return "$count";
            }
        }

        /// <summary>TODO The get edm type.</summary>
        /// <param name="previousEdmType">TODO The previous edm type.</param>
        /// <returns>The <see cref="IEdmType"/>.</returns>
        public override IEdmType GetEdmType(IEdmType previousEdmType)
        {
            return EdmCoreModel.Instance.FindDeclaredType("Edm.Int32");
        }

        /// <summary>TODO The get entity set.</summary>
        /// <param name="previousEntitySet">TODO The previous entity set.</param>
        /// <returns>The <see cref="IEdmEntitySet"/>.</returns>
        public override IEdmEntitySet GetEntitySet(IEdmEntitySet previousEntitySet)
        {
            return previousEntitySet;
        }

        /// <summary>TODO The to string.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return "$count";
        }
    }

    /// <summary>TODO The web api config.</summary>
    public static class WebApiConfig
    {
        /// <summary>TODO The register.</summary>
        /// <param name="config">TODO The config.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<ApplicationUser>("ApplicationUsers");

            IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
            routingConventions.Insert(0, new CountODataRoutingConvention());
            config.Routes.MapODataServiceRoute(
                "odata",
                "odata",
                builder.GetEdmModel(),
                new CountODataPathHandler(),
                routingConventions);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                });
        }
    }
}
