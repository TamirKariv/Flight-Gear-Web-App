using System.Web.Mvc;
using System.Web.Routing;

namespace FlightGearWebApp {
    public class RouteConfig {

        /// <summary>
        /// The function consists the routes that are needed to the missions.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes) {

            // the default path
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //DisplayOneTime
            routes.MapRoute("DisplayOneTime", "display/{ip}/{port}",
               new { controller = "Map", action = "DisplayOneTime" },
               new { ip = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$" });

            //DisplayPath
            routes.MapRoute("DisplayPath", "display/{ip}/{port}/{interval}",
            defaults: new { controller = "Map", action = "DisplayPath", time = UrlParameter.Optional });

            //save
            routes.MapRoute("Save", "save/{ip}/{port}/{interval}/{duration}/{dir}",
            defaults: new { controller = "Map", action = "Save" });

            //load
            routes.MapRoute("LoadDisplay", "display/{dir}/{interval}",
            defaults: new { controller = "Map", action = "LoadDisplay", time = UrlParameter.Optional });

            //defualt
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Map", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
