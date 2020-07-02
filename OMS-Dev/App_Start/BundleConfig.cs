using System.Web.Optimization;

namespace OMS_Dev
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include("~/Scripts/toastr.min.js", "~/Scripts/scripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin").Include("~/Areas/Admin/Scripts/jquery-3.5.1.min.js", "~/Areas/Admin/Scripts/bootstrap.bundle.min.js", "~/Areas/Admin/Scripts/datatables.bootstrap4.min.js", "~/Areas/Admin/Scripts/jquery.dataTables.min.js", "~/Areas/Admin/Scripts/chart.min.js", "~/Areas/Admin/Scripts/admin.js", "~/Scripts/toastr.min.js"));

            // Styles
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.min.css", "~/Content/fontawesome-free/css/all.css", "~/Content/toastr.min.css", "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/admin").Include("~/Areas/Admin/Content/styles.css", "~/Areas/Admin/Content/dataTables.bootstrap4.min.css", "~/Areas/Admin/Content/fontawesome-free/css/all.css", "~/Areas/Admin/Content/admin.css", "~/Content/toastr.min.css"));
        }
    }
}