using System.Web;
using System.Web.Optimization;

namespace EasySell
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/vendorsjs").IncludeDirectory(
            "~/Static/vendors/js", "*.js"));
            bundles.Add(new StyleBundle("~/bundles/vendorscss").IncludeDirectory(
            "~/Static/vendors/css", "*.css"));
            

            bundles.Add(new ScriptBundle("~/bundles/customjs").IncludeDirectory(
            "~/Static/vendors", "*.js"));
            bundles.Add(new ScriptBundle("~/bundles/customcss").IncludeDirectory(
            "~/Static/vendors", "*.css"));

            bundles.Add(new ScriptBundle("~/bundles/easyselljs").IncludeDirectory(
                        "~/Static/js", "*.js"));
            BundleTable.EnableOptimizations = false;

        }
    }
}
