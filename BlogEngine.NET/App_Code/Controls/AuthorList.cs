// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds an author list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.IO;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// Builds an author list.
    /// </summary>
    public class AuthorList : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The html string.
        /// </summary>
        private static string html;

        /// <summary>
        /// The show rss icon.
        /// </summary>
        private static bool showRssIcon = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="AuthorList"/> class. 
        /// </summary>
        static AuthorList()
        {
            Post.Saved += (sender, args) => html = null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether or not to show feed icons next to the category links.
        /// </summary>
        public bool ShowRssIcon
        {
            get
            {
                return showRssIcon;
            }

            set
            {
                if (showRssIcon == value)
                {
                    return;
                }

                showRssIcon = value;
                html = null;
            }
        }

        /// <summary>
        ///     Gets the rendered HTML in the private field and first
        ///     updates it when a post has been saved (new or updated).
        /// </summary>
        private string Html
        {
            get
            {
                if (html == null)
                {
                    var ul = this.BindAuthors();
                    html = Utils.RenderControl(ul);
                }

                return html;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(this.Html);
            writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loops through all users and builds the HTML
        /// presentation.
        /// </summary>
        /// <returns>The authors.</returns>
        private HtmlGenericControl BindAuthors()
        {
            if (Post.Posts.Count == 0)
            {
                var p = new HtmlGenericControl("p") { InnerHtml = labels.none };
                return p;
            }

            var ul = new HtmlGenericControl("ul") { ID = "authorlist" };

            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                var postCount = Post.GetPostsByAuthor(user.UserName).Count;
                if (postCount == 0)
                {
                    continue;
                }

                var li = new HtmlGenericControl("li");

                if (this.ShowRssIcon)
                {
                    var img = new HtmlImage
                        {
                            Src = string.Format("{0}pics/rssButton.png", Utils.RelativeWebRoot),
                            Alt = string.Format("RSS feed for {0}", user.UserName)
                        };
                    img.Attributes["class"] = "rssButton";

                    var feedAnchor = new HtmlAnchor
                        {
                            HRef =
                                string.Format("{0}syndication.axd?author={1}", Utils.RelativeWebRoot, Utils.RemoveIllegalCharacters(user.UserName))
                        };
                    feedAnchor.Attributes["rel"] = "nofollow";
                    feedAnchor.Controls.Add(img);

                    li.Controls.Add(feedAnchor);
                }

                var anc = new HtmlAnchor
                    {
                        HRef = string.Format("{0}author/{1}{2}", Utils.RelativeWebRoot, user.UserName, BlogConfig.FileExtension),
                        InnerHtml = string.Format("{0} ({1})", user.UserName, postCount),
                        Title = string.Format("Author: {0}", user.UserName)
                    };

                li.Controls.Add(anc);
                ul.Controls.Add(li);
            }

            return ul;
        }

        #endregion
    }
}