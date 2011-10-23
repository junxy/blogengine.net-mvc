using BlogEngine.Core.Json;

namespace Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Specialized;
    using System.Web.Security;
    using BlogEngine.Core;

    public partial class Dashboard : System.Web.UI.Page
    {
        #region Properties

        public static int PostsPublished { get; set; }
        public static int PagesCount { get; set; }
        public static int CommentsAll { get; set; }
        public static int CommentsUnapproved { get; set; }
        public static int CommentsSpam { get; set; }
        public static int CategoriesCount { get; set; }
        public static int TagsCount { get; set; }
        public static int UsersCount { get; set; }
        public static int DraftPostCount { get; set; }
        public static int DraftPageCount { get; set; }

        private readonly StringCollection CategoryList = new StringCollection();
        private readonly StringCollection TagList = new StringCollection();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);

            var postsLinq = from posts in Post.Posts where posts.IsPublished == true select posts.Id;
            PostsPublished = postsLinq.Count();

            PagesCount = BlogEngine.Core.Page.Pages.Count();

            CommentsAll = 0;
            CommentsUnapproved = 0;
            CommentsSpam = 0;
            DraftPostCount = 0;
            DraftPageCount = (from p in BlogEngine.Core.Page.Pages where p.IsPublished == false select p).Count();  

            foreach (var p in Post.Posts)
            {
                List<Comment> commentList = (from c in p.Comments where c.IsPingbackOrTrackback == false select c).ToList();
                
                CommentsAll += commentList.Count;
                CommentsUnapproved += p.NotApprovedComments.Count;
                CommentsSpam += p.SpamComments.Count;

                if (!p.IsPublished)
                {
                    // add to post drafts
                    DraftPosts.InnerHtml += string.Format("<li><a class='editAction' href=\"Posts/Add_entry.aspx?id={2}\">{0}</a><span class='meta'>Saved: {1} by {3}<span></li>", Context.Server.HtmlEncode(p.Title), p.DateModified.ToShortDateString() + " at " + p.DateModified.ToShortTimeString(), p.Id, p.Author);
                    DraftPosts.Visible = true;
                    DraftPostCount ++;
                }

                foreach (var c in p.Categories)
                {
                    if (!CategoryList.Contains(c.Id.ToString()))
                        CategoryList.Add(c.Id.ToString());
                }
                CategoriesCount = CategoryList.Count;

                foreach (var t in p.Tags)
                {
                    if (!TagList.Contains(t))
                        TagList.Add(t);
                }
                TagsCount = TagList.Count;

                int uCount = 0;
                Membership.Provider.GetAllUsers(0, 999, out uCount);
                UsersCount = uCount;
            }

            foreach (var pg in BlogEngine.Core.Page.Pages)
            {
                if (!pg.IsPublished)
                {
                    // add to page drafts
                    DraftPages.InnerHtml += string.Format("<li><a class='editAction' href=\"Pages/EditPage.aspx?id={2}\">{0}</a><span class='meta'>Saved: {1}</span></li>", Context.Server.HtmlEncode(pg.Title), pg.DateModified.ToShortDateString() + " at " + pg.DateModified.ToShortTimeString(), pg.Id);
                    DraftPages.Visible = true;
                }
            }

            if (DraftPostCount == 0)
            {
                DraftPosts.InnerHtml = Resources.labels.youDontHaveAnyDraftPosts;
            }
            if (DraftPageCount == 0)
            {
                DraftPages.InnerHtml = Resources.labels.youDontHaveAnyDraftPages;
            }

        }

        protected string GetCommentsList()
        {
            string commentList = "";
            string commentHeader = "";
            string commentFooter = "";
            List<JsonComment> jsonComments;
            if (BlogSettings.Instance.EnableCommentsModeration)
            {
                jsonComments = JsonComments.GetComments(CommentType.Pending, 10, 1);
                commentHeader = "<h2>" + Resources.labels.recentPendingComments + "</h2>";
                commentFooter += "<a class=\"viewAction\" href=\"Comments/Pending.aspx\">View all pending comments</a>";
            }
            else
            {
                commentHeader = "<h2>" + Resources.labels.recentComments + "</h2>";
                jsonComments = JsonComments.GetComments(CommentType.Approved, 10, 1);
                commentFooter += "<a class=\"viewAction\" href=\"Comments/Approved.aspx\">View all comments</a>";
            }

            if(jsonComments.Count > 0)
            {
                commentList += "<ul>";
                commentList = jsonComments.Aggregate(commentList, (current, jc) => current + string.Format("<li>{0}<span class='teaser'>{1}</span></li>", jc.Title, jc.Teaser));
                commentList += "</ul>";
                commentList += commentFooter;
            }
            return commentHeader + commentList;
        }
    }
}