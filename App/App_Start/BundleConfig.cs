using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace App
{
    public class BundleConfig
    {
        public class AsIsBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files) => files;
        }

        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new OrderedScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/globalize.js")
                .Include("~/Scripts/jquery.unobtrusive-ajax.js")
                .Include("~/Scripts/jquery.validate.js")
                .Include("~/Scripts/jquery.validate.globalize.js")
                .Include("~/Scripts/jquery.validate-vsdoc.js")
                .Include("~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.vjquery.unobtrusive-ajaxalidation.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/home.viewmodel.js",
                "~/Scripts/app/_run.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/stuart-crud").Include(
                "~/Scripts/stuart-crud-control.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/font-awesome.css",
                 "~/Content/Site.css"));

#if Release
            BundleTable.EnableOptimizations = true;
#endif
        }

        public class OrderedScriptBundle : ScriptBundle
        {
            public OrderedScriptBundle(string virtualPath) : this(virtualPath, null)
            {
            }

            public OrderedScriptBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath)
            {
                Orderer = new AsIsBundleOrderer();
            }
        }

        public class OrderedStyleBundle : StyleBundle
        {
            public OrderedStyleBundle(string virtualPath) : this(virtualPath, null)
            {
            }

            public OrderedStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath)
            {
                Orderer = new AsIsBundleOrderer();
            }
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            Regex pattern = new Regex(@"url\s*\(\s*([""']?)([^:)]+)\1\s*\)", RegexOptions.IgnoreCase);

            response.Content = string.Empty;

            // open each of the files
            foreach (BundleFile bfile in response.Files)
            {
                var file = bfile.VirtualFile;
                using (var reader = new StreamReader(file.Open()))
                {

                    var contents = reader.ReadToEnd();

                    // apply the RegEx to the file (to change relative paths)
                    var matches = pattern.Matches(contents);

                    if (matches.Count > 0)
                    {
                        var directoryPath = VirtualPathUtility.GetDirectory(file.VirtualPath);

                        foreach (Match match in matches)
                        {
                            // this is a path that is relative to the CSS file
                            var imageRelativePath = match.Groups[2].Value;

                            // get the image virtual path
                            var imageVirtualPath = VirtualPathUtility.Combine(directoryPath, imageRelativePath);

                            // convert the image virtual path to absolute
                            var quote = match.Groups[1].Value;
                            var replace = String.Format("url({0}{1}{0})", quote, VirtualPathUtility.ToAbsolute(imageVirtualPath));
                            contents = contents.Replace(match.Groups[0].Value, replace);
                        }

                    }
                    // copy the result into the response.
                    response.Content = String.Format("{0}\r\n{1}", response.Content, contents);
                }
            }
        }
    }
    public static class BundleExtensions
    {
        public static Bundle IncludeWithRewrite(this Bundle bundle, string virtualPath)
        {
            bundle.Include(virtualPath, new CssRewriteUrlTransform());

            return bundle;
        }
    }
}
