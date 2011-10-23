﻿using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace BlogEngine.MVC.Framework
{
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding))
                return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.IsChildAction)
                return;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }

        }

        public static void DisableCompression(HttpContext context)
        {
            if (context != null &&
                context.Response != null
                && context.Response.Filter != null
                && (context.Response.Filter is GZipStream || context.Response.Filter is DeflateStream))
            {
                context.Response.Filter = null;
            }
        }

    }
}