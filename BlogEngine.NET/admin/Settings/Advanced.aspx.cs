namespace admin.Settings
{
    using System;
    using System.Web.Services;
    using Resources;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using App_Code;
    using Page = System.Web.UI.Page;

    public partial class Advanced : Page
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindSettings();

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = labels.settings;
            base.OnInit(e);
        }

        /// <summary>
        /// The bind settings.
        /// </summary>
        private void BindSettings()
        {
            // -----------------------------------------------------------------------
            // Bind Advanced settings
            // -----------------------------------------------------------------------

            var settings = BlogSettings.Instance;

            cbEnableCompression.Checked = settings.EnableHttpCompression;
            cbRemoveWhitespaceInStyleSheets.Checked = settings.RemoveWhitespaceInStyleSheets;
            cbCompressWebResource.Checked = settings.CompressWebResource;
            cbEnableOpenSearch.Checked = settings.EnableOpenSearch;
            cbRequireSslForMetaWeblogApi.Checked = settings.RequireSslMetaWeblogApi;
            rblWwwSubdomain.SelectedValue = settings.HandleWwwSubdomain;
            cbEnablePingBackSend.Checked = settings.EnablePingBackSend;
            cbEnablePingBackReceive.Checked = settings.EnablePingBackReceive;
            cbEnableTrackBackSend.Checked = settings.EnableTrackBackSend;
            cbEnableTrackBackReceive.Checked = settings.EnableTrackBackReceive;
            cbEnableErrorLogging.Checked = settings.EnableErrorLogging;
            cbAllowRemoteFileDownloads.Checked = settings.AllowServerToDownloadRemoteFiles;
            txtRemoteTimeout.Text = settings.RemoteFileDownloadTimeout.ToString();
            txtRemoteMaxFileSize.Text = settings.RemoteMaxFileSize.ToString();
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="enableCompression"></param>
        /// <param name="removeWhitespaceInStyleSheets"></param>
        /// <param name="compressWebResource"></param>
        /// <param name="enableOpenSearch"></param>
        /// <param name="requireSslForMetaWeblogApi"></param>
        /// <param name="wwwSubdomain"></param>
        /// <param name="enableTrackBackSend"></param>
        /// <param name="enableTrackBackReceive"></param>
        /// <param name="enablePingBackSend"></param>
        /// <param name="enablePingBackReceive"></param>
        /// <param name="enableErrorLogging"></param>
        /// <param name="allowRemoteFileDownloads"></param>
        /// <param name="remoteTimeout"></param>
        /// <param name="remoteMaxFileSize"></param>
        /// <returns></returns>
        [WebMethod]
        public static JsonResponse Save(bool enableCompression, 
			bool removeWhitespaceInStyleSheets,
            bool compressWebResource,
            bool enableOpenSearch,
            bool requireSslForMetaWeblogApi,
			string wwwSubdomain,
            bool enableTrackBackSend,
            bool enableTrackBackReceive,
            bool enablePingBackSend,
            bool enablePingBackReceive,
            bool enableErrorLogging,
            bool allowRemoteFileDownloads,
            int remoteTimeout,
            int remoteMaxFileSize)
        {
            var response = new JsonResponse { Success = false };
            var settings = BlogSettings.Instance;

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {

                // Validate values before setting any of them to the BlogSettings instance.
                // Because it's a singleton, we don't want partial data being stored to
                // it if there's any exceptions thrown prior to saving. 

                if (remoteTimeout < 0)
                {
                    throw new ArgumentOutOfRangeException("RemoteFileDownloadTimeout must be greater than or equal to 0 milliseconds.");
                }
                else if (remoteMaxFileSize < 0)
                {
                    throw new ArgumentOutOfRangeException("RemoteMaxFileSize must be greater than or equal to 0 bytes.");
                }

         

                settings.EnableHttpCompression = enableCompression;
                settings.RemoveWhitespaceInStyleSheets = removeWhitespaceInStyleSheets;
                settings.CompressWebResource = compressWebResource;
                settings.EnableOpenSearch = enableOpenSearch;
                settings.RequireSslMetaWeblogApi = requireSslForMetaWeblogApi;
                settings.HandleWwwSubdomain = wwwSubdomain;
                settings.EnableTrackBackSend = enableTrackBackSend;
                settings.EnableTrackBackReceive = enableTrackBackReceive;
                settings.EnablePingBackSend = enablePingBackSend;
                settings.EnablePingBackReceive = enablePingBackReceive;
                settings.EnableErrorLogging = enableErrorLogging;

                settings.AllowServerToDownloadRemoteFiles = allowRemoteFileDownloads;
                settings.RemoteFileDownloadTimeout = remoteTimeout;
                settings.RemoteMaxFileSize = remoteMaxFileSize;

                settings.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.Advanced.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }
    }
}