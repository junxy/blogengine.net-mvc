namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Web.Configuration;
    using System.Xml.Serialization;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// Generic Database BlogProvider
    /// </summary>
    public class DbBlogProvider : BlogProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The conn string name.
        /// </summary>
        private string connStringName;

        /// <summary>
        /// The parm prefix.
        /// </summary>
        private string parmPrefix;

        /// <summary>
        /// The table prefix.
        /// </summary>
        private string tablePrefix;

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes a BlogRoll from the database
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void DeleteBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Remove(blogRollItem);
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}BlogRollItems WHERE BlogRollId = {1}BlogRollId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("BlogRollId"), blogRollItem.Id.ToString()));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a category from the database
        /// </summary>
        /// <param name="category">
        /// category to be removed
        /// </param>
        public override void DeleteCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Categories WHERE CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a page from the database
        /// </summary>
        /// <param name="page">
        /// page to be deleted
        /// </param>
        public override void DeletePage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Pages WHERE PageID = {1}id", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a post in the database
        /// </summary>
        /// <param name="post">
        /// post to delete
        /// </param>
        public override void DeletePost(Post post)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}PostTag WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostNotify WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}PostComment WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = string.Format("DELETE FROM {0}Posts WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        /// <summary>
        /// Remove AuthorProfile from database
        /// </summary>
        /// <param name="profile">An AuthorProfile.</param>
        public override void DeleteProfile(AuthorProfile profile)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Profiles WHERE UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), profile.Id));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets all BlogRolls in database
        /// </summary>
        /// <returns>
        /// List of BlogRolls
        /// </returns>
        public override List<BlogRollItem> FillBlogRoll()
        {
            var blogRoll = new List<BlogRollItem>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex FROM {0}BlogRollItems ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var br = new BlogRollItem
                                    {
                                        Id = rdr.GetGuid(0),
                                        Title = rdr.GetString(1),
                                        Description = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                                        BlogUrl = rdr.IsDBNull(3) ? null : new Uri(rdr.GetString(3)),
                                        FeedUrl = rdr.IsDBNull(4) ? null : new Uri(rdr.GetString(4)),
                                        Xfn = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5),
                                        SortIndex = rdr.GetInt32(6)
                                    };

                                blogRoll.Add(br);
                                br.MarkOld();
                            }
                        }
                    }
                }
            }

            return blogRoll;
        }

        /// <summary>
        /// Gets all categories in database
        /// </summary>
        /// <returns>
        /// List of categories
        /// </returns>
        public override List<Category> FillCategories()
        {
            var categories = new List<Category>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT CategoryID, CategoryName, description, ParentID FROM {0}Categories ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var cat = new Category
                                    {
                                        Title = rdr.GetString(1),
                                        Description = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                                        Parent = rdr.IsDBNull(3) ? (Guid?)null : new Guid(rdr.GetGuid(3).ToString()),
                                        Id = new Guid(rdr.GetGuid(0).ToString())
                                    };

                                categories.Add(cat);
                                cat.MarkOld();
                            }
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Gets all pages in database
        /// </summary>
        /// <returns>
        /// List of pages
        /// </returns>
        public override List<Page> FillPages()
        {
            var pageIDs = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PageID FROM {0}Pages ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                pageIDs.Add(rdr.GetGuid(0).ToString());
                            }
                        }
                    }
                }
            }

            return pageIDs.Select(id => Page.Load(new Guid(id))).ToList();
        }

        /// <summary>
        /// Gets all post from the database
        /// </summary>
        /// <returns>
        /// List of posts
        /// </returns>
        public override List<Post> FillPosts()
        {
            var postIDs = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT PostID FROM {0}Posts ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                postIDs.Add(rdr.GetGuid(0).ToString());
                            }
                        }
                    }
                }
            }

            var posts = postIDs.Select(id => Post.Load(new Guid(id))).ToList();

            posts.Sort();
            return posts;
        }

        /// <summary>
        /// Return collection for AuthorProfiles from database
        /// </summary>
        /// <returns>
        /// List of AuthorProfile
        /// </returns>
        public override List<AuthorProfile> FillProfiles()
        {
            var profileNames = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT UserName FROM {0}Profiles GROUP BY UserName", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                profileNames.Add(rdr.GetString(0));
                            }
                        }
                    }
                }
            }

            return profileNames.Select(BusinessBase<AuthorProfile, string>.Load).ToList();
        }

        /// <summary>
        /// Gets all Referrers from the database.
        /// </summary>
        /// <returns>
        /// List of Referrers.
        /// </returns>
        public override List<Referrer> FillReferrers()
        {
            this.DeleteOldReferrers();

            var referrers = new List<Referrer>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam FROM {0}Referrers ", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var refer = new Referrer
                                    {
                                        Id = rdr.GetGuid(0),
                                        Day = rdr.GetDateTime(1),
                                        ReferrerUrl = new Uri(rdr.GetString(2)),
                                        Count = rdr.GetInt32(3),
                                        Url = rdr.IsDBNull(4) ? null : new Uri(rdr.GetString(4)),
                                        PossibleSpam = rdr.IsDBNull(5) ? false : rdr.GetBoolean(5)
                                    };

                                referrers.Add(refer);
                                refer.MarkOld();
                            }
                        }
                    }
                }
            }

            return referrers;
        }

        public override IDictionary<string, IEnumerable<string>> FillRights()
        {
            var rightsWithRoles = new Dictionary<string, IEnumerable<string>>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT RightName FROM {0}Rights ", this.tablePrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                rightsWithRoles.Add(rdr.GetString(0), new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                            }
                        }

                        // Get Right Roles.
                        cmd.CommandText = string.Format("SELECT RightName, Role FROM {0}RightRoles ", this.tablePrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string rightName = rdr.GetString(0);
                                string roleName = rdr.GetString(1);

                                if (rightsWithRoles.ContainsKey(rightName))
                                {
                                    var roles = (HashSet<string>)rightsWithRoles[rightName];
                                    if (!roles.Contains(roleName))
                                    {
                                        roles.Add(roleName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return rightsWithRoles;
        }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="name">
        /// Configuration name
        /// </param>
        /// <param name="config">
        /// Configuration settings
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "DbBlogProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Generic Database Blog Provider");
            }

            base.Initialize(name, config);

            if (config["connectionStringName"] == null)
            {
                // default to BlogEngine
                config["connectionStringName"] = "BlogEngine";
            }

            this.connStringName = config["connectionStringName"];
            config.Remove("connectionStringName");

            if (config["tablePrefix"] == null)
            {
                // default
                config["tablePrefix"] = "be_";
            }

            this.tablePrefix = config["tablePrefix"];
            config.Remove("tablePrefix");

            if (config["parmPrefix"] == null)
            {
                // default
                config["parmPrefix"] = "@";
            }

            this.parmPrefix = config["parmPrefix"];
            config.Remove("parmPrefix");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                var attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                {
                    throw new ProviderException(string.Format("Unrecognized attribute: {0}", attr));
                }
            }
        }

        /// <summary>
        /// Adds a new BlogRoll to the database.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog Roll Item.
        /// </param>
        public override void InsertBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    var sqlQuery = string.Format("INSERT INTO {0}BlogRollItems (BlogRollId, Title, Description, BlogUrl, FeedUrl, Xfn, SortIndex) VALUES ({1}BlogRollId, {1}Title, {1}Description, {1}BlogUrl, {1}FeedUrl, {1}Xfn, {1}SortIndex)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogRollParametersToCommand(blogRollItem, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new category to the database
        /// </summary>
        /// <param name="category">
        /// category to add
        /// </param>
        public override void InsertCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Add(category);
            categories.Sort();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Categories (CategoryID, CategoryName, description, ParentID) VALUES ({1}catid, {1}catname, {1}description, {1}parentid)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("catname"), category.Title));
                        parms.Add(conn.CreateParameter(FormatParamName("description"), category.Description));
                        parms.Add(conn.CreateParameter(FormatParamName("parentid"), (category.Parent == null ? (object)DBNull.Value : category.Parent.ToString())));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a page to the database
        /// </summary>
        /// <param name="page">
        /// page to be added
        /// </param>
        public override void InsertPage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Pages (PageID, Title, Description, PageContent, DateCreated, DateModified, Keywords, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted) VALUES ({1}id, {1}title, {1}desc, {1}content, {1}created, {1}modified, {1}keywords, {1}ispublished, {1}isfrontpage, {1}parent, {1}showinlist, {1}slug, {1}isdeleted)", this.tablePrefix, this.parmPrefix);
                   
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("title"), page.Title));
                        parms.Add(conn.CreateParameter(FormatParamName("desc"), page.Description));
                        parms.Add(conn.CreateParameter(FormatParamName("content"), page.Content));
                        parms.Add(conn.CreateParameter(FormatParamName("created"), page.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        parms.Add(conn.CreateParameter(FormatParamName("modified"),
                                                                    (page.DateModified == new DateTime() ? DateTime.Now : page.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        parms.Add(conn.CreateParameter(FormatParamName("keywords"), page.Keywords));
                        parms.Add(conn.CreateParameter(FormatParamName("ispublished"), page.IsPublished));
                        parms.Add(conn.CreateParameter(FormatParamName("isfrontpage"), page.IsFrontPage));
                        parms.Add(conn.CreateParameter(FormatParamName("parent"), page.Parent.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("showinlist"), page.ShowInList));
                        parms.Add(conn.CreateParameter(FormatParamName("slug"), page.Slug));
                        parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), page.IsDeleted));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new post to database
        /// </summary>
        /// <param name="post">
        /// The new post.
        /// </param>
        public override void InsertPost(Post post)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Posts (PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted)VALUES ({1}id, {1}title, {1}desc, {1}content, {1}created, {1}modified, {1}author, {1}published, {1}commentEnabled, {1}raters, {1}rating, {1}slug, {1}isdeleted)", this.tablePrefix, this.parmPrefix);
               
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        parms.Add(conn.CreateParameter(FormatParamName("title"), post.Title));
                        parms.Add(conn.CreateParameter(FormatParamName("desc"), (post.Description ?? string.Empty)));
                        parms.Add(conn.CreateParameter(FormatParamName("content"), post.Content));
                        parms.Add(conn.CreateParameter(FormatParamName("created"), post.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        parms.Add(conn.CreateParameter(FormatParamName("modified"), (post.DateModified == new DateTime() ? DateTime.Now : post.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        parms.Add(conn.CreateParameter(FormatParamName("author"), (post.Author ?? string.Empty)));
                        parms.Add(conn.CreateParameter(FormatParamName("published"), post.IsPublished));
                        parms.Add(conn.CreateParameter(FormatParamName("commentEnabled"), post.HasCommentsEnabled));
                        parms.Add(conn.CreateParameter(FormatParamName("raters"), post.Raters));
                        parms.Add(conn.CreateParameter(FormatParamName("rating"), post.Rating));
                        parms.Add(conn.CreateParameter(FormatParamName("slug"), (post.Slug ?? string.Empty)));
                        parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), post.IsDeleted));

                        cmd.ExecuteNonQuery();
                    }

                    // Tags
                    this.UpdateTags(post, conn);

                    // Categories
                    this.UpdateCategories(post, conn);

                    // Comments
                    this.UpdateComments(post, conn);

                    // Email Notification
                    this.UpdateNotify(post, conn);
                }
            }

        }

        /// <summary>
        /// Adds AuthorProfile to database
        /// </summary>
        /// <param name="profile">An AuthorProfile.</param>
        public override void InsertProfile(AuthorProfile profile)
        {
            this.UpdateProfile(profile);
        }

        /// <summary>
        /// Adds a new Referrer to the database.
        /// </summary>
        /// <param name="referrer">
        /// Referrer to add.
        /// </param>
        public override void InsertReferrer(Referrer referrer)
        {
            var referrers = Referrer.Referrers;
            referrers.Add(referrer);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("INSERT INTO {0}Referrers (ReferrerId, ReferralDay, ReferrerUrl, ReferralCount, Url, IsSpam) VALUES ({1}ReferrerId, {1}ReferralDay, {1}ReferrerUrl, {1}ReferralCount, {1}Url, {1}IsSpam)", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddReferrersParametersToCommand(referrer, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Load user data from DataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of info
        /// </param>
        /// <param name="extensionId">
        /// id of info
        /// </param>
        /// <returns>
        /// stream of detail data
        /// </returns>
        public override object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            // MemoryStream stream;
            object o = null;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT Settings FROM {0}DataStoreSettings WHERE ExtensionType = {1}etype AND ExtensionId = {1}eid", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("etype"), extensionType.GetHashCode()));
                        parms.Add(conn.CreateParameter(FormatParamName("eid"), extensionId));

                        o = cmd.ExecuteScalar();
                    }
                }
            }

            return o;
        }

        /// <summary>
        /// Gets the PingServices from the database
        /// </summary>
        /// <returns>
        /// collection of PingServices
        /// </returns>
        public override StringCollection LoadPingServices()
        {
            var col = new StringCollection();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT Link FROM {0}PingService", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!col.Contains(rdr.GetString(0)))
                                {
                                    col.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// Gets the settings from the database
        /// </summary>
        /// <returns>
        /// dictionary of settings
        /// </returns>
        public override StringDictionary LoadSettings()
        {
            var dic = new StringDictionary();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}Settings", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var name = rdr.GetString(0);
                                var value = rdr.GetString(1);

                                dic.Add(name, value);
                            }
                        }
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// Get stopwords from the database
        /// </summary>
        /// <returns>
        /// collection of stopwords
        /// </returns>
        public override StringCollection LoadStopWords()
        {
            var col = new StringCollection();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT StopWord FROM {0}StopWords", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var value = rdr.GetString(0);
                                if (!col.Contains(value))
                                {
                                    col.Add(value);
                                }
                            }
                        }
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// Deletes an item from the dataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of item
        /// </param>
        /// <param name="extensionId">
        /// id of item
        /// </param>
        public override void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}DataStoreSettings WHERE ExtensionType = {1}type AND ExtensionId = {1}id", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("type"), extensionType));
                        p.Add(conn.CreateParameter(FormatParamName("id"), extensionId));

                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        /// <summary>
        /// Saves the PingServices to the database
        /// </summary>
        /// <param name="services">
        /// collection of PingServices
        /// </param>
        public override void SavePingServices(StringCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}PingService", this.tablePrefix)))
                    {
                        cmd.ExecuteNonQuery();

                        foreach (var service in services)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}PingService (Link) VALUES ({1}link)", this.tablePrefix, this.parmPrefix);
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("link"), service));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public override void SaveRights(IEnumerable<Right> rights)
        {
            if (rights == null)
            {
                throw new ArgumentNullException("rights");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Rights", this.tablePrefix)))
                    {
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("DELETE FROM {0}RightRoles", this.tablePrefix);
                        cmd.ExecuteNonQuery();

                        foreach (var right in rights)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}Rights (RightName) VALUES ({1}RightName)", this.tablePrefix, this.parmPrefix);

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("RightName"), right.Name));

                            cmd.ExecuteNonQuery();

                            foreach (var role in right.Roles)
                            {
                                cmd.CommandText = string.Format("INSERT INTO {0}RightRoles (RightName, Role) VALUES ({1}RightName, {1}Role)", this.tablePrefix, this.parmPrefix);

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("RightName"), right.Name));
                                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("Role"), role));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Saves the settings to the database
        /// </summary>
        /// <param name="settings">
        /// dictionary of settings
        /// </param>
        public override void SaveSettings(StringDictionary settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Settings", this.tablePrefix)))
                    {
                        cmd.ExecuteNonQuery();

                        foreach (string key in settings.Keys)
                        {
                            cmd.CommandText = string.Format("INSERT INTO {0}Settings (SettingName, SettingValue) VALUES ({1}name, {1}value)", this.tablePrefix, this.parmPrefix);
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), key));
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("value"), settings[key]));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save to DataStore
        /// </summary>
        /// <param name="extensionType">
        /// type of info
        /// </param>
        /// <param name="extensionId">
        /// id of info
        /// </param>
        /// <param name="settings">
        /// data of info
        /// </param>
        public override void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Save

            var xs = new XmlSerializer(settings.GetType());
            string objectXml;
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, settings);
                objectXml = sw.ToString();
            }

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("DELETE FROM {0}DataStoreSettings WHERE ExtensionType = {1}type AND ExtensionId = {1}id; ", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("type"), extensionType.GetHashCode()));
                        p.Add(conn.CreateParameter(FormatParamName("id"), extensionId));

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("INSERT INTO {0}DataStoreSettings (ExtensionType, ExtensionId, Settings) VALUES ({1}type, {1}id, {1}file)", this.tablePrefix, this.parmPrefix);

                        p.Add(conn.CreateParameter(FormatParamName("file"), objectXml));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a BlogRoll based on a Guid.
        /// </summary>
        /// <param name="id">
        /// The BlogRoll's Guid.
        /// </param>
        /// <returns>
        /// A matching BlogRoll
        /// </returns>
        public override BlogRollItem SelectBlogRollItem(Guid id)
        {
            var blogRoll = BlogRollItem.BlogRolls.Find(br => br.Id == id) ?? new BlogRollItem();

            blogRoll.MarkOld();
            return blogRoll;
        }

        /// <summary>
        /// Returns a category
        /// </summary>
        /// <param name="id">Id of category to return</param>
        /// <returns>A category.</returns>
        public override Category SelectCategory(Guid id)
        {
            var categories = Category.Categories;

            var category = new Category();

            foreach (var cat in categories.Where(cat => cat.Id == id))
            {
                category = cat;
            }

            category.MarkOld();
            return category;
        }

        /// <summary>
        /// Returns a page for given ID
        /// </summary>
        /// <param name="id">
        /// ID of page to return
        /// </param>
        /// <returns>
        /// selected page
        /// </returns>
        public override Page SelectPage(Guid id)
        {
            var page = new Page();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT PageID, Title, Description, PageContent, DateCreated, DateModified, Keywords, IsPublished, IsFrontPage, Parent, ShowInList, Slug, IsDeleted FROM {0}Pages WHERE PageID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                page.Id = rdr.GetGuid(0);
                                page.Title = rdr.IsDBNull(1) ? String.Empty : rdr.GetString(1);
                                page.Content = rdr.IsDBNull(3) ? String.Empty : rdr.GetString(3);
                                page.Description = rdr.IsDBNull(2) ? String.Empty : rdr.GetString(2);
                                if (!rdr.IsDBNull(4))
                                {
                                    page.DateCreated = rdr.GetDateTime(4);
                                }

                                if (!rdr.IsDBNull(5))
                                {
                                    page.DateModified = rdr.GetDateTime(5);
                                }

                                if (!rdr.IsDBNull(6))
                                {
                                    page.Keywords = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    page.IsPublished = rdr.GetBoolean(7);
                                }

                                if (!rdr.IsDBNull(8))
                                {
                                    page.IsFrontPage = rdr.GetBoolean(8);
                                }

                                if (!rdr.IsDBNull(9))
                                {
                                    page.Parent = rdr.GetGuid(9);
                                }

                                if (!rdr.IsDBNull(10))
                                {
                                    page.ShowInList = rdr.GetBoolean(10);
                                }

                                if (!rdr.IsDBNull(11))
                                {
                                    page.Slug = rdr.GetString(11);
                                }

                                if (!rdr.IsDBNull(12))
                                {
                                    page.IsDeleted = rdr.GetBoolean(12);
                                }
                            }
                        }
                    }
                }
            }

            return page;
        }

        /// <summary>
        /// Returns a Post based on Id.
        /// </summary>
        /// <param name="id">
        /// The Post ID.
        /// </param>
        /// <returns>
        /// The Post..
        /// </returns>
        public override Post SelectPost(Guid id)
        {
            var post = new Post();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT PostID, Title, Description, PostContent, DateCreated, DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug, IsDeleted FROM {0}Posts WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), id.ToString()));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                post.Id = rdr.GetGuid(0);
                                post.Title = rdr.GetString(1);
                                post.Content = rdr.GetString(3);
                                post.Description = rdr.IsDBNull(2) ? String.Empty : rdr.GetString(2);
                                if (!rdr.IsDBNull(4))
                                {
                                    post.DateCreated = rdr.GetDateTime(4);
                                }

                                if (!rdr.IsDBNull(5))
                                {
                                    post.DateModified = rdr.GetDateTime(5);
                                }

                                if (!rdr.IsDBNull(6))
                                {
                                    post.Author = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    post.IsPublished = rdr.GetBoolean(7);
                                }

                                if (!rdr.IsDBNull(8))
                                {
                                    post.HasCommentsEnabled = rdr.GetBoolean(8);
                                }

                                if (!rdr.IsDBNull(9))
                                {
                                    post.Raters = rdr.GetInt32(9);
                                }

                                if (!rdr.IsDBNull(10))
                                {
                                    post.Rating = rdr.GetFloat(10);
                                }

                                post.Slug = !rdr.IsDBNull(11) ? rdr.GetString(11) : string.Empty;

                                if (!rdr.IsDBNull(12))
                                {
                                    post.IsDeleted = rdr.GetBoolean(12);
                                }
                            }
                        }

                        // Tags
                        cmd.CommandText = string.Format("SELECT Tag FROM {0}PostTag WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    post.Tags.Add(rdr.GetString(0));
                                }
                            }
                        }

                        post.Tags.MarkOld();

                        // Categories
                        cmd.CommandText = string.Format("SELECT CategoryID FROM {0}PostCategory WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var key = rdr.GetGuid(0);
                                if (Category.GetCategory(key) != null)
                                {
                                    post.Categories.Add(Category.GetCategory(key));
                                }
                            }
                        }

                        // Comments
                        cmd.CommandText = string.Format("SELECT PostCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ParentCommentID, ModeratedBy, Avatar, IsSpam, IsDeleted FROM {0}PostComment WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                var comment = new Comment
                                    {
                                        Id = rdr.GetGuid(0),
                                        IsApproved = true,
                                        Author = rdr.GetString(2)
                                    };
                                if (!rdr.IsDBNull(4))
                                {
                                    Uri website;
                                    if (Uri.TryCreate(rdr.GetString(4), UriKind.Absolute, out website))
                                    {
                                        comment.Website = website;
                                    }
                                }

                                comment.Email = rdr.GetString(3);
                                comment.Content = rdr.GetString(5);
                                comment.DateCreated = rdr.GetDateTime(1);
                                comment.Parent = post;

                                if (!rdr.IsDBNull(6))
                                {
                                    comment.Country = rdr.GetString(6);
                                }

                                if (!rdr.IsDBNull(7))
                                {
                                    comment.IP = rdr.GetString(7);
                                }

                                comment.IsApproved = rdr.IsDBNull(8) || rdr.GetBoolean(8);

                                comment.ParentId = rdr.GetGuid(9);

                                if (!rdr.IsDBNull(10))
                                {
                                    comment.ModeratedBy = rdr.GetString(10);
                                }

                                if (!rdr.IsDBNull(11))
                                {
                                    comment.Avatar = rdr.GetString(11);
                                }

                                if (!rdr.IsDBNull(12))
                                {
                                    comment.IsSpam = rdr.GetBoolean(12);
                                }

                                if (!rdr.IsDBNull(13))
                                {
                                    comment.IsDeleted = rdr.GetBoolean(13);
                                }

                                post.AllComments.Add(comment);
                            }
                        }

                        post.AllComments.Sort();

                        // Email Notification
                        cmd.CommandText = string.Format("SELECT NotifyAddress FROM {0}PostNotify WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    var email = rdr.GetString(0);
                                    if (post.NotificationEmails.Contains(email))
                                    {
                                        post.NotificationEmails.Add(email);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return post;
        }

        /// <summary>
        /// Loads AuthorProfile from database
        /// </summary>
        /// <param name="id">The user name.</param>
        /// <returns>An AuthorProfile.</returns>
        public override AuthorProfile SelectProfile(string id)
        {
            var dic = new StringDictionary();
            var profile = new AuthorProfile(id);

            // Retrieve Profile data from Db

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT SettingName, SettingValue FROM {0}Profiles WHERE UserName = {1}name", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), id));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                dic.Add(rdr.GetString(0), rdr.GetString(1));
                            }
                        }
                    }
                }
            }

            // Load profile with data from dictionary
            if (dic.ContainsKey("DisplayName"))
            {
                profile.DisplayName = dic["DisplayName"];
            }

            if (dic.ContainsKey("FirstName"))
            {
                profile.FirstName = dic["FirstName"];
            }

            if (dic.ContainsKey("MiddleName"))
            {
                profile.MiddleName = dic["MiddleName"];
            }

            if (dic.ContainsKey("LastName"))
            {
                profile.LastName = dic["LastName"];
            }

            if (dic.ContainsKey("CityTown"))
            {
                profile.CityTown = dic["CityTown"];
            }

            if (dic.ContainsKey("RegionState"))
            {
                profile.RegionState = dic["RegionState"];
            }

            if (dic.ContainsKey("Country"))
            {
                profile.Country = dic["Country"];
            }

            if (dic.ContainsKey("Birthday"))
            {
                DateTime date;
                if (DateTime.TryParse(dic["Birthday"], out date))
                {
                    profile.Birthday = date;
                }
            }

            if (dic.ContainsKey("AboutMe"))
            {
                profile.AboutMe = dic["AboutMe"];
            }

            if (dic.ContainsKey("PhotoURL"))
            {
                profile.PhotoUrl = dic["PhotoURL"];
            }

            if (dic.ContainsKey("Company"))
            {
                profile.Company = dic["Company"];
            }

            if (dic.ContainsKey("EmailAddress"))
            {
                profile.EmailAddress = dic["EmailAddress"];
            }

            if (dic.ContainsKey("PhoneMain"))
            {
                profile.PhoneMain = dic["PhoneMain"];
            }

            if (dic.ContainsKey("PhoneMobile"))
            {
                profile.PhoneMobile = dic["PhoneMobile"];
            }

            if (dic.ContainsKey("PhoneFax"))
            {
                profile.PhoneFax = dic["PhoneFax"];
            }

            if (dic.ContainsKey("IsPrivate"))
            {
                profile.Private = dic["IsPrivate"] == "true";
            }

            return profile;
        }

        /// <summary>
        /// Gets a Referrer based on an Id.
        /// </summary>
        /// <param name="id">
        /// The Referrer Id.
        /// </param>
        /// <returns>
        /// A matching Referrer
        /// </returns>
        public override Referrer SelectReferrer(Guid id)
        {
            var refer = Referrer.Referrers.Find(r => r.Id.Equals(id)) ?? new Referrer();

            refer.MarkOld();
            return refer;
        }

        /// <summary>
        /// Saves an existing BlogRoll to the database
        /// </summary>
        /// <param name="blogRollItem">
        /// BlogRoll to be saved
        /// </param>
        public override void UpdateBlogRollItem(BlogRollItem blogRollItem)
        {
            var blogRolls = BlogRollItem.BlogRolls;
            blogRolls.Remove(blogRollItem);
            blogRolls.Add(blogRollItem);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}BlogRollItems SET Title = {1}Title, Description = {1}Description, BlogUrl = {1}BlogUrl, FeedUrl = {1}FeedUrl, Xfn = {1}Xfn, SortIndex = {1}SortIndex WHERE BlogRollId = {1}BlogRollId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddBlogRollParametersToCommand(blogRollItem, conn, cmd);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing category to the database
        /// </summary>
        /// <param name="category">
        /// category to be saved
        /// </param>
        public override void UpdateCategory(Category category)
        {
            var categories = Category.Categories;
            categories.Remove(category);
            categories.Add(category);
            categories.Sort();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Categories SET CategoryName = {1}catname, Description = {1}description, ParentID = {1}parentid WHERE CategoryID = {1}catid", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("catid"), category.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("catname"), category.Title));
                        p.Add(conn.CreateParameter(FormatParamName("description"), category.Description));
                        p.Add(conn.CreateParameter(FormatParamName("parentid"), (category.Parent == null ? (object)DBNull.Value : category.Parent.ToString())));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing page in the database
        /// </summary>
        /// <param name="page">
        /// page to be saved
        /// </param>
        public override void UpdatePage(Page page)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Pages SET Title = {1}title, Description = {1}desc, PageContent = {1}content, DateCreated = {1}created, DateModified = {1}modified, Keywords = {1}keywords, IsPublished = {1}ispublished, IsFrontPage = {1}isfrontpage, Parent = {1}parent, ShowInList = {1}showinlist, Slug = {1}slug, IsDeleted = {1}isdeleted WHERE PageID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var p = cmd.Parameters;

                        p.Add(conn.CreateParameter(FormatParamName("id"), page.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("title"), page.Title));
                        p.Add(conn.CreateParameter(FormatParamName("desc"), page.Description));
                        p.Add(conn.CreateParameter(FormatParamName("content"), page.Content));
                        p.Add(conn.CreateParameter(FormatParamName("created"), page.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        p.Add(conn.CreateParameter(FormatParamName("modified"), (page.DateModified == new DateTime() ? DateTime.Now : page.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        p.Add(conn.CreateParameter(FormatParamName("keywords"), page.Keywords));
                        p.Add(conn.CreateParameter(FormatParamName("ispublished"), page.IsPublished));
                        p.Add(conn.CreateParameter(FormatParamName("isfrontpage"), page.IsFrontPage));
                        p.Add(conn.CreateParameter(FormatParamName("parent"), page.Parent.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("showinlist"), page.ShowInList));
                        p.Add(conn.CreateParameter(FormatParamName("slug"), page.Slug));
                        p.Add(conn.CreateParameter(FormatParamName("isdeleted"), page.IsDeleted));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves and existing post in the database
        /// </summary>
        /// <param name="post">
        /// post to be saved
        /// </param>
        public override void UpdatePost(Post post)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Posts SET Title = {1}title, Description = {1}desc, PostContent = {1}content, DateCreated = {1}created, DateModified = {1}modified, Author = {1}Author, IsPublished = {1}published, IsCommentEnabled = {1}commentEnabled, Raters = {1}raters, Rating = {1}rating, Slug = {1}slug, IsDeleted = {1}isdeleted WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {

                        var p = cmd.Parameters;
                        p.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                        p.Add(conn.CreateParameter(FormatParamName("title"), post.Title));
                        p.Add(conn.CreateParameter(FormatParamName("desc"), (post.Description ?? string.Empty)));
                        p.Add(conn.CreateParameter(FormatParamName("content"), post.Content));
                        p.Add(conn.CreateParameter(FormatParamName("created"), post.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                        p.Add(conn.CreateParameter(FormatParamName("modified"), (post.DateModified == new DateTime() ? DateTime.Now : post.DateModified.AddHours(-BlogSettings.Instance.Timezone))));
                        p.Add(conn.CreateParameter(FormatParamName("author"), (post.Author ?? string.Empty)));
                        p.Add(conn.CreateParameter(FormatParamName("published"), post.IsPublished));
                        p.Add(conn.CreateParameter(FormatParamName("commentEnabled"), post.HasCommentsEnabled));
                        p.Add(conn.CreateParameter(FormatParamName("raters"), post.Raters));
                        p.Add(conn.CreateParameter(FormatParamName("rating"), post.Rating));
                        p.Add(conn.CreateParameter(FormatParamName("slug"), (post.Slug ?? string.Empty)));
                        p.Add(conn.CreateParameter(FormatParamName("isdeleted"), post.IsDeleted));

                        cmd.ExecuteNonQuery();
                    }

                    // Tags
                    this.UpdateTags(post, conn);

                    // Categories
                    this.UpdateCategories(post, conn);

                    // Comments
                    this.UpdateComments(post, conn);

                    // Email Notification
                    this.UpdateNotify(post, conn);
                }
            }
        }

        /// <summary>
        /// Updates AuthorProfile to database
        /// </summary>
        /// <param name="profile">
        /// An AuthorProfile.
        /// </param>
        public override void UpdateProfile(AuthorProfile profile)
        {
            // Remove Profile
            this.DeleteProfile(profile);

            // Create Profile Dictionary
            var dic = new StringDictionary();

            if (!String.IsNullOrEmpty(profile.DisplayName))
            {
                dic.Add("DisplayName", profile.DisplayName);
            }

            if (!String.IsNullOrEmpty(profile.FirstName))
            {
                dic.Add("FirstName", profile.FirstName);
            }

            if (!String.IsNullOrEmpty(profile.MiddleName))
            {
                dic.Add("MiddleName", profile.MiddleName);
            }

            if (!String.IsNullOrEmpty(profile.LastName))
            {
                dic.Add("LastName", profile.LastName);
            }

            if (!String.IsNullOrEmpty(profile.CityTown))
            {
                dic.Add("CityTown", profile.CityTown);
            }

            if (!String.IsNullOrEmpty(profile.RegionState))
            {
                dic.Add("RegionState", profile.RegionState);
            }

            if (!String.IsNullOrEmpty(profile.Country))
            {
                dic.Add("Country", profile.Country);
            }

            if (!String.IsNullOrEmpty(profile.AboutMe))
            {
                dic.Add("AboutMe", profile.AboutMe);
            }

            if (!String.IsNullOrEmpty(profile.PhotoUrl))
            {
                dic.Add("PhotoURL", profile.PhotoUrl);
            }

            if (!String.IsNullOrEmpty(profile.Company))
            {
                dic.Add("Company", profile.Company);
            }

            if (!String.IsNullOrEmpty(profile.EmailAddress))
            {
                dic.Add("EmailAddress", profile.EmailAddress);
            }

            if (!String.IsNullOrEmpty(profile.PhoneMain))
            {
                dic.Add("PhoneMain", profile.PhoneMain);
            }

            if (!String.IsNullOrEmpty(profile.PhoneMobile))
            {
                dic.Add("PhoneMobile", profile.PhoneMobile);
            }

            if (!String.IsNullOrEmpty(profile.PhoneFax))
            {
                dic.Add("PhoneFax", profile.PhoneFax);
            }

            if (profile.Birthday != DateTime.MinValue)
            {
                dic.Add("Birthday", profile.Birthday.ToString("yyyy-MM-dd"));
            }

            dic.Add("IsPrivate", profile.Private.ToString());

            // Save Profile Dictionary

            using (var conn = this.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    foreach (string key in dic.Keys)
                    {
                        var sqlQuery = string.Format("INSERT INTO {0}Profiles (UserName, SettingName, SettingValue) VALUES ({1}user, {1}name, {1}value)", this.tablePrefix, this.parmPrefix);

                        cmd.CommandText = sqlQuery;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("user"), profile.Id));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), key));
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("value"), dic[key]));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves an existing Referrer to the database.
        /// </summary>
        /// <param name="referrer">
        /// Referrer to be saved.
        /// </param>
        public override void UpdateReferrer(Referrer referrer)
        {
            var referrers = Referrer.Referrers;
            referrers.Remove(referrer);
            referrers.Add(referrer);

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("UPDATE {0}Referrers SET ReferralDay = {1}ReferralDay, ReferrerUrl = {1}ReferrerUrl, ReferralCount = {1}ReferralCount, Url = {1}Url, IsSpam = {1}IsSpam WHERE ReferrerId = {1}ReferrerId", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        this.AddReferrersParametersToCommand(referrer, conn, cmd);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Returns a formatted parameter name to include this DbBlogProvider instance's paramPrefix.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private string FormatParamName(string parameterName)
        {
            return String.Format("{0}{1}", this.parmPrefix, parameterName);
        }

        #endregion

        #region Methods




        /// <summary>
        /// The update categories.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateCategories(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostCategory WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {
                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var cat in post.Categories)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostCategory (PostID, CategoryID) VALUES ({1}id, {1}cat)", this.tablePrefix, this.parmPrefix);
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("cat"), cat.Id.ToString()));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update comments.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateComments(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostComment WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {

                var parms = cmd.Parameters;

                parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var comment in post.AllComments)
                {
                    sqlQuery = string.Format("INSERT INTO {0}PostComment (PostCommentID, ParentCommentID, PostID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ModeratedBy, Avatar, IsSpam, IsDeleted) VALUES ({1}postcommentid, {1}parentid, {1}id, {1}date, {1}author, {1}email, {1}website, {1}comment, {1}country, {1}ip, {1}isapproved, {1}moderatedby, {1}avatar, {1}isspam, {1}isdeleted)", this.tablePrefix, this.parmPrefix);

                    cmd.CommandText = sqlQuery;
                    parms.Clear();

                    parms.Add(conn.CreateParameter(FormatParamName("postcommentid"), comment.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("parentid"), comment.ParentId.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("date"), comment.DateCreated.AddHours(-BlogSettings.Instance.Timezone)));
                    parms.Add(conn.CreateParameter(FormatParamName("author"), comment.Author));
                    parms.Add(conn.CreateParameter(FormatParamName("email"), comment.Email));
                    parms.Add(conn.CreateParameter(FormatParamName("website"), (comment.Website == null ? string.Empty : comment.Website.ToString())));
                    parms.Add(conn.CreateParameter(FormatParamName("comment"), comment.Content));
                    parms.Add(conn.CreateParameter(FormatParamName("country"), (comment.Country ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("ip"), (comment.IP ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("isapproved"), comment.IsApproved));
                    parms.Add(conn.CreateParameter(FormatParamName("moderatedby"), (comment.ModeratedBy ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("avatar"), (comment.Avatar ?? string.Empty)));
                    parms.Add(conn.CreateParameter(FormatParamName("isspam"), comment.IsSpam));
                    parms.Add(conn.CreateParameter(FormatParamName("isdeleted"), comment.IsDeleted));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update notify.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        private void UpdateNotify(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostNotify WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {
                var parms = cmd.Parameters;

                parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));

                cmd.ExecuteNonQuery();

                foreach (var email in post.NotificationEmails)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostNotify (PostID, NotifyAddress) VALUES ({1}id, {1}notify)", this.tablePrefix, this.parmPrefix);
                    parms.Clear();

                    parms.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    parms.Add(conn.CreateParameter(FormatParamName("notify"), email));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The update tags.
        /// </summary>
        /// <param name="post">
        /// The post to update.
        /// </param>
        /// <param name="conn">
        /// The connection
        /// </param>
        private void UpdateTags(Post post, DbConnectionHelper conn)
        {
            var sqlQuery = string.Format("DELETE FROM {0}PostTag WHERE PostID = {1}id", this.tablePrefix, this.parmPrefix);
            using (var cmd = conn.CreateTextCommand(sqlQuery))
            {

                cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                cmd.ExecuteNonQuery();

                foreach (var tag in post.Tags)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0}PostTag (PostID, Tag) VALUES ({1}id, {1}tag)", this.tablePrefix, this.parmPrefix);
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("id"), post.Id.ToString()));
                    cmd.Parameters.Add(conn.CreateParameter(FormatParamName("tag"), tag));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The add blog roll parameters to command.
        /// </summary>
        /// <param name="blogRollItem">
        /// The blog roll item.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void AddBlogRollParametersToCommand(
            BlogRollItem blogRollItem, DbConnectionHelper conn, DbCommand cmd)
        {

            var parms = cmd.Parameters;
            parms.Add(conn.CreateParameter(FormatParamName("BlogRollId"), blogRollItem.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("Title"), blogRollItem.Title));
            parms.Add(conn.CreateParameter(FormatParamName("Description"), blogRollItem.Description));
            parms.Add(conn.CreateParameter(FormatParamName("BlogUrl"), (blogRollItem.BlogUrl != null ? (object)blogRollItem.BlogUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("FeedUrl"), (blogRollItem.FeedUrl != null ? (object)blogRollItem.FeedUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("Xfn"), blogRollItem.Xfn));
            parms.Add(conn.CreateParameter(FormatParamName("SortIndex"), blogRollItem.SortIndex));
        }

        /// <summary>
        /// The add referrers parameters to command.
        /// </summary>
        /// <param name="referrer">
        /// The referrer.
        /// </param>
        /// <param name="conn">
        /// The connection.
        /// </param>
        /// <param name="cmd">
        /// The command.
        /// </param>
        private void AddReferrersParametersToCommand(Referrer referrer, DbConnectionHelper conn, DbCommand cmd)
        {
            var parms = cmd.Parameters;

            parms.Add(conn.CreateParameter("ReferrerId", referrer.Id.ToString()));
            parms.Add(conn.CreateParameter(FormatParamName("ReferralDay"), referrer.Day));
            parms.Add(conn.CreateParameter(FormatParamName("ReferrerUrl"), (referrer.ReferrerUrl != null ? (object)referrer.ReferrerUrl.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("ReferralCount"), referrer.Count));
            parms.Add(conn.CreateParameter(FormatParamName("Url"), (referrer.Url != null ? (object)referrer.Url.ToString() : DBNull.Value)));
            parms.Add(conn.CreateParameter(FormatParamName("IsSpam"), referrer.PossibleSpam));
        }

        /// <summary>
        /// The delete old referrers.
        /// </summary>
        private void DeleteOldReferrers()
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Referrers WHERE ReferralDay < {1}ReferralDay", this.tablePrefix, this.parmPrefix)))
                    {
                        var cutoff = DateTime.Today.AddDays(-BlogSettings.Instance.NumberOfReferrerDays);

                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("ReferralDay"), cutoff));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion


        /// <summary>
        /// Creates a new DbConnectionHelper for this DbBlogProvider instance.
        /// </summary>
        /// <returns></returns>
        private DbConnectionHelper CreateConnection()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[this.connStringName];
            return new DbConnectionHelper(settings);
        }

    }
}