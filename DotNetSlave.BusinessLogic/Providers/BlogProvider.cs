﻿namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// A base class for all custom providers to inherit from.
    /// </summary>
    public abstract class BlogProvider : ProviderBase
    {
        // Post
        #region Public Methods

        /// <summary>
        /// Deletes a BlogRoll from the data store specified by the provider.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item to delete.
        /// </param>
        public abstract void DeleteBlogRollItem(BlogRollItem blogRollItem);

        /// <summary>
        /// Deletes a Category from the data store specified by the provider.
        /// </summary>
        /// <param name="category">
        /// The category to delete.
        /// </param>
        public abstract void DeleteCategory(Category category);

        /// <summary>
        /// Deletes a Page from the data store specified by the provider.
        /// </summary>
        /// <param name="page">
        /// The page to delete.
        /// </param>
        public abstract void DeletePage(Page page);

        /// <summary>
        /// Deletes a Post from the data store specified by the provider.
        /// </summary>
        /// <param name="post">
        /// The post to delete.
        /// </param>
        public abstract void DeletePost(Post post);

        /// <summary>
        /// Deletes a Page from the data store specified by the provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to delete.
        /// </param>
        public abstract void DeleteProfile(AuthorProfile profile);

        /// <summary>
        /// Retrieves all BlogRolls from the provider and returns them in a list.
        /// </summary>
        /// <returns>A list of BlogRollItem.</returns>
        public abstract List<BlogRollItem> FillBlogRoll();

        /// <summary>
        /// Retrieves all Categories from the provider and returns them in a List.
        /// </summary>
        /// <returns>A list of Category.</returns>
        public abstract List<Category> FillCategories();

        /// <summary>
        /// Retrieves all Pages from the provider and returns them in a List.
        /// </summary>
        /// <returns>A list of Page.</returns>
        public abstract List<Page> FillPages();

        /// <summary>
        /// Retrieves all Posts from the provider and returns them in a List.
        /// </summary>
        /// <returns>A list of Post.</returns>
        public abstract List<Post> FillPosts();

        /// <summary>
        /// Retrieves all Pages from the provider and returns them in a List.
        /// </summary>
        /// <returns>A list of AuthorProfile.</returns>
        public abstract List<AuthorProfile> FillProfiles();

        /// <summary>
        /// Deletes a Referrer from the data store specified by the provider.
        /// </summary>
        /// <returns>A list of Referrer.</returns>
        public abstract List<Referrer> FillReferrers();

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
        public abstract IDictionary<string, IEnumerable<String>> FillRights();

        /// <summary>
        /// Inserts a new BlogRoll into the data store specified by the provider.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public abstract void InsertBlogRollItem(BlogRollItem blogRollItem);

        /// <summary>
        /// Inserts a new Category into the data store specified by the provider.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public abstract void InsertCategory(Category category);

        /// <summary>
        /// Inserts a new Page into the data store specified by the provider.
        /// </summary>
        /// <param name="page">
        /// The page to insert.
        /// </param>
        public abstract void InsertPage(Page page);

        /// <summary>
        /// Inserts a new Post into the data store specified by the provider.
        /// </summary>
        /// <param name="post">
        /// The post to insert.
        /// </param>
        public abstract void InsertPost(Post post);

        /// <summary>
        /// Inserts a new Page into the data store specified by the provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to insert.
        /// </param>
        public abstract void InsertProfile(AuthorProfile profile);

        /// <summary>
        /// Inserts a new Referrer into the data store specified by the provider.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to insert.
        /// </param>
        public abstract void InsertReferrer(Referrer referrer);

        /// <summary>
        /// Loads settings from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extensio Id
        /// </param>
        /// <returns>
        /// Settings as stream
        /// </returns>
        public abstract object LoadFromDataStore(ExtensionType extensionType, string extensionId);

        /// <summary>
        /// Loads the ping services.
        /// </summary>
        /// <returns>
        /// A StringCollection.
        /// </returns>
        public abstract StringCollection LoadPingServices();

        /// <summary>
        /// Loads the settings from the provider.
        /// </summary>
        /// <returns>A StringDictionary.</returns>
        public abstract StringDictionary LoadSettings();

        /// <summary>
        /// Loads the stop words used in the search feature.
        /// </summary>
        /// <returns>
        /// A StringCollection.
        /// </returns>
        public abstract StringCollection LoadStopWords();

        /// <summary>
        /// Removes settings from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        public abstract void RemoveFromDataStore(ExtensionType extensionType, string extensionId);

        /// <summary>
        /// Saves the ping services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public abstract void SavePingServices(StringCollection services);

        /// <summary>
        /// Saves all of the Rights and the roles that coorespond with them.
        /// </summary>
        /// <param name="rights"></param>
        public abstract void SaveRights(IEnumerable<Right> rights);

        /// <summary>
        /// Saves the settings to the provider.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public abstract void SaveSettings(StringDictionary settings);

        /// <summary>
        /// Saves settings to data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        /// <param name="settings">
        /// Settings object
        /// </param>
        public abstract void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings);

        /// <summary>
        /// Retrieves a BlogRoll from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Blog Roll Item Id.</param>
        /// <returns>A BlogRollItem.</returns>
        public abstract BlogRollItem SelectBlogRollItem(Guid id);

        /// <summary>
        /// Retrieves a Category from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Category id.</param>
        /// <returns>A Category.</returns>
        public abstract Category SelectCategory(Guid id);

        /// <summary>
        /// Retrieves a Page from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Page id.</param>
        /// <returns>The Page object.</returns>
        public abstract Page SelectPage(Guid id);

        /// <summary>
        /// Retrieves a Post from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Post id.</param>
        /// <returns>A Post object.</returns>
        public abstract Post SelectPost(Guid id);

        /// <summary>
        /// Retrieves a Page from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The AuthorProfile id.</param>
        /// <returns>An AuthorProfile.</returns>
        public abstract AuthorProfile SelectProfile(string id);

        /// <summary>
        /// Retrieves a Referrer from the provider based on the specified id.
        /// </summary>
        /// <param name="id">The Referrer Id.</param>
        /// <returns>A Referrer.</returns>
        public abstract Referrer SelectReferrer(Guid id);

        /// <summary>
        /// Updates an existing BlogRoll in the data store specified by the provider.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item to update.
        /// </param>
        public abstract void UpdateBlogRollItem(BlogRollItem blogRollItem);

        /// <summary>
        /// Updates an existing Category in the data store specified by the provider.
        /// </summary>
        /// <param name="category">
        /// The category to update.
        /// </param>
        public abstract void UpdateCategory(Category category);

        /// <summary>
        /// Updates an existing Page in the data store specified by the provider.
        /// </summary>
        /// <param name="page">
        /// The page to update.
        /// </param>
        public abstract void UpdatePage(Page page);

        /// <summary>
        /// Updates an existing Post in the data store specified by the provider.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        public abstract void UpdatePost(Post post);

        /// <summary>
        /// Updates an existing Page in the data store specified by the provider.
        /// </summary>
        /// <param name="profile">
        /// The profile to update.
        /// </param>
        public abstract void UpdateProfile(AuthorProfile profile);

        /// <summary>
        /// Updates an existing Referrer in the data store specified by the provider.
        /// </summary>
        /// <param name="referrer">
        /// The referrer to update.
        /// </param>
        public abstract void UpdateReferrer(Referrer referrer);

        #endregion
    }
}