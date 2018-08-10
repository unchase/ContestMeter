using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace ContestMeter.Web.Site
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (!Directory.Exists(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions"))
            {
                Directory.CreateDirectory(HostingEnvironment.ApplicationPhysicalPath + "Documents/TasksDescriptions");
            }
        }
    }
}
