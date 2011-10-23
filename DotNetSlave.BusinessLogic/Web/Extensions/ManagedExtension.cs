﻿namespace BlogEngine.Core.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializable object that holds extension,
    ///     extension attributes and methods
    /// </summary>
    [Serializable]
    public class ManagedExtension
    {
        #region Constants and Fields

        /// <summary>
        /// The settings.
        /// </summary>
        private List<ExtensionSettings> settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "ManagedExtension" /> class. 
        ///     Default constructor required for serialization
        /// </summary>
        public ManagedExtension()
        {
            this.Version = string.Empty;
            this.ShowSettings = true;
            this.Name = string.Empty;
            this.Enabled = true;
            this.Description = string.Empty;
            this.Author = string.Empty;
            this.AdminPage = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedExtension"/> class.
        /// </summary>
        /// <param name="name">The extension name.</param>
        /// <param name="version">The version.</param>
        /// <param name="desc">The description.</param>
        /// <param name="author">The author.</param>
        public ManagedExtension(string name, string version, string desc, string author)
        {
            this.AdminPage = string.Empty;
            this.Name = name;
            this.Version = version;
            this.Description = desc;
            this.Author = author;
            this.settings = new List<ExtensionSettings>();
            this.Enabled = true;
            this.ShowSettings = true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets Custom admin page. If defined, link to default settings
        ///     page will be replaced by the link to this page in the UI
        /// </summary>
        [XmlElement]
        public string AdminPage { get; set; }

        /// <summary>
        ///     Gets or sets Extension Author. Will show up in the settings page, can be used as a 
        ///     link to author's home page
        /// </summary>
        [XmlElement]
        public string Author { get; set; }

        /// <summary>
        ///     Gets or sets Extension Description
        /// </summary>
        [XmlElement]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether extension is enabled.
        /// </summary>
        [XmlElement]
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets Extension Name
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets Extension Priority
        /// </summary>
        [XmlElement]
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets Settings for the extension
        /// </summary>
        [XmlElement(IsNullable = true)]
        public List<ExtensionSettings> Settings
        {
            get
            {
                return this.settings;
            }
            set
            {
                this.settings = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to Show or hide settings in the admin/extensions list
        /// </summary>
        [XmlElement]
        public bool ShowSettings { get; set; }

        /// <summary>
        ///     Gets or sets Extension Version
        /// </summary>
        [XmlElement]
        public string Version { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to find out if extension has setting with this name
        /// </summary>
        /// <param name="settingName">
        /// Setting Name
        /// </param>
        /// <returns>
        /// True if settings with this name already exists
        /// </returns>
        public bool ContainsSetting(string settingName)
        {
            return this.settings.Any(xset => xset.Name == settingName);
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        /// <param name="extensionSettings">The extension settings.</param>
        public void InitializeSettings(ExtensionSettings extensionSettings)
        {
            extensionSettings.Index = this.settings.Count;
            this.SaveSettings(extensionSettings);
        }

        /// <summary>
        /// Determine if settings has been initialized with default
        ///     values (first time new extension loaded into the manager)
        /// </summary>
        /// <param name="xs">
        /// The ExtensionSettings.
        /// </param>
        /// <returns>
        /// True if initialized
        /// </returns>
        public bool Initialized(ExtensionSettings xs)
        {
            return xs != null && this.settings.Where(setItem => setItem.Name == xs.Name).Any(setItem => setItem.Parameters.Count == xs.Parameters.Count);
        }

        /// <summary>
        /// Method to cache and serialize settings object
        /// </summary>
        /// <param name="extensionSettings">The extension settings.</param>
        public void SaveSettings(ExtensionSettings extensionSettings)
        {
            if (string.IsNullOrEmpty(extensionSettings.Name))
            {
                extensionSettings.Name = this.Name;
            }

            foreach (var setItem in this.settings.Where(setItem => setItem.Name == extensionSettings.Name))
            {
                this.settings.Remove(setItem);
                break;
            }

            this.settings.Add(extensionSettings);
			this.settings.Sort((s1, s2) => string.Compare(s1.Index.ToString(), s2.Index.ToString()));
        }

        #endregion
    }
}