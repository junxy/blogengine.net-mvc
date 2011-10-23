using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Be = BlogEngine.Core;
using BlogEngine.Core;
using BlogEngine.MVC.ViewsModels;
using BlogEngine.MVC.Framework;

namespace BlogEngine.MVC.Controllers
{
    [Compress]
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index(int page = 1)
        {
            var model = new IndexViewModel()
            {
                Base = GetBaseViewModel(),
                ContentBy = ServingContentBy.AllContent,
                Posts = Be.Post.Posts.ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; })),

            };

            ViewData["BlogSettings.Name"] = BlogSettings.Instance.Name;

            return View(model);
        }

        public ActionResult Post(string Id)
        {

            var model = new PostViewModel() { Base = GetBaseViewModel() };

            Guid guid;

            if ((!Utils.StringIsNullOrWhitespace(Id)) && Id.TryParse(out guid))
            {
                model.Post = Be.Post.GetPost(guid);
            }
            else
            {
                model.Post = Be.Post.Posts.Find(
                p => Id.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase));
            }

            ViewData["BlogSettings.Name"] = BlogSettings.Instance.Name;

            return View(model);
        }

        public ActionResult Page()
        {
            return View();
        }

        public ActionResult Archive()
        {
            var model = new ArchiveViewModel()
            {
                Base = GetBaseViewModel(),
                Categories = Category.Categories,
                NoCatList = Be.Post.Posts.FindAll(delegate(Post p) { return p.Categories.Count == 0 && p.IsVisible; })
            };

            return View(model);
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
            }

            return View();
        }

        public ActionResult Search()
        {
            return View();
        }


        private BaseViewModel GetBaseViewModel()
        {
            var model = new BaseViewModel()
            {
                Keywords = "",
                Description = Server.HtmlEncode(BlogSettings.Instance.Description),
                Author = Server.HtmlEncode(BlogSettings.Instance.AuthorName),

            };

            if (!BlogSettings.Instance.UseBlogNameInPageTitles)
                model.Title = BlogSettings.Instance.Name + " | ";

            if (!string.IsNullOrEmpty(BlogSettings.Instance.Description))
                model.Title += Server.HtmlEncode(BlogSettings.Instance.Description);

            return model;
        }
    }
}
