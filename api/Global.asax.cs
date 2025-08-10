namespace api
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Web.Configs;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            // AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.ConnectionString = Config.Instance.Database.ConnectionString;
            ORM.Initial();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs eventArgs)
        {
            HttpResponse httpResponse = HttpContext.Current.Response;
            httpResponse.Headers.Remove("X-AspNet-Version");
            httpResponse.Headers.Remove("Server");
        }
    }
}
