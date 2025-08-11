Imports System.Web
Imports System.Web.Optimization

Public Class BundleConfig
    ' For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                   "~/Scripts/jquery.js",
                   "~/Scripts/datatable/js/jquery.dataTables.js",
                   "~/Scripts/datatable/js/dataTables.bootstrap.js",
                   "~/Scripts/jquery.blockUI.js",
                   "~/Scripts/multiselect.js"
                   ))

        bundles.Add(New ScriptBundle("~/bundles/mainjs").Include(
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/autoNumeric.js",
                   "~/Scripts/select2.js",
                   "~/Scripts/site.js",
                   "~/Scripts/bootbox.js",
                   "~/Scripts/bootstrap-datetimepicker.js"
                   ))

        bundles.Add(New ScriptBundle("~/bundles/jqueryui").Include(
                    "~/Scripts/jquery-ui.js"
                    ))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.unobtrusive*",
                    "~/Scripts/jquery.validate*"))

        ' Use the development version of Modernizr to develop with and learn from. Then, when you're
        ' ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/fontawesome/font-awesome.css",
                    "~/Content/select2.css",
                    "~/Content/Fector.css",
                    "~/Content/bootstrap.min.css",
                    "~/Content/bootstrap-datetimepicker.css"
                    ))

        bundles.Add(New StyleBundle("~/Content/themes/base/css").Include(
                    "~/Content/themes/base/jquery.ui.css",
                    "~/Content/themes/base/jquery.ui.core.css",
                    "~/Content/themes/base/jquery.ui.resizable.css",
                    "~/Content/themes/base/jquery.ui.selectable.css",
                    "~/Content/themes/base/jquery.ui.accordion.css",
                    "~/Content/themes/base/jquery.ui.autocomplete.css",
                    "~/Content/themes/base/jquery.ui.button.css",
                    "~/Content/themes/base/jquery.ui.dialog.css",
                    "~/Content/themes/base/jquery.ui.slider.css",
                    "~/Content/themes/base/jquery.ui.tabs.css",
                    "~/Content/themes/base/jquery.ui.datepicker.css",
                    "~/Content/themes/base/jquery.ui.progressbar.css",
                    "~/Content/themes/base/jquery.ui.theme.css"))

        bundles.Add(New StyleBundle("~/AdminLTE/css").Include(
                  "~/dist/css/AdminLTE.css",
                  "~/dist/css/skins/_all-skins.css"
                  ))

        bundles.Add(New StyleBundle("~/AdminLTE/js").Include(
                 "~/plugins/jQuery/jQuery-2.1.4.min.js",
                 "~/Scripts/bootstrap.min.js",
                 "~/plugins/slimScroll/jquery.slimscroll.js",
                 "~/plugins/fastclick/fastclick.js",
                 "~/dist/js/app.js",
                 "~/dist/js/demo.js"
                 ))

        bundles.Add(New ScriptBundle("~/Morris/js").Include(
                   "~/plugins/morris/morris.js",
                   "~/plugins/raphael/raphael.js"
                   ))


        bundles.Add(New StyleBundle("~/Morris/css").Include(
                   "~/plugins/morris/morris.css"
                   ))


    End Sub
End Class