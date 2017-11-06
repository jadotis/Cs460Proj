using System.Web;
using System.Web.Optimization;

namespace PhotoApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-3.2.1.min.js",
                        "~/Scripts/angular.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/angular-animate.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-grid.min.css",
                      "~/Content/bootstrap-reboot.min.css",
                      "~/animations.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/Scripts/AngularControllers").Include(
                "~/Scripts/AngularControllers/config.js",
                "~/Scripts/AngularControllers/Login.js",
                "~/Scripts/AngularControllers/Me.js",
                "~/Scripts/AngularControllers/Friends.js",
                "~/Scripts/AngularControllers/Stream.js",
                "~/Scripts/AngularControllers/Home.js",
                "~/Scripts/AngularControllers/Search.js"
                ));

        }
    }
}
