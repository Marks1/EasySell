using System.Web;
using System.Web.Optimization;

namespace EasySell
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/vendors/js").IncludeDirectory(
            "~/Static/vendors/js", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/vendors/customjs").IncludeDirectory(
            "~/Static/vendors", "*.js"));

            bundles.Add(new StyleBundle("~/bundles/vendors/css").IncludeDirectory(
                        "~/Static/vendors/css", "*.css"));

            bundles.Add(new ScriptBundle("~/bundles/js").IncludeDirectory(
                        "~/Static/js", "*.js"));
        }
    }
}
