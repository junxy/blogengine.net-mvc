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
    public class ResourceHandler : IHttpHandler
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
            var lang = request.QueryString["lang"];

            if (string.IsNullOrEmpty(lang))
            {
                // Use the current Language if the lang query isn't set.
                lang = BlogSettings.Instance.Language;
            }

            lang = lang.ToLowerInvariant();

            string cacheKey = "resource.axd - " + lang;
            string script = (string)context.Cache[cacheKey];
           
            if (String.IsNullOrEmpty(script))
            {

                System.Globalization.CultureInfo culture = null;
                try
                {
                    // This needs to be in a try-catch because there's no other
                    // way to find an invalid culture/language string.
                    culture = new System.Globalization.CultureInfo(lang);
                }
                catch (Exception)
                {
                    // set to default language otherwise.
                    culture = Utils.GetDefaultCulture();
                }
                
                Json.JsonCulture jc = new Json.JsonCulture(culture);

                // Although this handler is intended to output resource strings,
                // also outputting other non-resource variables.

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("BlogEngine.webRoot='{0}';", Utils.RelativeWebRoot);
                sb.AppendFormat("BlogEngine.i18n = {0};", jc.ToJsonString());

                script = sb.ToString();
                HttpContext.Current.Cache.Insert(cacheKey, script, null, Cache.NoAbsoluteExpiration, new TimeSpan(3, 0, 0, 0));
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
        /// Returns the script path used to load resources on a page. 
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string GetScriptPath(System.Globalization.CultureInfo cultureInfo)
        {
            return String.Format("{0}res.axd?lang={1}", Utils.AbsoluteWebRoot, cultureInfo.Name.ToLowerInvariant());
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
            cache.SetExpires(DateTime.UtcNow.AddDays(7));
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