// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.Newsletter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.UI;
    using System.Xml;

    using App_Code.Controls;

    using BlogEngine.Core;
    using BlogEngine.Core.Providers;
    using System.Collections.Specialized;

    /// <summary>
    /// The widget.
    /// </summary>
    public partial class Widget : WidgetBase, ICallbackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        ///     The xml doc.
        /// </summary>
        private static XmlDocument doc;

        /// <summary>
        ///     The filename.
        /// </summary>
        private static string fileName;

        /// <summary>
        ///     The callback.
        /// </summary>
        private string callback;

        /// <summary>
        ///     Whether emails are sent for posts.
        /// </summary>
        private const bool SendEmailsForPosts = true;

        /// <summary>
        ///     Whether emails are sent for pages.
        /// </summary>
        private const bool SendEmailsForPages = false;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "Widget" /> class.
        /// </summary>
        static Widget()
        {
            Post.Saved += PublishableSaved;
            Post.Saving += PublishableSaving;
            BlogEngine.Core.Page.Saved += PublishableSaved;
            BlogEngine.Core.Page.Saving += PublishableSaving;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether IsEditable.
        /// </summary>
        public override bool IsEditable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets Name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Newsletter";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method works as a substitute for Page_Load. You should use this method for
        ///     data binding etc. instead of Page_Load.
        /// </summary>
        public override void LoadWidget()
        {
        }

        #endregion

        #region Implemented Interfaces

        #region ICallbackEventHandler

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>
        /// The result of the callback.
        /// </returns>
        public string GetCallbackResult()
        {
            return this.callback;
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">
        /// A string that represents an event argument to pass to the event handler.
        /// </param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            this.callback = eventArgument;
            this.AddEmail(eventArgument);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates the email.
        /// </summary>
        /// <param name="publishable">
        /// The publishable to mail.
        /// </param>
        /// <returns>
        /// The email.
        /// </returns>
        private static MailMessage CreateEmail(IPublishable publishable)
        {
            var mail = new MailMessage
                {
                    Subject = publishable.Title,
                    Body = FormatBodyMail(publishable), 
                    From = new MailAddress(BlogSettings.Instance.Email, BlogSettings.Instance.Name)
                };
            return mail;
        }

        /// <summary>
        /// Does the email exist.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// Whether the email exists.
        /// </returns>
        private static bool DoesEmailExist(string email)
        {
            return doc.SelectSingleNode(string.Format("emails/email[text()='{0}']", email)) != null;
        }

        /// <summary>
        /// Replace tags below in newsletter.html theme
        ///     [TITLE]
        ///     [LINK_DESCRIPTION]
        ///     [LINK]
        ///     [WebRoot]
        ///     [httpBase]
        /// </summary>
        /// <param name="publishable">
        /// The publishable to format.
        /// </param>
        /// <returns>
        /// The format body mail.
        /// </returns>
        private static string FormatBodyMail(IPublishable publishable)
        {
            var body = new StringBuilder();
            var urlbase = Path.Combine(
                Path.Combine(Utils.AbsoluteWebRoot.AbsoluteUri, "themes"), BlogSettings.Instance.Theme);
            var filePath = string.Format("~/themes/{0}/newsletter.html", BlogSettings.Instance.Theme);
            filePath = HostingEnvironment.MapPath(filePath);
            if (File.Exists(filePath))
            {
                body.Append(File.ReadAllText(filePath));
            }
            else
            {
                // if custom theme doesn't have email template
                // use email template from standard theme
                filePath = HostingEnvironment.MapPath("~/themes/Standard/newsletter.html");
                if (File.Exists(filePath))
                {
                    body.Append(File.ReadAllText(filePath));
                }
                else
                {
                    Utils.Log(
                        "When sending newsletter, newsletter.html does not exist " +
                        "in theme folder, and does not exist in the Standard theme " +
                        "folder.");
                }
            }

            body = body.Replace("[TITLE]", publishable.Title);
            body = body.Replace("[LINK]", publishable.AbsoluteLink.AbsoluteUri);
            body = body.Replace("[LINK_DESCRIPTION]", publishable.Description);
            body = body.Replace("[WebRoot]", Utils.AbsoluteWebRoot.AbsoluteUri);
            body = body.Replace("[httpBase]", urlbase);
            return body.ToString();
        }

        /// <summary>
        /// Gets the send newsletters context data.
        /// </summary>
        /// <returns>
        /// A dictionary.
        /// </returns>
        private static Dictionary<Guid, bool> GetSendNewslettersContextData()
        {
            const string SendNewsletterEmailsContextItemKey = "SendNewsletterEmails";
            Dictionary<Guid, bool> data;

            if (HttpContext.Current.Items.Contains(SendNewsletterEmailsContextItemKey))
            {
                data = HttpContext.Current.Items[SendNewsletterEmailsContextItemKey] as Dictionary<Guid, bool>;
            }
            else
            {
                data = new Dictionary<Guid, bool>();
                HttpContext.Current.Items[SendNewsletterEmailsContextItemKey] = data;
            }

            return data;
        }

        /// <summary>
        /// Gets the send send newsletter emails.
        /// </summary>
        /// <param name="publishableId">
        /// The publishableId id.
        /// </param>
        /// <returns>
        /// Whether send newsletter emails.
        /// </returns>
        private static bool GetSendSendNewsletterEmails(Guid publishableId)
        {
            var data = GetSendNewslettersContextData();

            return data.ContainsKey(publishableId) && data[publishableId];
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
        /// Handles the Saved event of the Publishable.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.
        /// </param>
        private static void PublishableSaved(object sender, SavedEventArgs e)
        {
            var publishable = (IPublishable)sender;

            if (!GetSendSendNewsletterEmails(publishable.Id))
            {
                return;
            }

            LoadEmails();
            var emails = doc.SelectNodes("emails/email");
            if (emails == null)
            {
                return;
            }

            foreach (XmlNode node in emails)
            {
                var mail = CreateEmail(publishable);
                mail.To.Add(node.InnerText);
                Utils.SendMailMessageAsync(mail);
            }
        }

        /// <summary>
        /// Handles the Saving event of the Publishable.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="BlogEngine.Core.SavedEventArgs"/> instance containing the event data.
        /// </param>
        private static void PublishableSaving(object sender, SavedEventArgs e)
        {
            // Set SendNewsletterEmails to true whenever a publishable is changing from an unpublished
            // state to a published state.  To check the published state of this publishable before
            // it was changed, it's necessary to retrieve the publishable from the datastore since the
            // publishable in memory (via Post.GetPost() or Page.GetPage()) will already have the
            // updated values about to be saved.

            var publishable = (IPublishable)sender;

            SetSendNewsletterEmails(publishable.Id, false); // default to not sending

            if (publishable is Post && !SendEmailsForPosts)
                return;
            else if (publishable is BlogEngine.Core.Page && !SendEmailsForPages)
                return;

            if (e.Action == SaveAction.Insert && publishable.IsVisibleToPublic)
            {
                SetSendNewsletterEmails(publishable.Id, true);
            }
            else if (e.Action == SaveAction.Update && publishable.IsVisibleToPublic)
            {
                var preUpdatePublishable = (IPublishable)null;

                if (publishable is Post)
                    preUpdatePublishable = (IPublishable)BlogService.SelectPost(publishable.Id);
                else
                    preUpdatePublishable = (IPublishable)BlogService.SelectPage(publishable.Id);

                if (preUpdatePublishable != null && !preUpdatePublishable.IsVisibleToPublic)
                {
                    // Note, use publishable.Id below instead of preUpdatePublishable.Id because
                    // when directly calling BlogService.SelectPage or BlogService.SelectPost,
                    // the Guid ID is not set, and so will be a random, non-matching Guid ID.

                    SetSendNewsletterEmails(publishable.Id, true);
                }
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
        /// Sets the send newsletter emails.
        /// </summary>
        /// <param name="postId">
        /// The post id.
        /// </param>
        /// <param name="send">
        /// if set to <c>true</c> [send].
        /// </param>
        private static void SetSendNewsletterEmails(Guid postId, bool send)
        {
            var data = GetSendNewslettersContextData();
            data[postId] = send;
        }

        /// <summary>
        /// Adds the email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        private void AddEmail(string email)
        {
            try
            {
                if (!Utils.IsEmailValid(email)) { return; }
                email = email.Trim();

                LoadEmails();

                if (!DoesEmailExist(email))
                {
                    XmlNode node = doc.CreateElement("email");
                    node.InnerText = email;
                    doc.FirstChild.AppendChild(node);

                    this.callback = "true";
                    SaveEmails();
                }
                else
                {
                    var emailNode = doc.SelectSingleNode(string.Format("emails/email[text()='{0}']", email));
                    if (emailNode != null)
                    {
                        doc.FirstChild.RemoveChild(emailNode);
                    }

                    this.callback = "false";
                    SaveEmails();
                }
            }
            catch
            {
                this.callback = "false";
            }
        }

        #endregion
    }
}