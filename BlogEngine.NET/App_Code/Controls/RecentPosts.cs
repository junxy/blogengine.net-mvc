// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Shows a chronological list of recent posts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// Shows a chronological list of recent posts.
    /// </summary>
    public class RecentPosts : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The posts.
        /// </summary>
        private static readonly List<Post> Posts = new List<Post>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RecentPosts"/> class.
        /// </summary>
        static RecentPosts()
        {
            BuildPostList();
            Post.Saved += PostSaved;
            Post.CommentAdded += (sender, args) => BuildPostList();
            Post.CommentRemoved += (sender, args) => BuildPostList();
            Post.Rated += (sender, args) => BuildPostList();
            BlogSettings.Changed += (sender, args) => BuildPostList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (this.Page.IsCallback)
            {
                return;
            }

            var html = RenderPosts();
            writer.Write(html);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the post list.
        /// </summary>
        private static void BuildPostList()
        {
            var number = Math.Min(BlogSettings.Instance.NumberOfRecentPosts, Post.Posts.Count);

            Posts.Clear();
            foreach (var post in Post.Posts.Where(post => post.IsVisibleToPublic).Take(number))
            {
                Posts.Add(post);
            }
        }

        /// <summary>
        /// Handles the Saved event of the Post control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.</param>
        private static void PostSaved(object sender, SavedEventArgs e)
        {
            if (e.Action == SaveAction.Update)
            {
                return;
            }

            BuildPostList();
        }

        /// <summary>
        /// Renders the posts.
        /// </summary>
        /// <returns>The HTML string.</returns>
        private static string RenderPosts()
        {
            if (Posts.Count == 0)
            {
                return string.Format("<p>{0}</p>", labels.none);
            }

            var sb = new StringBuilder();
            sb.Append("<ul class=\"recentPosts\" id=\"recentPosts\">");

            foreach (var post in Posts.Where(post => post.IsVisibleToPublic))
            {
                var rating = Math.Round(post.Rating, 1).ToString(CultureInfo.InvariantCulture);

                const string Link = "<li><a href=\"{0}\">{1}</a>{2}{3}</li>";
                var comments = string.Format("<span>{0}: {1}</span>", labels.comments, post.ApprovedComments.Count);
                var rate = string.Format("<span>{0}: {1} / {2}</span>", labels.rating, rating, post.Raters);

                if (!BlogSettings.Instance.DisplayCommentsOnRecentPosts || !BlogSettings.Instance.IsCommentsEnabled)
                {
                    comments = null;
                }

                if (!BlogSettings.Instance.DisplayRatingsOnRecentPosts || !BlogSettings.Instance.EnableRating)
                {
                    rate = null;
                }

                if (post.Raters == 0)
                {
                    rate = string.Format("<span>{0}</span>", labels.notRatedYet);
                }

                sb.AppendFormat(Link, post.RelativeLink, HttpUtility.HtmlEncode(post.Title), comments, rate);
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        #endregion
    }
}