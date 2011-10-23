namespace Account
{
    using System;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The login.
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RegisterHyperLink.NavigateUrl = "register.aspx?ReturnUrl=" +
                                                 HttpUtility.UrlEncode(this.Request.QueryString["ReturnUrl"]);
            this.RegisterHyperLink.Text = labels.createNow;
            ((PlaceHolder)LoginUser.FindControl("phResetPassword")).Visible = BlogSettings.Instance.EnablePasswordReset;

            if (this.Request.QueryString.ToString() == "logoff")
            {
                FormsAuthentication.SignOut();
                if (this.Request.UrlReferrer != null && this.Request.UrlReferrer != this.Request.Url)
                {
                    this.Response.Redirect(this.Request.UrlReferrer.ToString(), true);
                }
                else
                {
                    this.Response.Redirect("login.aspx");
                }

                return;
            }

            if (!this.Page.IsPostBack || Security.IsAuthenticated)
            {
                return;
            }

            this.Master.SetStatus("warning", "Login failed");
        }

        #endregion
    }
}