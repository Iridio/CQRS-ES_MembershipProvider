using System.Web.Mvc;
using System.Web.Routing;

namespace Iridio.SampleWebSite
{
  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute("", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);

      AutofacBootstrapper.Bootstrap();
    }
  }
}