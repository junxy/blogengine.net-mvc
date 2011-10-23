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
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    /// <summary>
    /// Builds a category list.
    /// </summary>
    public class CategoryList : Control
    {
        #region Constants and Fields


        /// <summary>
        ///     The html string.
        /// </summary>
        private static string html;

        /// <summary>
        ///     The show rss icon.
        /// </summary>
        private static bool showRssIcon = true;

        /// <summary>
        ///     The show post count.
        /// </summary>
        private bool showPostCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "CategoryList" /> class.
        /// </summary>
        static CategoryList()
        {
            Post.Saved += (sender, args) => html = null;
            Category.Saved += (sender, args) => html = null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether ShowPostCount.
        /// </summary>
        public bool ShowPostCount
        {
            get
            {
                return this.showPostCount;
            }

            set
            {
                if (this.showPostCount == value)
                {
                    return;
                }

                this.showPostCount = value;
                html = null;
            }
        }

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
        ///     Gets Html.
        /// </summary>
        private string Html
        {
            get
            {
                if (html == null)
                {
                    var ul = this.BindCategories();
                    using (var sw = new StringWriter())
                    {
                        using (var hw = new HtmlTextWriter(sw))
                        {
                            ul.RenderControl(hw);
                            html = sw.ToString();
                        }
                    }
                }

                return html;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.
        /// </param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(this.Html);
            writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified category has posts.
        /// </summary>
        /// <param name="cat">
        /// The category.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified category has posts; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasPosts(Category cat)
        {
            return
                Post.Posts.Where(post => post.IsVisible).SelectMany(post => post.Categories).Any(
                    category => category == cat);
        }

        /// <summary>
        /// Sorts the categories.
        /// </summary>
        /// <returns>
        /// The sorted categories.
        /// </returns>
        private static SortedDictionary<string, Guid> SortCategories()
        {
            var dic = new SortedDictionary<string, Guid>();
            foreach (var cat in Category.Categories.Where(HasPosts))
            {
                dic.Add(cat.CompleteTitle(), cat.Id);
            }

            return dic;
        }

        /// <summary>
        /// Binds the categories.
        /// </summary>
        /// <returns>A category control.</returns>
        private HtmlGenericControl BindCategories()
        {
            var dic = SortCategories();

            if (dic.Keys.Count == 0)
            {
                var none = new HtmlGenericControl("p") { InnerText = "None" };
                return none;
            }

            var ul = new HtmlGenericControl("ul") { ID = "categorylist" };

            foreach (var id in dic.Values)
            {
                // Find full category
                var cat = Category.GetCategory(id);
                var key = cat.CompleteTitle();

                var li = new HtmlGenericControl("li");

                if (this.ShowRssIcon)
                {
                    var img = new HtmlImage
                        {
                            Src = string.Format("{0}pics/rssButton.png", Utils.RelativeWebRoot),
                            Alt =
                                string.Format(
                                    "{0} feed for {1}", BlogSettings.Instance.SyndicationFormat.ToUpperInvariant(), key)
                        };
                    img.Attributes["class"] = "rssButton";

                    var feedAnchor = new HtmlAnchor { HRef = cat.FeedRelativeLink };
                    feedAnchor.Attributes["rel"] = "nofollow";
                    feedAnchor.Controls.Add(img);

                    li.Controls.Add(feedAnchor);
                }

                var posts = Post.GetPostsByCategory(dic[key]).FindAll(p => p.IsVisible).Count;

                var postCount = string.Format(" ({0})", posts);
                if (!this.ShowPostCount)
                {
                    postCount = null;
                }

                var anc = new HtmlAnchor
                    {
                        HRef = cat.RelativeLink,
                        InnerHtml = HttpUtility.HtmlEncode(key) + postCount,
                        Title = string.Format("Category: {0}", key)
                    };

                li.Controls.Add(anc);
                ul.Controls.Add(li);
            }

            return ul;
        }

        #endregion
    }
}