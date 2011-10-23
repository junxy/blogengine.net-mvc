using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlogEngine.MVC
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default_0", // 路由名称
                "{action}.aspx", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
                new[] { "BlogEngine.MVC.Controllers" } //解决名称冲突
            );

            routes.MapRoute(
                "Default_1", // 路由名称
                "{action}/{id}.aspx", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
                new[] { "BlogEngine.MVC.Controllers" }
            );

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
                new[] { "BlogEngine.MVC.Controllers" }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}