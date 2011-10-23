using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Diagnostics;
using System.Security;

namespace BlogEngine.Core
{
    /// <summary>
    /// Class to provide a unified area of authentication/authorization checking.
    /// </summary>
    public static partial class Security
    {
        static Security()
        {
        }

        #region "Properties"

        /// <summary>
        /// If the current user is authenticated, returns the current MembershipUser. If not, returns null. This is just a shortcut to Membership.GetUser().
        /// </summary>
        public static MembershipUser CurrentMembershipUser
        {
            get
            {
                return Membership.GetUser();
            }
        }

        /// <summary>
        /// Gets the current user for the current HttpContext.
        /// </summary>
        /// <remarks>
        /// This should always return HttpContext.Current.User. That value and Thread.CurrentPrincipal can't be
        /// guaranteed to always be the same value, as they can be set independently from one another. Looking
        /// through the .Net source, the System.Web.Security.Roles class also returns the HttpContext's User.
        /// </remarks>
        public static System.Security.Principal.IPrincipal CurrentUser
        {
            get
            {
                return HttpContext.Current.User;
            }
        }

        /// <summary>
        /// Gets whether the current user is logged in.
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return Security.CurrentUser.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets whether the currently logged in user is in the administrator role.
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                return (Security.IsAuthenticated && Security.CurrentUser.IsInRole(BlogSettings.Instance.AdministratorRole));
            }
        }

        /// <summary>
        /// Returns an IEnumerable of Rights that belong to the ecurrent user.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Right> CurrentUserRights()
        {
            return Right.GetRights(Security.GetCurrentUserRoles());
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// If the current user does not have the requested right, either redirects to the login page,
        /// or throws a SecurityException.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="redirectToLoginPage">
        /// If true and user does not have rights, redirects to the login page.
        /// If false and user does not have rights, throws a security exception.
        /// </param>
        public static void DemandUserHasRight(Rights right, bool redirectToLoginPage)
        {
            DemandUserHasRight(AuthorizationCheck.HasAny, redirectToLoginPage, new[] { right });
        }

        /// <summary>
        /// If the current user does not have the requested rights, either redirects to the login page,
        /// or throws a SecurityException.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="redirectIfUnauthorized">
        /// If true and user does not have rights, redirects to the login page or homepage.
        /// If false and user does not have rights, throws a security exception.
        /// </param>
        /// <param name="rights"></param>
        public static void DemandUserHasRight(AuthorizationCheck authCheck, bool redirectIfUnauthorized, params Rights[] rights)
        {
            if (!IsAuthorizedTo(authCheck, rights))
            {
                if (redirectIfUnauthorized)
                {
                    RedirectForUnauthorizedRequest();
                }
                else
                {
                    throw new SecurityException("User doesn't have the right to perform this");
                }
            }
        }

        public static void RedirectForUnauthorizedRequest()
        {
            HttpContext context = HttpContext.Current;
            Uri referrer = context.Request.UrlReferrer;
            bool isFromLoginPage = referrer != null && referrer.LocalPath.IndexOf("/Account/login.aspx", StringComparison.OrdinalIgnoreCase) != -1;

            // If the user was just redirected from the login page to the current page,
            // we will then redirect them to the homepage, rather than back to the
            // login page to prevent confusion.
            if (isFromLoginPage)
            {
                context.Response.Redirect(Utils.RelativeWebRoot);
            }
            else
            {
                context.Response.Redirect(string.Format("~/Account/login.aspx?ReturnURL={0}", HttpUtility.UrlPathEncode(context.Request.RawUrl)));
            }
        }

        /// <summary>
        /// Returns whether or not the current user has the passed in Right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(Rights right)
        {
            return Right.HasRight(right, Security.GetCurrentUserRoles());
        }

        /// <summary>
        /// Returns whether the current user passes authorization on the rights based on the given AuthorizationCheck.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(AuthorizationCheck authCheck, IEnumerable<Rights> rights)
        {
            if (rights.Count() == 0)
            {
                // Always return false for this. If there's a mistake where authorization
                // is being checked for on an empty collection, we don't want to return 
                // true.
                return false;
            }
            else
            {
                var roles = Security.GetCurrentUserRoles();

                if (authCheck == AuthorizationCheck.HasAny)
                {
                    foreach (var right in rights)
                    {
                        if (Right.HasRight(right, roles))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else if (authCheck == AuthorizationCheck.HasAll)
                {
                    bool authCheckPassed = true;

                    foreach (var right in rights)
                    {
                        if (!Right.HasRight(right, roles))
                        {
                            authCheckPassed = false;
                            break;
                        }
                    }
                    return authCheckPassed;
                }
                else
                {
                    throw new NotSupportedException();
                }

            }
        }

        /// <summary>
        /// Returns whether a role is a System role.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>true if the roleName is a system role, otherwiser false</returns>
        public static bool IsSystemRole(string roleName)
        {
            if (roleName.Equals(BlogSettings.Instance.AdministratorRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(BlogSettings.Instance.AnonymousRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(BlogSettings.Instance.EditorsRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the current user passes authorization on the rights based on the given AuthorizationCheck.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(AuthorizationCheck authCheck, params Rights[] rights)
        {
            return IsAuthorizedTo(authCheck, rights.ToList());
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Helper method that returns the correct roles based on authentication.
        /// </summary>
        /// <returns></returns>
        public static string[] GetCurrentUserRoles()
        {
            if (!IsAuthenticated)
            {
                // This needs to be recreated each time, because it's possible 
                // that the array can fall into the wrong hands and then someone
                // could alter it. 
                return new[] { BlogSettings.Instance.AnonymousRole };
            }
            else
            {
                return Roles.GetRolesForUser();
            }
        }

        /// <summary>
        /// Impersonates a user for the duration of the HTTP request.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>True if the credentials are correct and impersonation succeeds</returns>
        public static bool ImpersonateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            CustomIdentity identity = new CustomIdentity(username, password);
            if (!identity.IsAuthenticated) { return false; }

            CustomPrincipal principal = new CustomPrincipal(identity);

            // Make the custom principal be the user for the rest of this request.
            HttpContext.Current.User = principal;

            return true;
        }

        #endregion
    }


    /// <summary>
    /// Enum for setting how rights should be checked for.
    /// </summary>
    public enum AuthorizationCheck
    {
        /// <summary>
        /// A user will be considered authorized if they have any of the given Rights.
        /// </summary>
        HasAny,

        /// <summary>
        /// A user will be considered authorized if they have all of the given Rights.
        /// </summary>
        HasAll
    }

}
