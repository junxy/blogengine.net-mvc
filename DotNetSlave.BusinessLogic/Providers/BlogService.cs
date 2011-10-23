namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Web.Configuration;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// The proxy class for communication between
    ///     the business objects and the providers.
    /// </summary>
    public static class BlogService
    {
        #region Constants and Fields

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object TheLock = new object();

        /// <summary>
        /// The provider. Don't access this directly. Access it through the property accessor.
        /// </summary>
        private static BlogProvider _provider;

        /// <summary>
        /// The providers.
        /// </summary>
        private static BlogProviderCollection _providers;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current provider.
        /// </summary>
        public static BlogProvider Provider
        {
            get
            {
                LoadProviders();
                return _provider;
            }
        }

        /// <summary>
        ///     Gets a collection of all registered providers.
        /// </summary>
        public static BlogProviderCollection Providers
        {
            get
            {
                LoadProviders();
                return _providers;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes the specified BlogRoll from the current provider.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void DeleteBlogRoll(BlogRollItem blogRoll)
        {
            Provider.DeleteBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Deletes the specified Category from the current provider.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void DeleteCategory(Category category)
        {
            Provider.DeleteCategory(category);
        }

        /// <summary>
        /// Deletes the specified Page from the current provider.
        /// </summary>
        /// <param name="page">
        /// The page to delete.
        /// </param>
        public static void DeletePage(Page page)
        {
            Provider.DeletePage(page);
        }

        /// <summary>
        /// Deletes the specified Post from the current provider.
        /// </summary>
        /// <param name="post">
        /// The post to delete.
        /// </param>
        public static void DeletePost(Post post)
        {
            Provider.DeletePost(post);
        }

        /// <summary>
        /// Deletes the specified Page from the current provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to delete.
        /// </param>
        public static void DeleteProfile(AuthorProfile profile)
        {
            Provider.DeleteProfile(profile);
        }

        /// <summary>
        /// Returns a list of all BlogRolls in the current provider.
        /// </summary>
        /// <returns>
        /// A list of BlogRollItem.
        /// </returns>
        public static List<BlogRollItem> FillBlogRolls()
        {
            return Provider.FillBlogRoll();
        }

        /// <summary>
        /// The fill categories.
        /// </summary>
        /// <returns>
        /// A list of Category.
        /// </returns>
        public static List<Category> FillCategories()
        {
            return Provider.FillCategories();
        }

        /// <summary>
        /// The fill pages.
        /// </summary>
        /// <returns>
        /// A list of Page.
        /// </returns>
        public static List<Page> FillPages()
        {
            return Provider.FillPages();
        }

        /// <summary>
        /// The fill posts.
        /// </summary>
        /// <returns>
        /// A list of Post.
        /// </returns>
        public static List<Post> FillPosts()
        {
            return Provider.FillPosts();
        }

        /// <summary>
        /// The fill profiles.
        /// </summary>
        /// <returns>
        /// A list of AuthorProfile.
        /// </returns>
        public static List<AuthorProfile> FillProfiles()
        {
            return Provider.FillProfiles();
        }

        /// <summary>
        /// Returns a list of all Referrers in the current provider.
        /// </summary>
        /// <returns>
        /// A list of Referrer.
        /// </returns>
        public static List<Referrer> FillReferrers()
        {
            return Provider.FillReferrers();
        }

        /// <summary>
        /// Returns a dictionary representing rights and the roles that allow them.
        /// </summary>
        /// <returns>
        /// 
        /// The key must be a string of the name of the Rights enum of the represented Right.
        /// The value must be an IEnumerable of strings that includes only the role names of
        /// roles the right represents.
        /// 
        /// Inheritors do not need to worry about verifying that the keys and values are valid.
        /// This is handled in the Right class.
        /// 
        /// </returns>
        public static IDictionary<string, IEnumerable<string>> FillRights()
        {
            return Provider.FillRights();
        }

        /// <summary>
        /// Persists a new BlogRoll in the current provider.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void InsertBlogRoll(BlogRollItem blogRoll)
        {
            Provider.InsertBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Persists a new Category in the current provider.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void InsertCategory(Category category)
        {
            Provider.InsertCategory(category);
        }

        /// <summary>
        /// Persists a new Page in the current provider.
        /// </summary>
        /// <param name="page">
        /// The page to insert.
        /// </param>
        public static void InsertPage(Page page)
        {
            Provider.InsertPage(page);
        }

        /// <summary>
        /// Persists a new Post in the current provider.
        /// </summary>
        /// <param name="post">
        /// The post to insert.
        /// </param>
        public static void InsertPost(Post post)
        {
            Provider.InsertPost(post);
        }

        /// <summary>
        /// Persists a new Page in the current provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to insert.
        /// </param>
        public static void InsertProfile(AuthorProfile profile)
        {
            Provider.InsertProfile(profile);
        }

        /// <summary>
        /// Persists a new Referrer in the current provider.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to insert.
        /// </param>
        public static void InsertReferrer(Referrer referrer)
        {
            Provider.InsertReferrer(referrer);
        }

        /// <summary>
        /// Loads settings from data storage
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <returns>
        /// Settings as stream
        /// </returns>
        public static object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            return Provider.LoadFromDataStore(extensionType, extensionId);
        }

        /// <summary>
        /// Loads the ping services.
        /// </summary>
        /// <returns>A StringCollection.</returns>
        public static StringCollection LoadPingServices()
        {
            return Provider.LoadPingServices();
        }

        /// <summary>
        /// Loads the settings from the provider and returns
        /// them in a StringDictionary for the BlogSettings class to use.
        /// </summary>
        /// <returns>A StringDictionary.</returns>
        public static StringDictionary LoadSettings()
        {
            return Provider.LoadSettings();
        }

        /// <summary>
        /// Loads the stop words from the data store.
        /// </summary>
        /// <returns>A StringCollection.</returns>
        public static StringCollection LoadStopWords()
        {
            return Provider.LoadStopWords();
        }

        /// <summary>
        /// Removes object from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        public static void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            Provider.RemoveFromDataStore(extensionType, extensionId);
        }

        /// <summary>
        /// Saves the ping services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public static void SavePingServices(StringCollection services)
        {
            Provider.SavePingServices(services);
        }

        /// <summary>
        /// Saves all of the current BlogEngine rights to the provider.
        /// </summary>
        public static void SaveRights()
        {
            Provider.SaveRights(Right.GetAllRights());

            // This needs to be called after rights are changed.
            Right.RefreshAllRights();
        }

        /// <summary>
        /// Save the settings to the current provider.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveSettings(StringDictionary settings)
        {
            Provider.SaveSettings(settings);
        }

        /// <summary>
        /// Saves settings to data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extensio ID
        /// </param>
        /// <param name="settings">
        /// Settings object
        /// </param>
        public static void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            Provider.SaveToDataStore(extensionType, extensionId, settings);
        }

        /// <summary>
        /// Returns a BlogRoll based on the specified id.
        /// </summary>
        /// <param name="id">The BlogRoll id.</param>
        /// <returns>A BlogRollItem.</returns>
        public static BlogRollItem SelectBlogRoll(Guid id)
        {
            return Provider.SelectBlogRollItem(id);
        }

        /// <summary>
        /// Returns a Category based on the specified id.
        /// </summary>
        /// <param name="id">The Category id.</param>
        /// <returns>A Category.</returns>
        public static Category SelectCategory(Guid id)
        {
            return Provider.SelectCategory(id);
        }

        /// <summary>
        /// Returns a Page based on the specified id.
        /// </summary>
        /// <param name="id">The Page id.</param>
        /// <returns>A Page object.</returns>
        public static Page SelectPage(Guid id)
        {
            return Provider.SelectPage(id);
        }

        /// <summary>
        /// Returns a Post based on the specified id.
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <returns>A Post object.</returns>
        public static Post SelectPost(Guid id)
        {
            return Provider.SelectPost(id);
        }

        /// <summary>
        /// Returns a Page based on the specified id.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>An AuthorProfile.</returns>
        public static AuthorProfile SelectProfile(string id)
        {
            return Provider.SelectProfile(id);
        }

        /// <summary>
        /// Returns a Referrer based on the specified id.
        /// </summary>
        /// <param name="id">The Referrer Id.</param>
        /// <returns>A Referrer.</returns>
        public static Referrer SelectReferrer(Guid id)
        {
            return Provider.SelectReferrer(id);
        }

        /// <summary>
        /// Updates an exsiting BlogRoll.
        /// </summary>
        /// <param name="blogRoll">
        /// The blog Roll.
        /// </param>
        public static void UpdateBlogRoll(BlogRollItem blogRoll)
        {
            Provider.UpdateBlogRollItem(blogRoll);
        }

        /// <summary>
        /// Updates an exsiting Category.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public static void UpdateCategory(Category category)
        {
            Provider.UpdateCategory(category);
        }

        /// <summary>
        /// Updates an exsiting Page.
        /// </summary>
        /// <param name="page">
        /// The page to update.
        /// </param>
        public static void UpdatePage(Page page)
        {
            Provider.UpdatePage(page);
        }

        /// <summary>
        /// Updates an exsiting Post.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        public static void UpdatePost(Post post)
        {
            Provider.UpdatePost(post);
        }

        /// <summary>
        /// Updates an exsiting Page.
        /// </summary>
        /// <param name="profile">
        /// The profile to update.
        /// </param>
        public static void UpdateProfile(AuthorProfile profile)
        {
            Provider.UpdateProfile(profile);
        }

        /// <summary>
        /// Updates an existing Referrer.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to update.
        /// </param>
        public static void UpdateReferrer(Referrer referrer)
        {
            Provider.UpdateReferrer(referrer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the providers from the web.config.
        /// </summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (TheLock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <blogProvider> section
                        var section = (BlogProviderSection)WebConfigurationManager.GetSection("BlogEngine/blogProvider");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new BlogProviderCollection();
                        ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(BlogProvider));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                        {
                            throw new ProviderException("Unable to load default BlogProvider");
                        }
                    }
                }
            }
        }

        #endregion
    }
}