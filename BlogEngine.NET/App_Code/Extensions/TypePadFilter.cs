namespace App_Code.Extensions
{
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;
    using BlogEngine.Core.Web.Extensions;

    using Joel.Net;

    /// <summary>
    /// The type pad filter.
    /// </summary>
    [Extension("TypePad anti-spam comment filter (based on AkismetFilter)", "1.0", 
        "<a href=\"http://lucsiferre.net\">By Chris Nicola</a>")]
    public class TypePadFilter : ICustomFilter
    {
        #region Constants and Fields

        /// <summary>
        /// The Akismet api.
        /// </summary>
        private static Akismet api;

        /// <summary>
        /// The fall through.
        /// </summary>
        private static bool fallThrough = true;

        /// <summary>
        /// The TypePad key.
        /// </summary>
        private static string key;

        /// <summary>
        /// The settings.
        /// </summary>
        private static ExtensionSettings settings;

        /// <summary>
        /// The TypePad site.
        /// </summary>
        private static string site;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePadFilter"/> class.
        /// </summary>
        public TypePadFilter()
        {
            this.InitSettings();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether FallThrough.
        /// </summary>
        public bool FallThrough
        {
            get
            {
                return fallThrough;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICustomFilter

        /// <summary>
        /// Check if comment is spam
        /// </summary>
        /// <param name="comment">BlogEngine comment</param>
        /// <returns>True if comment is spam</returns>
        public bool Check(Comment comment)
        {
            if (api == null)
            {
                this.Initialize();
            }

            var typePadComment = GetAkismetComment(comment);
            var isspam = api.CommentCheck(typePadComment);
            fallThrough = !isspam;
            return isspam;
        }

        /// <summary>
        /// Initializes anti-spam service
        /// </summary>
        /// <returns>
        /// True if service online and credentials validated
        /// </returns>
        public bool Initialize()
        {
            if (!ExtensionManager.ExtensionEnabled("TypePadFilter"))
            {
                return false;
            }

            site = settings.GetSingleValue("SiteURL");
            key = settings.GetSingleValue("ApiKey");
            api = new Akismet(key, site, "BlogEngine.net 1.5", "api.antispam.typepad.com");

            return api.VerifyKey();
        }

        /// <summary>
        /// Report mistakes back to service
        /// </summary>
        /// <param name="comment">BlogEngine comment</param>
        public void Report(Comment comment)
        {
            if (api == null)
            {
                this.Initialize();
            }

            var akismetComment = GetAkismetComment(comment);

            if (comment.IsApproved)
            {
                Utils.Log(string.Format("TypePad: Reporting NOT spam from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                api.SubmitHam(akismetComment);
            }
            else
            {
                Utils.Log(string.Format("TypePad: Reporting SPAM from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
                api.SubmitSpam(akismetComment);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the akismet comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>An Akismet Comment.</returns>
        private static AkismetComment GetAkismetComment(Comment comment)
        {
            var akismetComment = new AkismetComment
                {
                    Blog = settings.GetSingleValue("SiteURL"),
                    UserIp = comment.IP,
                    CommentContent = comment.Content,
                    CommentAuthor = comment.Author,
                    CommentAuthorEmail = comment.Email
                };
            if (comment.Website != null)
            {
                akismetComment.CommentAuthorUrl = comment.Website.OriginalString;
            }

            return akismetComment;
        }

        /// <summary>
        /// Inits the settings.
        /// </summary>
        private void InitSettings()
        {
            var extensionSettings = new ExtensionSettings(this) { IsScalar = true };

            extensionSettings.AddParameter("SiteURL", "Site URL");
            extensionSettings.AddParameter("ApiKey", "API Key");

            extensionSettings.AddValue("SiteURL", "http://example.com/blog");
            extensionSettings.AddValue("ApiKey", "123456789");

            settings = ExtensionManager.InitSettings("TypePadFilter", extensionSettings);
            ExtensionManager.SetStatus("TypePadFilter", false);
        }

        #endregion
    }
}