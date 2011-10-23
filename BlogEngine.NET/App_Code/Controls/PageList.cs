// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds a page list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// Builds a page list.
    /// </summary>
    public class PageList : Control
    {
        #region Constants and Fields

        /// <summary>
        /// The html string.
        /// </summary>
        private static string html;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="PageList"/> class. 
        /// </summary>
        static PageList()
        {
            BlogEngine.Core.Page.Saved += (sender, args) =>
            {
                RefreshCachedHtml();
            };

            RefreshCachedHtml();
        }

        private static void RefreshCachedHtml()
        {
            html = string.Empty;

            if (BlogEngine.Core.Page.Pages != null)
            {
                var ul = BindPages();
                html = BlogEngine.Core.Utils.RenderControl(ul);
              
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
            writer.Write(PageList.html);
           // writer.Write(Environment.NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loops through all pages and builds the HTML
        /// presentation.
        /// </summary>
        /// <returns>A list item.</returns>
        private static HtmlGenericControl BindPages()
        {
            var ul = new HtmlGenericControl("ul") { ID = "pagelist" };
            ul.Attributes.Add("class", "pagelist");

            foreach (var page in BlogEngine.Core.Page.Pages.Where(page => page.ShowInList && page.IsVisibleToPublic))
            {
                var li = new HtmlGenericControl("li");
                var anc = new HtmlAnchor { HRef = page.RelativeLink, InnerHtml = page.Title, Title = page.Description };

                li.Controls.Add(anc);
                ul.Controls.Add(li);
            }

            return ul;
        }

        #endregion
    }
}