namespace BlogEngine.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Data;
    using System.Data.Common;
    using System.Web.Security;

    /// <summary>
    /// Generic Db Role Provider
    /// </summary>
    public class DbRoleProvider : RoleProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The application name.
        /// </summary>
        private string applicationName;

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

        #region Properties

        /// <summary>
        ///     Gets or sets the application name as set in the web.config
        ///     otherwise returns BlogEngine.  Set will throw an error.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds all users in user array to all roles in role array
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        var parms = cmd.Parameters;

                        foreach (var user in usernames)
                        {
                            parms.Clear();
                            cmd.CommandText = string.Format("SELECT UserID FROM {0}Users WHERE UserName = {1}user", this.tablePrefix, this.parmPrefix);

                            parms.Add(conn.CreateParameter(FormatParamName("user"), user));

                            var userId = Int32.Parse(cmd.ExecuteScalar().ToString());

                            foreach (var role in roleNames)
                            {
                                if (!role.Equals(BlogSettings.Instance.AnonymousRole))
                                {
                                    parms.Clear();
                                    cmd.CommandText = string.Format("SELECT RoleID FROM {0}Roles WHERE Role = {1}role", this.tablePrefix, this.parmPrefix);

                                    parms.Add(conn.CreateParameter(FormatParamName("role"), role));

                                    var roleId = Int32.Parse(cmd.ExecuteScalar().ToString());

                                    cmd.CommandText = string.Format("INSERT INTO {0}UserRoles (UserID, RoleID) VALUES ({1}uID, {1}rID)", this.tablePrefix, this.parmPrefix);

                                    parms.Add(conn.CreateParameter(FormatParamName("uID"), userId));
                                    parms.Add(conn.CreateParameter(FormatParamName("rID"), roleId));

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }

            // This needs to be called in order to keep the Right class in sync.
            Right.RefreshAllRights();
        }

        /// <summary>
        /// Adds a new role to the database
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("INSERT INTO {0}Roles (role) VALUES ({1}role)", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), roleName));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // This needs to be called in order to keep the Right class in sync.
            Right.RefreshAllRights();
        }

        /// <summary>
        /// Removes a role from database
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.</param>
        /// <returns>The delete role.</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var success = false;

            if (!Security.IsSystemRole(roleName))
            {
                using (var conn = this.CreateConnection())
                {
                    if (conn.HasConnection)
                    {
                        using (var cmd = conn.CreateTextCommand(string.Format("DELETE FROM {0}Roles WHERE Role = {1}role", this.tablePrefix, this.parmPrefix)))
                        {
                            cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), roleName));
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                    }
                }
            }

            // This needs to be called in order to keep the Right class in sync.
            Right.RefreshAllRights();

            return success;
        }

        /// <summary>
        /// Returns all users in selected role with names that match usernameToMatch
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>
        /// A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch"/> and the user is a member of the specified role.
        /// </returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var users = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT u.UserName FROM {0}Users u INNER JOIN {0}UserRoles ur ON u.UserID = ur.UserID INNER JOIN {0}Roles r ON ur.RoleID = r.RoleID WHERE r.Role  = {1}role AND u.UserName LIKE {1}name", this.tablePrefix, this.parmPrefix);
                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("role"), roleName));
                        parms.Add(conn.CreateParameter(FormatParamName("name"), string.Format("{0}%", usernameToMatch)));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    users.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return users.ToArray();
        }

        /// <summary>
        /// Returns array of all roles in database
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            var roles = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT role FROM {0}Roles", this.tablePrefix)))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    roles.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return roles.ToArray();
        }

        /// <summary>
        /// Return an array of roles that user is in
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            var roles = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT r.role FROM {0}Roles r INNER JOIN {0}UserRoles ur ON r.RoleID = ur.RoleID INNER JOIN {0}Users u ON ur.UserID = u.UserID WHERE u.UserName = {1}name", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("name"), username));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    roles.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return roles.ToArray();
        }

        /// <summary>
        /// Returns array of users in selected role
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the configured applicationName.
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            var users = new List<string>();

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT u.UserName FROM {0}Users u INNER JOIN {0}UserRoles ur ON u.UserID = ur.UserID INNER JOIN {0}Roles r ON ur.RoleID = r.RoleID WHERE r.Role  = {1}role", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), roleName));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    users.Add(rdr.GetString(0));
                                }
                            }
                        }
                    }
                }
            }

            return users.ToArray();
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
                name = "DbMembershipProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Generic Database Membership Provider");
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

            if (config["applicationName"] == null)
            {
                // default to BlogEngine
                config["applicationName"] = "BlogEngine";
            }

            this.applicationName = config["applicationName"];
            config.Remove("applicationName");

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
        /// Check to see if user is in a role
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>The is user in role.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            var roleFound = false;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    var sqlQuery = string.Format("SELECT r.roleID FROM {0}Roles r INNER JOIN {0}UserRoles ur ON r.RoleID = ur.RoleID INNER JOIN {0}Users u ON ur.UserID = u.UserID WHERE u.UserName = {1}name AND r.role = {1}role", this.tablePrefix, this.parmPrefix);

                    using (var cmd = conn.CreateTextCommand(sqlQuery))
                    {
                        var parms = cmd.Parameters;
                        parms.Add(conn.CreateParameter(FormatParamName("name"), username));
                        parms.Add(conn.CreateParameter(FormatParamName("role"), roleName));
                        
                        using (var rdr = cmd.ExecuteReader())
                        {
                            roleFound = rdr.Read();
                        }
                    }
                }
            }

            return roleFound;
        }

        /// <summary>
        /// Removes all users in user array from all roles in role array
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        var parms = cmd.Parameters;

                        foreach (var user in usernames)
                        {
                            parms.Clear();
                            cmd.CommandText = string.Format("SELECT UserID FROM {0}Users WHERE UserName = {1}user", this.tablePrefix, this.parmPrefix);

                            parms.Add(conn.CreateParameter(FormatParamName("user"), user));
                            
                            int userId;
                            try
                            {
                                userId = Int32.Parse(cmd.ExecuteScalar().ToString());
                            }
                            catch
                            {
                                userId = 0;
                            }

                            if (userId <= 0)
                            {
                                continue;
                            }

                            foreach (var role in roleNames)
                            {
                                parms.Clear();
                                cmd.CommandText = string.Format("SELECT RoleID FROM {0}Roles WHERE Role = {1}role", this.tablePrefix, this.parmPrefix);

                                parms.Add(conn.CreateParameter(FormatParamName("role"), role));

                                var roleId = Int32.Parse(cmd.ExecuteScalar().ToString());

                                cmd.CommandText = string.Format("DELETE FROM {0}UserRoles WHERE UserID = {1}uID AND RoleID = {1}rID", this.tablePrefix, this.parmPrefix);

                                parms.Add(conn.CreateParameter(FormatParamName("uID"), userId));
                                parms.Add(conn.CreateParameter(FormatParamName("rID"), roleId));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            // This needs to be called in order to keep the Right class in sync.
            Right.RefreshAllRights();
        }

        /// <summary>
        /// Checks to see if role exists
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>The role exists.</returns>
        public override bool RoleExists(string roleName)
        {
            var roleFound = false;

            using (var conn = this.CreateConnection())
            {
                if (conn.HasConnection)
                {

                    using (var cmd = conn.CreateTextCommand(string.Format("SELECT roleID FROM {0}Roles WHERE role = {1}role", this.tablePrefix, this.parmPrefix)))
                    {
                        cmd.Parameters.Add(conn.CreateParameter(FormatParamName("role"), roleName));

                        using (var rdr = cmd.ExecuteReader())
                        {
                            roleFound = rdr.Read();
                        }
                    }
                }
            }

            return roleFound;
        }

        #endregion

        #region "Methods"

        private DbConnectionHelper CreateConnection()
        {
            var settings = ConfigurationManager.ConnectionStrings[this.connStringName];
            return new DbConnectionHelper(settings);
        }


        /// <summary>
        /// Returns a formatted parameter name to include this DbRoleProvider instance's paramPrefix.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private string FormatParamName(string parameterName)
        {
            return String.Format("{0}{1}", this.parmPrefix, parameterName);
        }

        #endregion

    }
}