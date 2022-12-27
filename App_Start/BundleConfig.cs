using System.Web;
using System.Web.Optimization;

namespace genetrix
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/Content/js").Include(
                        "~/assets/libs/jquery/jquery.min.js",
                        "~/assets/libs/bootstrap/js/bootstrap.bundle.min.js",
                        "~/assets/libs/metismenu/metisMenu.min.js",
                        "~/assets/libs/simplebar/simplebar.min.js",
                        "~/assets/libs/node-waves/waves.min.js",
                        "~/assets/libs/waypoints/lib/jquery.waypoints.min.js",
                        "~/assets/libs/jquery.counterup/jquery.counterup.min.js",
                      "~/assets/js/app.js"
                        ));

            bundles.Add(new ScriptBundle("~/Content/tablescripts").Include(
                      "~/assets/libs/datatables.net/js/jquery.dataTables.min.js",
                      "~/assets/libs/datatables.net-bs4/js/dataTables.bootstrap4.min.js",
                      "~/assets/libs/datatables.net-buttons/js/dataTables.buttons.min.js",
                      "~/assets/libs/datatables.net-buttons-bs4/js/buttons.bootstrap4.min.js",
                      "~/assets/libs/jszip/jszip.min.js",
                      "~/assets/libs/pdfmake/build/pdfmake.min.js",
                      "~/assets/libs/pdfmake/build/vfs_fonts.js",
                      "~/assets/libs/datatables.net-buttons/js/buttons.html5.min.js",
                      "~/assets/libs/datatables.net-buttons/js/buttons.print.min.js",
                      "~/assets/libs/datatables.net-buttons/js/buttons.colVis.min.js",
                      "~/assets/libs/datatables.net-responsive/js/dataTables.responsive.min.js",
                      "~/assets/libs/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js",
                      "~/assets/js/pages/datatables.init.js"
                      ));
            
            bundles.Add(new ScriptBundle("~/Content/wizard").Include(
                      "~/assets/js/pages/form-wizard.init.js",
                      "~/assets/libs/jquery-steps/build/jquery.steps.min.js",
                      "~/assets/js/pages/form-wizard.init.js",
                      "~/assets/libs/jquery.repeater/jquery.repeater.min.js",
                      "~/assets/js/pages/form-repeater.int.js",
                      "~/assets/libs/magnific-popup/jquery.magnific-popup.min.js",
                      "~/assets/js/pages/lightbox.init.js",
                      "~/assets/js/app.js"
                      ));;

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/css/bootstrap.min.css",
                      "~/assets/css/icons.min.css",
                      "~/assets/css/app.min.css"));
        }
    }
}
