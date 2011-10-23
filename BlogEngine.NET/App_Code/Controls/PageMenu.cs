// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Builds nested page list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.HtmlControls;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using BlogEngine.Core;

    /// <summary>
    /// Summary description for PageMenu
    /// </summary>
    public class PageMenu : Control
    {
        public PageMenu()
        {
            BlogEngine.Core.Page.Saved += delegate { _Html = null; };
        }

        #region Properties

        private static object _SyncRoot = new object();
        private static string _Html;
        private bool _ulIdSet = false;
        string _curPage = HttpUtility.UrlEncode(GetPageName(HttpContext.Current.Request.RawUrl.ToLower()));

        private string Html
        {
            get
            {
                lock (_SyncRoot)
                {
                    HtmlGenericControl ul = BindPages();
                    System.IO.StringWriter sw = new System.IO.StringWriter();
                    ul.RenderControl(new HtmlTextWriter(sw));
                    _Html = sw.ToString();
                }

                return _Html;
            }
        }

        #endregion

        private HtmlGenericControl BindPages()
        {
            // recursivly get all children of the root page
            HtmlGenericControl ul = GetChildren(Guid.Empty);

            // items that will be appended to the end of menu list
            AddMenuItem(ul, Resources.labels.contact, "~/contact.aspx");

            if (Page.User.Identity.IsAuthenticated)
            {
                AddMenuItem(ul, Resources.labels.logoff, "~/Account/login.aspx?logoff");
            }
            else
            {
                AddMenuItem(ul, Resources.labels.login, "~/Account/login.aspx");
            }

            return ul;
        }

        bool HasChildren(Guid pageId)
        {
            bool returnValue = false;

            foreach (BlogEngine.Core.Page page in BlogEngine.Core.Page.Pages)
            {
                if (page.ShowInList && page.IsPublished)
                {
                    if (page.Parent == pageId)
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }

        HtmlGenericControl GetChildren(Guid parentId)
        {
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            if (!_ulIdSet)
            {
                ul.Attributes.Add("id", "menu-topmenu");
                ul.Attributes.Add("class", "menu");
                _ulIdSet = true;

                AddMenuItem(ul, Resources.labels.home, "~/default.aspx");
                AddMenuItem(ul, Resources.labels.archive, "~/archive.aspx");
            }

            foreach (BlogEngine.Core.Page page in BlogEngine.Core.Page.Pages)
            {
                if (page.ShowInList && page.IsPublished)
                {
                    if (page.Parent == parentId)
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        string pageName = HttpUtility.UrlEncode(GetPageName(page.RelativeLink.ToString().ToLower()));

                        HtmlAnchor anc = new HtmlAnchor();
                        anc.HRef = page.RelativeLink.ToString();
                        anc.InnerHtml = page.Title;
                        anc.Title = page.Description;

                        if (pageName == _curPage)
                        {
                            anc.Attributes.Add("class", "current");
                        }

                        li.Controls.Add(anc);

                        if (HasChildren(page.Id))
                        {
                            HtmlGenericControl subUl = GetChildren(page.Id);
                            li.Controls.Add(subUl);
                        }
                        ul.Controls.Add(li);
                    }
                }
            }

            return ul;
        }

        private void AddMenuItem(HtmlGenericControl ul, string pageName, string pageUrl)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlAnchor anc = new HtmlAnchor();

            anc.HRef = pageUrl;
            anc.InnerHtml = pageName;
            anc.Title = pageName;

            if (GetPageName(pageUrl).ToLower() == _curPage.ToLower())
            {
                anc.Attributes.Add("class", "current");
            }

            li.Controls.Add(anc);
            ul.Controls.Add(li);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(Html);
            writer.Write(Environment.NewLine);
        }

        public static string GetPageName(string requestPath)
        {
            if (requestPath.IndexOf('?') != -1)
                requestPath = requestPath.Substring(0, requestPath.IndexOf('?'));
            return requestPath.Remove(0, requestPath.LastIndexOf("/") + 1).ToLower();
        }
    }
}