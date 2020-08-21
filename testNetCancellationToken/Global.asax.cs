using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace testNetCancellationToken
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Remove(typeof(CancellationToken));
            ModelBinders.Binders.Add(typeof(CancellationToken), new FixedCancellationTokenModelBinder());
        }
    }

    public class FixedCancellationTokenModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(default(CancellationToken),
                                                                         controllerContext.HttpContext.Response.ClientDisconnectedToken);

            return source.Token;
        }
    }
}