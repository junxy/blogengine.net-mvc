namespace Account
{
    using System;
    using System.Web.Security;
    using System.Web.UI;

    /// <summary>
    /// The account change password.
    /// </summary>
    public partial class ChangePassword : Page
    {
        #region Methods

        /// <summary>
        /// Handles the Click event of the ChangePasswordPushButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {
            this.Master.SetStatus("warning", "Password was not changed");
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.hdnPassLength.Value = Membership.MinRequiredPasswordLength.ToString();
        }

        #endregion
    }
}