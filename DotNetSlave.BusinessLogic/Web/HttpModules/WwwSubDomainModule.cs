﻿namespace BlogEngine.Core.Web.HttpModules
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Removes or adds the www subdomain from all requests
    ///     and makes a permanent redirection to the new location.
    /// </summary>
    public class WwwSubDomainModule : IHttpModule
    {
        #region Constants and Fields

        /// <summary>
        /// The link regex.
        /// </summary>
        private static readonly Regex LinkRegex = new Regex(
            "(http|https)://www\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Adds the www subdomain to the request and redirects.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void AddWww(HttpContext context)
        {
            var url = context.Request.Url.ToString().Replace("://", "://www.");
            PermanentRedirect(url, context);
        }

        /// <summary>
        /// Sends permanent redirection headers (301)
        /// </summary>
        /// <param name="url">
        /// The url to redirect to.
        /// </param>
        /// <param name="context">
        /// The HTTP context.
        /// </param>
        private static void PermanentRedirect(string url, HttpContext context)
        {
            if (url.EndsWith("default.aspx", StringComparison.OrdinalIgnoreCase))
            {
                url = url.ToLowerInvariant().Replace("default.aspx", string.Empty);
            }

            context.Response.Clear();
            context.Response.StatusCode = 301;
            context.Response.AppendHeader("location", url);
            context.Response.End();
        }

        /// <summary>
        /// Removes the www subdomain from the request and redirects.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void RemoveWww(HttpContext context)
        {
            var url = context.Request.Url.ToString();
            if (!LinkRegex.IsMatch(url))
            {
                return;
            }

            url = LinkRegex.Replace(url, "$1://");
            PermanentRedirect(url, context);
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void ContextBeginRequest(object sender, EventArgs e)
        {
            if (BlogSettings.Instance.HandleWwwSubdomain == "ignore" ||
                string.IsNullOrEmpty(BlogSettings.Instance.HandleWwwSubdomain))
            {
                return;
            }

            var context = ((HttpApplication)sender).Context;
            if (context.Request.HttpMethod != "GET" || context.Request.RawUrl.Contains("/admin/") ||
                context.Request.IsLocal)
            {
                return;
            }

            if (context.Request.PhysicalPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                var url = context.Request.Url.ToString();

                if (url.Contains("://www.") && BlogSettings.Instance.HandleWwwSubdomain == "remove")
                {
                    RemoveWww(context);
                }

                if (!url.Contains("://www.") && BlogSettings.Instance.HandleWwwSubdomain == "add")
                {
                    AddWww(context);
                }
            }
        }

        #endregion
    }
}