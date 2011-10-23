// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Menu control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The Menu control.
    /// </summary>
    public partial class Menu : UserControl
    {
        #region Public Methods

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="text">
        /// The text string.
        /// </param>
        /// <param name="url">
        /// The URL string.
        /// </param>
        public void AddItem(string text, string url)
        {
            var a = new HtmlAnchor { InnerHtml = string.Format("<span>{0}</span>", text), HRef = url };

            var li = new HtmlGenericControl("li");
            li.Controls.Add(a);
            this.ulMenu.Controls.Add(li);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsCallback)
            {
                this.BindMenu();
            }
        }

        /// <summary>
        /// Gets the sub URL.
        /// </summary>
        /// <param name="url">
        /// The URL string.
        /// </param>
        /// <returns>
        /// The sub-url.
        /// </returns>
        private static string SubUrl(string url)
        {
            var i = url.LastIndexOf("/");

            return (i > 0) ? url.Substring(0, i) : string.Empty;
        }

        /// <summary>
        /// The bind menu.
        /// </summary>
        private void BindMenu()
        {
            var sitemap = SiteMap.Providers["SecuritySiteMap"];
            if (sitemap != null)
            {
                var root = sitemap.RootNode;
                if (root != null)
                {
                    foreach (
                        var adminNode in
                            root.ChildNodes.Cast<SiteMapNode>().Where(
                                adminNode => adminNode.IsAccessibleToUser(HttpContext.Current)).Where(
                                    adminNode =>
                                    this.Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/") ||
                                    (!adminNode.Url.Contains("xmanager") && !adminNode.Url.Contains("PingServices"))))
                    {

                        var a = new HtmlAnchor
                            {
                                HRef = adminNode.Url, 
                                InnerHtml =
                                    string.Format("<span>{0}</span>", Utils.Translate(adminNode.Title, adminNode.Title))
                            };

                        // "<span>" + Utils.Translate(info.Name.Replace(".aspx", string.Empty)) + "</span>";
                        if (this.Request.RawUrl.IndexOf(adminNode.Url, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            a.Attributes["class"] = "current";
                        }

                        // if "page" has its own subfolder (comments, extensions) should 
                        // select parent tab when navigating through child tabs
                        if (adminNode.Url.IndexOf("/admin/pagesxx/", StringComparison.OrdinalIgnoreCase) == -1 &&
                            SubUrl(this.Request.RawUrl) == SubUrl(adminNode.Url))
                        {
                            a.Attributes["class"] = "current";
                        }

                        var li = new HtmlGenericControl("li");
                        li.Controls.Add(a);
                        this.ulMenu.Controls.Add(li);
                    }
                }
            }

            if (!this.Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/"))
            {
                this.AddItem(
                    labels.myProfile, string.Format("{0}admin/Users/Profile.aspx?id={1}", Utils.RelativeWebRoot, HttpUtility.UrlPathEncode(Security.CurrentUser.Identity.Name)));

                this.AddItem(
                    labels.changePassword, string.Format("{0}Account/change-password.aspx", Utils.RelativeWebRoot));
            }
        }

        #endregion
    }
}