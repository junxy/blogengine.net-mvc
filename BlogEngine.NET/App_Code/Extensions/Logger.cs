namespace App_Code.Extensions
{
    using System;
    using System.IO;
    using System.Web.Hosting;

    using BlogEngine.Core;
    using BlogEngine.Core.Web.Controls;

    /// <summary>
    /// Subscribes to Log events and records the events in a file.
    /// </summary>
    [Extension("Subscribes to Log events and records the events in a file.", "1.0", "BlogEngine.NET")]
    public class Logger
    {
        #region Constants and Fields

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The file name.
        /// </summary>
        private static string fileName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            Utils.OnLog += OnLog;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <returns>The file name.</returns>
        private static string GetFileName()
        {
            if (fileName != null)
            {
                return fileName;
            }

            fileName = HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "logger.txt"));
            return fileName;
        }

        /// <summary>
        /// The event handler that is triggered every time there is a log notification.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void OnLog(object sender, EventArgs e)
        {
            if (sender == null || !(sender is string))
            {
                return;
            }

            var logMsg = (string)sender;

            if (string.IsNullOrEmpty(logMsg))
            {
                return;
            }

            var file = GetFileName();

            lock (SyncRoot)
            {
                try
                {
                    using (var fs = new FileStream(file, FileMode.Append))
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(@"*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
                        sw.WriteLine("Date: {0}", DateTime.Now);
                        sw.WriteLine("Contents Below");
                        sw.WriteLine(logMsg);
                    }
                }
                catch
                {
                    // Absorb the error.
                }
            }
        }

        #endregion
    }
}