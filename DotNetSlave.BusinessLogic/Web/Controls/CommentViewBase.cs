﻿namespace BlogEngine.Core.Web.Controls
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Web.UI;

    /// <summary>
    /// Inherit from this class when you are building the
    ///     commentview.ascx user control in your custom theme.
    /// </summary>
    /// <remarks>
    /// The class exposes a lot of functionality to the custom
    ///     comment control in the theme folder.
    /// </remarks>
    public class CommentViewBase : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The flag image.
        /// </summary>
        private const string FlagImage =
            "<span class=\"adr\"><img src=\"{0}pics/flags/{1}.png\" class=\"country-name flag\" title=\"{2}\" alt=\"{2}\" /></span>";

/*
        /// <summary>
        /// The link.
        /// </summary>
        private const string Link = "<a href=\"{0}{1}\" rel=\"nofollow\">{2}</a>";
*/

/*
        /// <summary>
        /// The link regex.
        /// </summary>
        private static readonly Regex LinkRegex =
            new Regex(
                "((http://|www\\.)([A-Z0-9.-]{1,})\\.[0-9A-Z?;~&#=\\-_\\./]{2,})", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
*/

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Comment.
        /// </summary>
        /// <value>The comment.</value>
        public Comment Comment { get; set; }

        /// <summary>
        ///     Gets or sets the Post from which the comment belongs.
        /// </summary>
        /// <value>The Post object.</value>
        public Post Post { get; set; }

        /// <summary>
        ///     Gets the text of the comment.
        /// </summary>
        /// <value>The comment text.</value>
        public string Text
        {
            get
            {
                var arg = new ServingEventArgs(this.Comment.Content, ServingLocation.SinglePost);
                Comment.OnServing(this.Comment, arg);
                if (arg.Cancel)
                {
                    this.Visible = false;
                }

                var body = arg.Body.Replace("\n", "<br />");
                body = body.Replace("\t", "&nbsp;&nbsp;");
                body = body.Replace("  ", "&nbsp;&nbsp;");
                return body;
            }
        }

        /// <summary>
        ///     Gets a delete link to visitors that are authenticated
        ///     using the default membership provider.
        /// </summary>
        protected string AdminLinks
        {
            get
            {
                if (Security.IsAuthorizedTo(Rights.ModerateComments))
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat(" | <a class=\"email\" href=\"mailto:{0}\">{0}</a>", this.Comment.Email);
                    sb.AppendFormat(
                        " | <a href=\"http://www.domaintools.com/go/?service=whois&amp;q={0}/\">{0}</a>", 
                        this.Comment.IP);

                    if (this.Comment.Comments.Count > 0)
                    {
                        var confirmDelete = string.Format(
                            CultureInfo.InvariantCulture, 
                            Utils.Translate("areYouSure"), 
                            Utils.Translate("delete").ToLowerInvariant(), 
                            Utils.Translate("theComment"));
                        sb.AppendFormat(
                            " | <a href=\"javascript:void(0);\" onclick=\"if (confirm('{1}?')) location.href='?deletecomment={0}'\">{2}</a>", 
                            this.Comment.Id, 
                            confirmDelete, 
                            Utils.Translate("deleteKeepReplies"));

                        var confirmRepliesDelete = string.Format(
                            CultureInfo.InvariantCulture, 
                            Utils.Translate("areYouSure"), 
                            Utils.Translate("delete").ToLowerInvariant(), 
                            Utils.Translate("theComment"));
                        sb.AppendFormat(
                            " | <a href=\"javascript:void(0);\" onclick=\"if (confirm('{1}?')) location.href='?deletecommentandchildren={0}'\">{2}</a>", 
                            this.Comment.Id, 
                            confirmRepliesDelete, 
                            Utils.Translate("deletePlusReplies"));
                    }
                    else
                    {
                        var confirmDelete = string.Format(
                            CultureInfo.InvariantCulture, 
                            Utils.Translate("areYouSure"), 
                            Utils.Translate("delete").ToLowerInvariant(), 
                            Utils.Translate("theComment"));
                        sb.AppendFormat(
                            " | <a href=\"javascript:void(0);\" onclick=\"if (confirm('{1}?')) location.href='?deletecomment={0}'\">{2}</a>", 
                            this.Comment.Id, 
                            confirmDelete, 
                            Utils.Translate("delete"));
                    }

                    if (!this.Comment.IsApproved)
                    {
                        sb.AppendFormat(
                            " | <a href=\"?approvecomment={0}\">{1}</a>", this.Comment.Id, Utils.Translate("approve"));
                    }

                    return sb.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets the flag of the country from which the comment was written.
        ///     <remarks>
        ///         If the country hasn't been resolved from the authors IP address or
        ///         the flag does not exist for that country, nothing is displayed.
        ///     </remarks>
        /// </summary>
        protected string Flag
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Comment.Country))
                {
                    // return "<img src=\"" + Utils.RelativeWebRoot + "pics/flags/" + Comment.Country + ".png\" class=\"country-name flag\" title=\"" + Comment.Country + "\" alt=\"" + Comment.Country + "\" />";
                    return string.Format(
                        FlagImage, Utils.RelativeWebRoot, this.Comment.Country, FindCountry(this.Comment.Country));
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets a link that lets a user reply to a specific comment
        /// </summary>
        protected string ReplyToLink
        {
            get
            {
                return (((BlogSettings.Instance.IsCommentsEnabled && BlogSettings.Instance.IsCommentNestingEnabled) &&
                         this.Post.HasCommentsEnabled) && this.Comment.IsApproved) &&
                       (BlogSettings.Instance.DaysCommentsAreEnabled <= 0 ||
                        this.Post.DateCreated.AddDays(BlogSettings.Instance.DaysCommentsAreEnabled) >= DateTime.Now.Date)
                           ? string.Format(
                               "<a href=\"javascript:void(0);\" class=\"reply-to-comment\" onclick=\"BlogEngine.replyToComment('{0}');\">{1}</a>", 
                               this.Comment.Id, 
                               Utils.Translate("replyToThis"))
                           : string.Empty;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the Gravatar image that matches the specified email.
        /// </summary>
        /// <param name="size">
        /// The image size.
        /// </param>
        /// <returns>
        /// The gravatar.
        /// </returns>
        protected string Gravatar(int size)
        {
            return Avatar.GetAvatarImageTag(size, this.Comment.Email, this.Comment.Website, this.Comment.Avatar, this.Comment.Author);
        }

        /// <summary>
        /// Examins the comment body for any links and turns them
        ///     automatically into one that can be clicked.
        /// </summary>
        /// <param name="body">
        /// The body string.
        /// </param>
        /// <returns>
        /// The resolve links.
        /// </returns>
        [Obsolete("Use the Text property instead. This method will be removed in a future version.")]
        protected string ResolveLinks(string body)
        {
            return this.Text;
        }

        /// <summary>
        /// Finds country.
        /// </summary>
        /// <param name="isoCode">
        /// The iso code.
        /// </param>
        /// <returns>
        /// The find country.
        /// </returns>
        private static string FindCountry(string isoCode)
        {
            foreach (var ri in
                CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.Name)).Where(
                    ri => ri.TwoLetterISORegionName.Equals(isoCode, StringComparison.OrdinalIgnoreCase)))
            {
                return ri.DisplayName;
            }

            return isoCode;
        }

        #endregion
    }
}