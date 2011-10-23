namespace BlogEngine.Core.Web.HttpModules
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Handles pretty URL's and redirects them to the permalinks.
    /// </summary>
    public class UrlRewrite : IHttpModule
    {
        #region Constants and Fields

        /// <summary>
        /// The Year Regex.
        /// </summary>
        private static readonly Regex YearRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Regex.
        /// </summary>
        private static readonly Regex YearMonthRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Day Regex.
        /// </summary>
        private static readonly Regex YearMonthDayRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/([0-3][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        /// Extracts the year and month from the requested URL and returns that as a DateTime.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="year">
        /// The year number.
        /// </param>
        /// <param name="month">
        /// The month number.
        /// </param>
        /// <param name="day">
        /// The day number.
        /// </param>
        /// <returns>
        /// Whether date extraction succeeded.
        /// </returns>
        private static bool ExtractDate(HttpContext context, out int year, out int month, out int day)
        {
            year = 0;
            month = 0;
            day = 0;

            if (!BlogSettings.Instance.TimeStampPostLinks)
            {
                return false;
            }

            var match = YearMonthDayRegex.Match(context.Request.RawUrl);
            if (match.Success)
            {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                day = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                return true;
            }

            match = YearMonthRegex.Match(context.Request.RawUrl);
            if (match.Success)
            {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts the title from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="url">
        /// The url string.
        /// </param>
        /// <returns>
        /// The extract title.
        /// </returns>
        private static string ExtractTitle(HttpContext context, string url)
        {
            url = url.ToLowerInvariant().Replace("---", "-");
            if (url.Contains(BlogConfig.FileExtension) && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
                context.Response.AppendHeader("location", url);
                context.Response.StatusCode = 301;
            }

            url = url.Substring(0, url.IndexOf(BlogConfig.FileExtension));
            var index = url.LastIndexOf("/") + 1;
            var title = url.Substring(index);
            return context.Server.HtmlEncode(title);
        }

        /// <summary>
        /// Gets the query string from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The query string.
        /// </returns>
        private static string GetQueryString(HttpContext context)
        {
            var query = context.Request.QueryString.ToString();
            return !string.IsNullOrEmpty(query) ? string.Format("&{0}", query) : string.Empty;
        }

        /// <summary>
        /// Rewrites the category.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewriteCategory(HttpContext context, string url)
        {
            var title = ExtractTitle(context, url);
            foreach (var cat in from cat in Category.Categories
                                let legalTitle = Utils.RemoveIllegalCharacters(cat.Title).ToLowerInvariant()
                                where title.Equals(legalTitle, StringComparison.OrdinalIgnoreCase)
                                select cat)
            {
                if (url.Contains("/FEED/"))
                {
                    context.RewritePath(string.Format("syndication.axd?category={0}{1}", cat.Id, GetQueryString(context)), false);
                }
                else
                {
                    context.RewritePath(
                        string.Format("{0}default.aspx?id={1}{2}", Utils.RelativeWebRoot, cat.Id, GetQueryString(context)), false);
                    break;
                }
            }
        }

        /// <summary>
        /// Rewrites the tag.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewriteTag(HttpContext context, string url)
        {
            var tag = ExtractTitle(context, url);

            if (url.Contains("/FEED/"))
            {
                context.RewritePath(string.Format("syndication.axd?tag={0}{1}", tag, GetQueryString(context)), false);
            }
            else
            {
                context.RewritePath(
                    string.Format("{0}?tag=/{1}{2}", Utils.RelativeWebRoot, tag, GetQueryString(context)), false);
            }
        }

        /// <summary>
        /// The rewrite default.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void RewriteDefault(HttpContext context)
        {
            var url = context.Request.RawUrl;
            var page = string.Format("&page={0}", context.Request.QueryString["page"]);
            if (string.IsNullOrEmpty(context.Request.QueryString["page"]))
            {
                page = null;
            }

            if (YearMonthDayRegex.IsMatch(url))
            {
                var match = YearMonthDayRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var day = match.Groups[3].Value;
                var date = string.Format("{0}-{1}-{2}", year, month, day);
                context.RewritePath(string.Format("{0}default.aspx?date={1}{2}", Utils.RelativeWebRoot, date, page), false);
            }
            else if (YearMonthRegex.IsMatch(url))
            {
                var match = YearMonthRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var path = string.Format("default.aspx?year={0}&month={1}", year, month);
                context.RewritePath(Utils.RelativeWebRoot + path + page, false);
            }
            else if (YearRegex.IsMatch(url))
            {
                var match = YearRegex.Match(url);
                var year = match.Groups[1].Value;
                var path = string.Format("default.aspx?year={0}", year);
                context.RewritePath(Utils.RelativeWebRoot + path + page, false);
            }
            else
            {
                context.RewritePath(url.Replace("Default.aspx", "default.aspx")); // fixes a casing oddity on Mono
            }
        }

        /// <summary>
        /// Rewrites the page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewritePage(HttpContext context, string url)
        {
            var slug = ExtractTitle(context, url);
            var page =
                Page.Pages.Find(
                    p => slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase));

            if (page != null)
            {
                context.RewritePath(string.Format("{0}page.aspx?id={1}{2}", Utils.RelativeWebRoot, page.Id, GetQueryString(context)), false);
            }
        }

        /// <summary>
        /// Rewrites the post.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewritePost(HttpContext context, string url)
        {
            int year, month, day;

            var haveDate = ExtractDate(context, out year, out month, out day);
            var slug = ExtractTitle(context, url);

            // Allow for Year/Month only dates in URL (in this case, day == 0), as well as Year/Month/Day dates.
            // first make sure the Year and Month match.
            // if a day is also available, make sure the Day matches.
            var post = Post.Posts.Find(
                p =>
                (!haveDate || (p.DateCreated.Year == year && p.DateCreated.Month == month)) &&
                ((!haveDate || (day == 0 || p.DateCreated.Day == day)) &&
                 slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase)));

            if (post == null)
            {
                return;
            }

            context.RewritePath(
                url.Contains("/FEED/")
                    ? string.Format("syndication.axd?post={0}{1}", post.Id, GetQueryString(context))
                    : string.Format("{0}post.aspx?id={1}{2}", Utils.RelativeWebRoot, post.Id, GetQueryString(context)),
                false);
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void ContextBeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var path = context.Request.Path.ToUpperInvariant();
            var url = context.Request.RawUrl.ToUpperInvariant();

            path = path.Replace(".ASPX.CS", string.Empty);
            url = url.Replace(".ASPX.CS", string.Empty);

            if (!path.Contains(BlogConfig.FileExtension.ToUpperInvariant()) || path.Contains("ERROR404.ASPX"))
            {
                return;
            }

            if (path == string.Format("{0}DEFAULT.ASPX", Utils.RelativeWebRoot.ToUpperInvariant()) &&
                context.Request.QueryString.Count == 0)
            {
                var front = Page.GetFrontPage();
                if (front != null)
                {
                    url = front.RelativeLink.ToUpperInvariant();
                }
            }

            if (url.Contains("/POST/"))
            {
                RewritePost(context, url);
            }
            else if (url.Contains("/CATEGORY/"))
            {
                RewriteCategory(context, url);
            }
            else if (url.Contains("/TAG/"))
            {
                RewriteTag(context, url);
            }
            else if (url.Contains("/PAGE/"))
            {
                RewritePage(context, url);
            }
            else if (url.Contains("/CALENDAR/"))
            {
                context.RewritePath(string.Format("{0}default.aspx?calendar=show", Utils.RelativeWebRoot), false);
            }
            else if (url.Contains(string.Format("/DEFAULT{0}", BlogConfig.FileExtension.ToUpperInvariant())))
            {
                RewriteDefault(context);
            }
            else if (url.Contains("/AUTHOR/"))
            {
                var author = ExtractTitle(context, url);
                context.RewritePath(
                    string.Format("{0}default{1}?name={2}{3}", Utils.RelativeWebRoot, BlogConfig.FileExtension, author, GetQueryString(context)),
                    false);
            }
            else if (path.Contains("/BLOG.ASPX"))
            {
                context.RewritePath(string.Format("{0}default.aspx?blog=true{1}", Utils.RelativeWebRoot, GetQueryString(context)));
            }
        }

        #endregion
    }
}