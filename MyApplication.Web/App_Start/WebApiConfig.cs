using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    [ExcludeFromCodeCoverage]
    public class CountODataRoutingConvention : EntitySetRoutingConvention
    {
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

    [ExcludeFromCodeCoverage]
    public class CountODataPathHandler : DefaultODataPathHandler
    {
        protected override ODataPathSegment ParseAtEntityCollection(IEdmModel model, ODataPathSegment previous, IEdmType previousEdmType, string segment)
        {
            if (segment == "$count")
            {
                return new CountPathSegment();
            }

            return base.ParseAtEntityCollection(model, previous, previousEdmType, segment);
        }
    }

    [ExcludeFromCodeCoverage]
    public class CountPathSegment : ODataPathSegment
    {
        public override string SegmentKind
        {
            get
            {
                return "$count";
            }
        }

        public override IEdmType GetEdmType(IEdmType previousEdmType)
        {
            return EdmCoreModel.Instance.FindDeclaredType("Edm.Int32");
        }

        public override IEdmEntitySet GetEntitySet(IEdmEntitySet previousEntitySet)
        {
            return previousEntitySet;
        }

        public override string ToString()
        {
            return "$count";
        }
    }

    [ExcludeFromCodeCoverage]
    public static class WebApiConfig
    {
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
