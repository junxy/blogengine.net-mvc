namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Security.Principal;

    public class CustomIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { return "BlogEngine.NET Custom Identity"; }
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public CustomIdentity(string username, string password)
        {
            if (Utils.StringIsNullOrWhitespace(username))
                throw new ArgumentNullException("username");

            if (Utils.StringIsNullOrWhitespace(password))
                throw new ArgumentNullException("password");

            if (!Membership.ValidateUser(username, password)) { return; }

            _isAuthenticated = true;
            _name = username;
        }
    }
}
