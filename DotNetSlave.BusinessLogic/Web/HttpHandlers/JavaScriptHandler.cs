namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;

    using BlogEngine.Core.Web.HttpModules;

    /// <summary>
    /// Removes whitespace in all stylesheets added to the 
    ///     header of the HTML document in site.master.
    /// </summary>
    /// <remarks>
    /// 
    /// This handler uses an external library to perform minification of scripts. 
    /// See the BlogEngine.Core.JavascriptMinifier class for more details.
    /// 
    /// </remarks>
    public class JavaScriptHandler : IHttpHandler
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom 
        ///     HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides 
        ///     references to the intrinsic server objects 
        ///     (for example, Request, Response, Session, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var path = request.QueryString["path"];

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string rawUrl = request.RawUrl.Trim();
            string cacheKey = context.Server.HtmlDecode(rawUrl);
            string script = (string)context.Cache[cacheKey];
            bool minify = ((request.QueryString["minify"] != null) || (BlogSettings.Instance.CompressWebResource && cacheKey.Contains("WebResource.axd")));


            if (String.IsNullOrEmpty(script))
            {
                if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    script = RetrieveRemoteScript(path, cacheKey, minify);
                }
                else
                {
                    script = RetrieveLocalScript(path, cacheKey, minify);
                }
            }


            if (string.IsNullOrEmpty(script))
            {
                return;
            }

            SetHeaders(script.GetHashCode(), context);
            context.Response.Write(script);

            if (BlogSettings.Instance.EnableHttpCompression)
            {
                CompressionModule.CompressResponse(context); // Compress(context);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether to hard minify output.
        /// </summary>
        /// <param name="file">The file name.</param>
        /// <returns>Whether to hard minify output.</returns>
        private static bool HardMinify(string file)
        {
            var lookfor = ConfigurationManager.AppSettings.Get("BlogEngine.HardMinify").Split(
                new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return lookfor.Any(file.Contains);
        }

        /// <summary>
        /// Call this method for any extra processing that needs to be done on a script resource before
        /// being written to the response.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="filePath"></param>
        /// <param name="shouldMinify"></param>
        /// <returns></returns>
        private static string ProcessScript(string script, string filePath, bool shouldMinify)
        {
            // The HardMinify call is for backwards compatibility. It's really not needed anymore.
            if ((shouldMinify) || HardMinify(filePath))
            {
                var min = new JavascriptMinifier();
                min.VariableMinification = VariableMinification.LocalVariablesOnly;

                return min.Minify(script);
            }
            else
            {
                return script;
            }

        }

        /// <summary>
        /// Retrieves the local script from the disk
        /// </summary>
        /// <param name="file">
        /// The file name.
        /// </param>
        /// <param name="cacheKey">The key used to insert this script into the cache.</param>
        /// <param name="minify">Whether or not the local script should be minfied</param>
        /// <returns>
        /// The retrieve local script.
        /// </returns>
        private static string RetrieveLocalScript(string file, string cacheKey, bool minify)
        {

            if (StringComparer.OrdinalIgnoreCase.Compare(Path.GetExtension(file), ".js") != 0)
            {
                throw new SecurityException("No access");
            }

            var path = HttpContext.Current.Server.MapPath(file);

            if (File.Exists(path))
            {
                string script;
                using (var reader = new StreamReader(path))
                {
                    script = reader.ReadToEnd();
                }

                script = ProcessScript(script, file, minify);
                HttpContext.Current.Cache.Insert(cacheKey, script, new CacheDependency(path));
                return script;
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves and cached the specified remote script.
        /// </summary>
        /// <param name="file">
        /// The remote URL
        /// </param>
        /// <param name="cacheKey">The key used to insert this script into the cache.</param>
        /// <param name="minify">Whether or not the remote script should be minified</param>
        /// <returns>
        /// The retrieve remote script.
        /// </returns>
        private static string RetrieveRemoteScript(string file, string cacheKey, bool minify)
        {

            Uri url;

            if (Uri.TryCreate(file, UriKind.Absolute, out url))
            {
                try
                {

                    var remoteFile = new RemoteFile(url, false);
                    string script = remoteFile.GetFileAsString();
                    script = ProcessScript(script, file, minify);
                    HttpContext.Current.Cache.Insert(cacheKey, script, null, Cache.NoAbsoluteExpiration, new TimeSpan(3, 0, 0, 0));
                    return script;
                }
                catch (SocketException)
                {
                    // The remote site is currently down. Try again next time.
                }
            }

            return String.Empty;
        }



        /// <summary>
        /// This will make the browser and server keep the output
        ///     in its cache and thereby improve performance.
        /// </summary>
        /// <param name="hash">
        /// The hash number.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void SetHeaders(int hash, HttpContext context)
        {

            var response = context.Response;

            response.ContentType = "text/javascript";

            var cache = response.Cache;

            cache.VaryByHeaders["Accept-Encoding"] = true;
            cache.SetExpires(DateTime.Now.ToUniversalTime().AddDays(7));
            cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            var etag = string.Format("\"{0}\"", hash);
            var incomingEtag = context.Request.Headers["If-None-Match"];

            cache.SetETag(etag);
            cache.SetCacheability(HttpCacheability.Public);

            if (String.Compare(incomingEtag, etag) != 0)
            {
                return;
            }

            response.Clear();
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }

        #endregion

    }
}