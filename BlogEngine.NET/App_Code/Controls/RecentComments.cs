// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds a category list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// Builds a category list.
    /// </summary>
    public class RecentComments : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The comments.
        /// </summary>
        private static readonly List<Comment> Comments = new List<Comment>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RecentComments"/> class.
        /// </summary>
        static RecentComments()
        {
            BindComments();
            Post.CommentAdded += (sender, args) => BindComments();
            Post.CommentRemoved += (sender, args) => BindComments();
            Post.Saved += PostSaved;
            Comment.Approved += (sender, args) => BindComments();
            BlogSettings.Changed += (sender, args) => BindComments();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Post.Posts.Count <= 0)
            {
                return;
            }
            
            var html = RenderComments();
            writer.Write(html);
            writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the comments.
        /// </summary>
        private static void BindComments()
        {
                var comments = (from post in Post.Posts
                                where post.IsVisible
                                from comment in post.Comments
                                where comment.IsApproved
                                select comment).ToList();

                comments.Sort();
                comments.Reverse();

                RecentComments.Comments.Clear();
                foreach (var comment in
                    comments.Where(comment => comment.Email != "pingback" && comment.Email != "trackback").Take(BlogSettings.Instance.NumberOfRecentComments))
                {
                    Comments.Add(comment);
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

            BindComments();
        }

        /// <summary>
        /// Renders the comments.
        /// </summary>
        /// <returns>The HTML string.</returns>
        private static string RenderComments()
        {
            if (Comments.Count == 0)
            {
                return string.Format("<p>{0}</p>", labels.none);
            }

            using (var ul = new HtmlGenericControl("ul"))
            {
                ul.Attributes.Add("class", "recentComments");
                ul.ID = "recentComments";

                foreach (var comment in Comments.Where(comment => comment.IsApproved))
                {
                    var li = new HtmlGenericControl("li");

                    // The post title
                    var title = new HtmlAnchor { HRef = comment.Parent.RelativeLink, InnerText = comment.Parent.Title };
                    title.Attributes.Add("class", "postTitle");
                    li.Controls.Add(title);

                    // The comment count on the post
                    var count =
                        new LiteralControl(string.Format(" ({0})<br />", ((Post)comment.Parent).ApprovedComments.Count));
                    li.Controls.Add(count);

                    // The author
                    if (comment.Website != null)
                    {
                        var author = new HtmlAnchor { HRef = comment.Website.ToString(), InnerHtml = comment.Author };
                        author.Attributes.Add("rel", "nofollow");
                        li.Controls.Add(author);

                        var wrote = new LiteralControl(string.Format(" {0}: ", labels.wrote));
                        li.Controls.Add(wrote);
                    }
                    else
                    {
                        var author = new LiteralControl(string.Format("{0} {1}: ", comment.Author, labels.wrote));
                        li.Controls.Add(author);
                    }

                    // The comment body
                    var commentBody = Regex.Replace(comment.Content, @"\[(.*?)\]", string.Empty);
                    var bodyLength = Math.Min(commentBody.Length, 50);

                    commentBody = commentBody.Substring(0, bodyLength);
                    if (commentBody.Length > 0)
                    {
                        if (commentBody[commentBody.Length - 1] == '&')
                        {
                            commentBody = commentBody.Substring(0, commentBody.Length - 1);
                        }
                    }

                    commentBody += comment.Content.Length <= 50 ? " " : "� ";
                    var body = new LiteralControl(commentBody);
                    li.Controls.Add(body);

                    // The comment link
                    var link = new HtmlAnchor
                        {
                            HRef = string.Format("{0}#id_{1}", comment.Parent.RelativeLink, comment.Id), InnerHtml = string.Format("[{0}]", labels.more) 
                        };
                    link.Attributes.Add("class", "moreLink");
                    li.Controls.Add(link);

                    ul.Controls.Add(li);
                }

                using (var sw = new StringWriter())
                {
                    ul.RenderControl(new HtmlTextWriter(sw));
                    return sw.ToString();
                }
            }
        }

        #endregion
    }
}