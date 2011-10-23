// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.Newsletter
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using System.Web.UI.WebControls;
    using System.Xml;

    using App_Code.Controls;

    using BlogEngine.Core;

    /// <summary>
    /// The edit widget.
    /// </summary>
    public partial class Edit : WidgetEditBase
    {
        #region Constants and Fields

        /// <summary>
        ///     The xml document.
        /// </summary>
        private static XmlDocument doc;

        /// <summary>
        ///     The file name.
        /// </summary>
        private static string fileName;

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            SaveEmails();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            doc = null;
            this.BindGrid();

            this.gridEmails.RowDeleting += this.GridEmailsRowDeleting;
            this.gridEmails.RowDataBound += this.GridEmailsRowDataBound;

            base.OnInit(e);
        }

        /// <summary>
        /// Loads the emails.
        /// </summary>
        private static void LoadEmails()
        {
            if (doc != null && fileName != null)
            {
                return;
            }

            fileName = Path.Combine(BlogConfig.StorageLocation, "newsletter.xml");
            fileName = HostingEnvironment.MapPath(fileName);

            if (File.Exists(fileName))
            {
                doc = new XmlDocument();
                doc.Load(fileName);
            }
            else
            {
                doc = new XmlDocument();
                doc.LoadXml("<emails></emails>");
            }
        }

        /// <summary>
        /// Saves the emails.
        /// </summary>
        private static void SaveEmails()
        {
            using (var ms = new MemoryStream())
            using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                doc.Save(ms);
                ms.WriteTo(fs);
            }
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            LoadEmails();

            this.gridEmails.DataKeyNames = new[] { "innertext" };
            this.gridEmails.DataSource = doc.FirstChild;
            this.gridEmails.DataBind();
        }

        /// <summary>
        /// Handles the RowDataBound event of the gridEmails control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.
        /// </param>
        private void GridEmailsRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow || this.Page.IsPostBack)
            {
                return;
            }

            var delete = e.Row.Cells[0].Controls[0] as LinkButton;
            if (delete != null)
            {
                delete.OnClientClick = "return confirm('Are you sure you want to delete this e-mail?')";
            }
        }

        /// <summary>
        /// Handles the RowDeleting event of the gridEmails control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.
        /// </param>
        private void GridEmailsRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            LoadEmails();
            var emails = this.gridEmails.DataKeys[e.RowIndex];
            if (emails == null)
            {
                return;
            }

            var email = (string)emails.Value;
            var node = doc.SelectSingleNode(string.Format("emails/email[text()='{0}']", email));
            if (node == null)
            {
                return;
            }

            doc.FirstChild.RemoveChild(node);
            this.BindGrid();
        }

        #endregion
    }
}