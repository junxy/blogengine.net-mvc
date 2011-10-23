// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Avatar support.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// Avatar support.
    /// </summary>
    public class Avatar
    {
        #region Constants and Fields

        /// <summary>
        ///     The avatar image.
        /// </summary>
        private const string AvatarImage = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" />";

        /// <summary>
        ///     Gets or sets the URL to the Avatar image.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        ///     Gets or sets the image tag for the Avatar image.
        /// </summary>
        public string ImageTag { get; set; }

        /// <summary>
        ///    Gets or sets a value indicating whether there is not a specific image available.
        /// </summary>
        public bool HasNoImage { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the avatar/gravatar that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="size">
        /// The image size.
        /// </param>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="website">
        /// The website URL.
        /// </param>
        /// <param name="avatarUrl">
        /// An optional avatar URL to use instead of the default.
        /// </param>
        /// <param name="description">
        /// Description used for the Alt/Title attributes.
        /// </param>
        /// <returns>
        /// The avatar/gravatar image.
        /// </returns>
        public static Avatar GetAvatar(int size, string email, Uri website, string avatarUrl, string description)
        {
            if (BlogSettings.Instance.Avatar == "none")
            {
                return new Avatar { HasNoImage = true };
            }

            string imageTag;
            Uri url;

            if (!string.IsNullOrEmpty(avatarUrl) && Uri.TryCreate(avatarUrl, UriKind.RelativeOrAbsolute, out url))
            {
                imageTag = string.Format(
                    CultureInfo.InvariantCulture, AvatarImage, url, HttpUtility.HtmlEncode(description));

                return new Avatar { Url = url, ImageTag = imageTag };
            }

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                if (website != null && website.ToString().Length > 0 && website.ToString().Contains("http://"))
                {
                    url =
                        new Uri(
                            string.Format(
                                "http://images.websnapr.com/?url={0}&amp;size=t", 
                                HttpUtility.UrlEncode(website.ToString())));

                    imageTag = string.Format(
                        CultureInfo.InvariantCulture, "<img class=\"thumb\" src=\"{0}\" alt=\"{1}\" />", url, email);

                    return new Avatar { Url = url, ImageTag = imageTag };
                }

                var themeAvatar = HttpContext.Current.Server.MapPath(string.Format("{0}themes/{1}/noavatar.jpg", Utils.RelativeWebRoot, BlogSettings.Instance.Theme));

                var uri = 
                    new Uri(
                        System.IO.File.Exists(themeAvatar) ? 
                        string.Format("{0}themes/{1}/noavatar.jpg", Utils.AbsoluteWebRoot, BlogSettings.Instance.Theme) : 
                        string.Format("{0}pics/noavatar.jpg", Utils.AbsoluteWebRoot));

                imageTag = string.Format("<img src=\"{0}\" alt=\"{1}\" />", uri, description);

                return new Avatar { Url = uri, ImageTag = imageTag, HasNoImage = true };
            }

            var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5");
            if (hash != null)
            {
                hash = hash.ToLowerInvariant();
            }

            var gravatar = string.Format("http://www.gravatar.com/avatar/{0}.jpg?s={1}&amp;d=", hash, size);

            string link;
            switch (BlogSettings.Instance.Avatar)
            {
                case "identicon":
                    link = string.Format("{0}identicon", gravatar);
                    break;

                case "wavatar":
                    link = string.Format("{0}wavatar", gravatar);
                    break;

                default:
                    link = string.Format("{0}monsterid", gravatar);
                    break;
            }

            imageTag = string.Format(
                CultureInfo.InvariantCulture, AvatarImage, link, HttpUtility.HtmlEncode(description));

            return new Avatar { Url = new Uri(link), ImageTag = imageTag };
        }

        /// <summary>
        /// Returns the avatar/gravatar image tag that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="size">
        /// The image size.
        /// </param>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="website">
        /// The website URL.
        /// </param>
        /// <param name="avatarUrl">
        /// An optional avatar URL to use instead of the default.
        /// </param>
        /// <param name="description">
        /// Description used for the Alt/Title attributes.
        /// </param>
        /// <returns>
        /// The avatar/gravatar image.
        /// </returns>
        public static string GetAvatarImageTag(
            int size, string email, Uri website, string avatarUrl, string description)
        {
            var avatar = GetAvatar(size, email, website, avatarUrl, description);
            return avatar == null ? string.Empty : avatar.ImageTag;
        }

        #endregion
    }
}