namespace BlogEngine.Core.Web.HttpHandlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using System.Linq;

    using BlogEngine.Core.Web.HttpModules;

    /// <summary>
    /// Removes whitespace in all stylesheets added to the 
    ///     header of the HTML document in site.master.
    /// </summary>
    public class CssHandler : IHttpHandler
    {
        #region Events

        /// <summary>
        ///     Occurs when the requested file does not exist;
        /// </summary>
        public static event EventHandler<EventArgs> BadRequest;

        /// <summary>
        ///     Occurs when a file is served;
        /// </summary>
        public static event EventHandler<EventArgs> Served;

        /// <summary>
        ///     Occurs when the requested file does not exist;
        /// </summary>
        public static event EventHandler<EventArgs> Serving;

        #endregion

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
            string fileName = (string)request.QueryString["name"];

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Replace(BlogSettings.Instance.Version(), string.Empty);

                OnServing(fileName);

                if (StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetExtension(fileName), ".css") != 0)
                {
                    throw new SecurityException("Invalid CSS file extension");
                }

                string cacheKey = request.RawUrl.Trim();
                string css = (string)context.Cache[cacheKey];

                if (String.IsNullOrEmpty(css))
                {
                    if (fileName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        css = RetrieveRemoteCss(fileName, cacheKey);
                    }
                    else
                    {
                        css = RetrieveLocalCss(fileName, cacheKey);
                    }
                }

                // Make sure css isn't empty
                if (!string.IsNullOrEmpty(css))
                {
                    // Configure response headers
                    SetHeaders(css.GetHashCode(), context);

                    context.Response.Write(css);

                    // Check if we should compress content
                    if (BlogSettings.Instance.EnableHttpCompression)
                    {
                        CompressionModule.CompressResponse(context);
                    }

                    OnServed(fileName);
                }
                else
                {
                    OnBadRequest(fileName);
                    context.Response.Status = "404 Bad Request";
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Called when [bad request].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnBadRequest(string file)
        {
            if (BadRequest != null)
            {
                BadRequest(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [served].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServed(string file)
        {
            if (Served != null)
            {
                Served(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [serving].
        /// </summary>
        /// <param name="file">The file name.</param>
        private static void OnServing(string file)
        {
            if (Serving != null)
            {
                Serving(file, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Call this method to do any post-processing on the css before its returned in the context response.
        /// </summary>
        /// <param name="css"></param>
              /// <returns></returns>
        private static string ProcessCss(string css)
        {
            if (BlogSettings.Instance.RemoveWhitespaceInStyleSheets)
            {
                css = StripWhitespace(css);
            }
            return css;
        }

        /// <summary>
        /// Retrieves the local CSS from the disk
        /// </summary>
        /// <param name="file">
        /// The file name.
        /// </param>
        /// <param name="cacheKey">
        /// The key used to insert this script into the cache.
        /// </param>
        /// <returns>
        /// The retrieve local css.
        /// </returns>
        private static string RetrieveLocalCss(string file, string cacheKey)
        {
            var path = HttpContext.Current.Server.MapPath(file);
            try
            {
                string css;
                using (var reader = new StreamReader(path))
                {
                    css = reader.ReadToEnd();
                }

                css = ProcessCss(css);
                HttpContext.Current.Cache.Insert(cacheKey, css, new CacheDependency(path));

                return css;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves and caches the specified remote CSS.
        /// </summary>
        /// <param name="file">
        /// The remote URL
        /// </param>
        /// <param name="cacheKey">
        /// The key used to insert this script into the cache.
        /// </param>
        /// <returns>
        /// The retrieve remote css.
        /// </returns>
        private static string RetrieveRemoteCss(string file, string cacheKey)
        {

            Uri url;

            if (Uri.TryCreate(file, UriKind.Absolute, out url))
            {
                try
                {
                    var remoteFile = new RemoteFile(url, false);
                    string css = remoteFile.GetFileAsString();
                    css = ProcessCss(css);

                    // Insert into cache
                    HttpContext.Current.Cache.Insert(
                        cacheKey,
                        css,
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(3, 0, 0, 0));

                    return css;
                }
                catch (SocketException)
                {
                    // The remote site is currently down. Try again next time.
                }
            }

            return string.Empty;
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
            response.ContentType = "text/css";

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

        /// <summary>
        /// Strips the whitespace from any .css file.
        /// </summary>
        /// <param name="body">
        /// The body string.
        /// </param>
        /// <returns>
        /// The strip whitespace.
        /// </returns>
        private static string StripWhitespace(string body)
        {

            body = body.Replace("  ", " ");
            body = body.Replace(Environment.NewLine, String.Empty);
            body = body.Replace("\t", string.Empty);
            body = body.Replace(" {", "{");
            body = body.Replace(" :", ":");
            body = body.Replace(": ", ":");
            body = body.Replace(", ", ",");
            body = body.Replace("; ", ";");
            body = body.Replace(";}", "}");

            // sometimes found when retrieving CSS remotely
            body = body.Replace(@"?", string.Empty);

            // body = Regex.Replace(body, @"/\*[^\*]*\*+([^/\*]*\*+)*/", "$1");
            body = Regex.Replace(
                body, @"(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,}(?=&nbsp;)|(?<=&ndsp;)\s{2,}(?=[<])", String.Empty);

            // Remove comments from CSS
            body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);

            return body;
        }

        #endregion

    }
}